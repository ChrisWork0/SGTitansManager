using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SGTitansManager.Models;
using SGTitansManager.Models.Dtos;
using SGTitansManager.Server.Database;
using SGTitansManager.Server.Repositories;

namespace SGTitansManager.Server.Controller;

[ApiController]
[Route("api/[controller]")]
public class PlayerController : ControllerBase
{
    private readonly PlayerRepository _playerRepo;
    private readonly UserRepository _userRepo;
    private readonly string[] _allIncludes = [nameof(Player.Availabilities), 
        nameof(Player.ChampionPool), nameof(Player.PlayerRanks)];
    
    public PlayerController(ManagerContext dbContext)
    {
        _playerRepo = new PlayerRepository(dbContext);
        _userRepo = new UserRepository(dbContext);
    }

    [HttpGet]
    [Authorize(Policy = Policies.CoachOnly)]
    public async Task<IActionResult> Get([FromQuery] bool withDeleted = false)
        => Ok(await _playerRepo.Get(withDeleted));

    [HttpGet("{playerId}")]
    [Authorize]
    public async Task<IActionResult> GetById(Guid playerId)
    {
        var user = await _userRepo.GetUserByPlayerId(playerId);
        if (user == null)
            return NotFound();
        if (!HttpContext.User.IsInRole(nameof(Role.Admin)) 
            && user.Id != Guid.Parse(HttpContext.User.Claims.First(c => c.Type == "userId").Value))
            return Forbid();
        var player = await _playerRepo.GetByIdWithIncludes(playerId, _allIncludes);
        if (player == null)
            return NotFound();
        
        return Ok(player);
    }

    [HttpPost("{userId}")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Post(Guid userId, Player player)
    {
        var user = await _userRepo.GetByIdWithIncludes(userId, [nameof(UserDto.Member)]);
        if (user?.Member == null)
            return NotFound();
        user.Member.PlayerId = player.Id;
        user.Member.Player = player;
        await _playerRepo.AddAndSave(player);
        return Ok(player);
    }

    [HttpPatch("{playerId}")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Patch(Guid playerId, [FromBody] JsonPatchDocument<Player> updates)
    {
        var result = await _playerRepo.Patch(playerId, updates);
        if (!result.Success)
            return StatusCode(result.StatusCode ?? 500);
        return Ok(result.Model);
    }

    [HttpDelete("{playerId}")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Delete(Guid playerId)
    {
        var user = await _userRepo.GetUserByPlayerId(playerId); 
        if (user?.Member == null)
            return NotFound();
        user.Member.PlayerId = null;
        user.Member.Player = null;
        
        var result = await _playerRepo.Delete(playerId);
        if (!result)
            return NotFound();
        return NoContent();
    }
}