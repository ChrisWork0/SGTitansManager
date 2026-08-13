using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGTitansManager.Models;
using SGTitansManager.Server.Database;
using SGTitansManager.Server.Repositories;

namespace SGTitansManager.Server.Controller;

[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly UserRepository _userRepo;
    
    public LoginController(ManagerContext dbContext)
    {
        _userRepo = new UserRepository(dbContext);
    }

    // [HttpPost]
    // public async Task<IActionResult> Login(LoginDto loginDto)
    // {
    //     
    // }
}