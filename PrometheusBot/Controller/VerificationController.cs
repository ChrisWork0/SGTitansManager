using Microsoft.AspNetCore.Mvc;
using NetCord.Gateway;
using PrometheusBot.Dtos;
using PrometheusBot.Services;

namespace PrometheusBot.Controller;

[ApiController]
[Route("internal/[controller]")]
public class VerificationController(VerificationService verificationService) : ControllerBase
{
    [HttpPost("send")]
    public async Task<IActionResult> Verify(SendVerificationRequest request, CancellationToken cancellationToken)
    {
        if (!ulong.TryParse(request.DiscordId, out var discordUserId))
            return BadRequest("Unvalid Discord ID");

        var result = await verificationService.SendVerificationRequestAsync(discordUserId, request.TokenId, 
            cancellationToken);
        if (!result.Success)
            return BadRequest(result.Message);
        return Ok(result.Data);
    }
}