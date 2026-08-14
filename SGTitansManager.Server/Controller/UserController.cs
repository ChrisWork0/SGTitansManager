using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGTitansManager.Models;
using SGTitansManager.Server.Database;
using SGTitansManager.Server.Repositories;
using SGTitansManager.Server.Services;

namespace SGTitansManager.Server.Controller;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = Policies.AdminOnly)]

public class UserController : ControllerBase
{
    private readonly UserRepository _userRepo; 
    
    public UserController(ManagerContext context)
    {
        _userRepo = new UserRepository(context);
    }

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

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetById(Guid userId)
    {
        var user = await _userRepo.GetByIncludes(userId, ["Member"]);
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

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateUserDto createUser)
    {
        var member = new Member
        {
            DiscordName = createUser.DiscordName,
            MemberSince = createUser.MemberSince
        };
        
        var user = new User
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
    
    [HttpPut("{userId}")]
    public async Task<IActionResult> ActivateUser(Guid userId, [FromQuery] bool active)
    {
        var result = await _userRepo.DeactivateUser(userId, active);
        if (!result.Success)
            return NotFound(result.Message);
        return Ok(result.Message);
    }
}