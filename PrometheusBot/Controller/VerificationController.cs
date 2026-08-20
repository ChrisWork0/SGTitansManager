using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using NetCord.Gateway;
using PrometheusBot.Dtos;
using PrometheusBot.Services;

namespace PrometheusBot.Controller;

[ApiController]
[Route("internal/[controller]")]
public class VerificationController : ControllerBase
{
    private readonly VerificationService _verificationService;
    private readonly string _applicationName;

    public VerificationController(VerificationService verificationService, IConfiguration configuration)
    {
        _verificationService = verificationService;
        _applicationName = configuration.GetSection("BackendName").Value ?? "";
    }

    [HttpPost("send/password-change-request")]
    public async Task<IActionResult> Verify(SendVerificationRequest request, CancellationToken cancellationToken)
    {
        if (!ulong.TryParse(request.DiscordId, out var discordUserId))
            return BadRequest("Unvalid Discord ID");
        var content = new VerificationContent();
        content.Title = $"Passwortänderung bei **'{_applicationName}'**";
        content.Message = "\n\nWillst du wirklich dein Passwort ändern? 🔑\n\n" +
                          "*Diese Anfrage läuft in 5 Minuten ab. ⏱️*";
        var result = await _verificationService.SendVerificationRequestAsync(discordUserId, request.TokenId, 
            cancellationToken, content);
        if (!result.Success)
            return BadRequest(result.Message);
        return Ok(result.Data);
    }

    [HttpPost("send/login-request")]
    public async Task<IActionResult> VerifyLogin(SendVerificationRequest request, CancellationToken cancellationToken)
    {
        if (!ulong.TryParse(request.DiscordId, out var discordUserId))
            return BadRequest("Unvalid Discord ID");
        var content = new VerificationContent();
        content.Title = $"Login bei **'{_applicationName}'**";
        content.Message = "\n\nWillst du dich gerade wirklich einloggen? 🔑\n\n" +
                         "**⚠️ Wenn nicht, dann versuche bitte dein Passwort zu ändern! ⚠️**\n\n" +
                         "*Diese Anfrage läuft in 5 Minuten ab. ⏱️*";
        var result = await _verificationService.SendVerificationRequestAsync(discordUserId, request.TokenId,
            cancellationToken, content);
        if (!result.Success)
            return BadRequest(result.Message);
        return Ok(result.Data);
    }
}