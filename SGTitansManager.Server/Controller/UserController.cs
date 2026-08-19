using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SGTitansManager.Models;
using SGTitansManager.Models.Dtos;
using SGTitansManager.Server.Database;
using SGTitansManager.Server.Repositories;
using SGTitansManager.Server.Services;

namespace SGTitansManager.Server.Controller;

[ApiController]
[Route("api/[controller]")]

public class UserController : ControllerBase
{
    private readonly UserRepository _userRepo; 
    private readonly VerificationService _verificationService;
    
    public UserController(ManagerContext context, VerificationService verificationService)
    {
        _userRepo = new UserRepository(context);
        _verificationService = verificationService;
    }

    [Authorize(Policy = Policies.AdminOnly)]
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery]bool withDeleted = false)
    {
        var users = await _userRepo.Get(withDeleted);
        return Ok(users.Select(u => new UserDto
        {
            Id = u.Id,
            UserName = u.UserName,
            Role = u.Role,
            IsActive = u.IsActive,
        }));
    }

    [Authorize]
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetById(Guid userId)
    {
        if (!HttpContext.User.IsInRole(nameof(Role.Admin))
                                       && userId != Guid.Parse(HttpContext.User.Claims.First(c => c.Type == "userId").Value))
            return Forbid();
        
        var user = await _userRepo.GetUserWithMemberAndPlayer(userId);
        if (user == null)
            return NotFound();
        return Ok(new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Role = user.Role,
            IsActive = user.IsActive,
            Member = user.Member
        });
    }

    [Authorize(Policy = Policies.AdminOnly)]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateUserDto createUser)
    {
        if (!ulong.TryParse(createUser.DiscordId, out var discordId))
            return BadRequest("Unvalid Discord ID");
        var member = new Member
        {
            DiscordUser = createUser.DiscordId,
            MemberSince = createUser.MemberSince
        };
        
        var user = new AppUser
        {
            PasswordHash = createUser.Password.Sha256(),
            UserName = createUser.UserName,
            Role = createUser.Role,
            IsActive = true,
            Member = member
        };
        
        await _userRepo.AddAndSave(user);
        return NoContent();
    }

    [Authorize(Policy = Policies.AdminOnly)]
    [HttpPatch("{userId}")]
    public async Task<IActionResult> Patch(Guid userId, JsonPatchDocument<AppUser> updates)
    {
        var result = await _userRepo.Patch(userId, updates);
        if (!result.Success)
            return StatusCode(result.StatusCode ?? 500);
        return Ok(result.Model);
    }

    [Authorize]
    [HttpPost("password-change/request")]
    public async Task<IActionResult> RequestPasswordChange(string newPassword)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.First(c => c.Type == "userId").Value);
        var user = await _userRepo.GetByIdWithIncludes(userId, [nameof(AppUser.Member)]);
        if (user?.Member == null) 
            return NotFound();
        if (!await _verificationService.VerifyPasswordChange(user.Member.DiscordUser, userId))
            return BadRequest();
        return NoContent();
    }
}