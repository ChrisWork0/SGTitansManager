using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGTitansManager.Server.Database;
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
    
    public LoginController(ManagerContext dbContext, AuthorizationService authorizationService)
    {
        _userRepo = new UserRepository(dbContext);
        _authorizationService = authorizationService;
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        var user = await _userRepo.LoginByUserName(loginDto.Username, loginDto.Password.Sha256());
        if (user == null)
            return Unauthorized("Wrong username or password");
        var token = _authorizationService.CreateJsonWebToken(user);
        HttpContext.Response.Headers.Append("Authorization", 
            new AuthenticationHeaderValue("Bearer", token).ToString());
        return NoContent();
    }
}