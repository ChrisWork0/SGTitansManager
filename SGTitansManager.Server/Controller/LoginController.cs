using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGTitansManager.Server.Database;
using SGTitansManager.Server.Dtos;
using SGTitansManager.Server.Repositories;
using SGTitansManager.Server.Services;

namespace SGTitansManager.Server.Controller;

[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly UserRepository _userRepo;
    private readonly AuthorizationService _authorizationService;
    private readonly VerificationService _verificationService;
    
    public LoginController(ManagerContext dbContext, AuthorizationService authorizationService, 
        VerificationService verificationService)
    {
        _userRepo = new UserRepository(dbContext);
        _authorizationService = authorizationService;
        _verificationService = verificationService;
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        var user = await _userRepo.LoginByUserName(loginDto.Username, loginDto.Password.Sha256());
        if (user?.Member == null)
            return Unauthorized("Wrong username or password");
        if (loginDto.Username != "admin")
            if (!await _verificationService.VerifyLogin(user.Member.DiscordUser))
                return Forbid();
        var token = _authorizationService.CreateJsonWebToken(user);
        HttpContext.Response.Headers.Append("Authorization", 
            new AuthenticationHeaderValue("Bearer", token).ToString());
        return NoContent();
    }
}