using NetCord;
using NetCord.Gateway;
using NetCord.Rest;
using PrometheusBot.Dtos;
using PrometheusBot.Helper;

namespace PrometheusBot.Services;

public class VerificationService
{
    private readonly GatewayClient _client;
    private readonly PendingVerificationService _pendingVerificationService;
    private readonly ILogger<VerificationService> _logger;
    private readonly string _applicationName;

    public VerificationService(GatewayClient client, PendingVerificationService pendingVerificationService,
        ILogger<VerificationService> logger, IConfiguration configuration)
    {
        _client = client;
        _pendingVerificationService = pendingVerificationService;
        _logger = logger;
        _applicationName = configuration.GetSection("BackendName").Value ?? "";
    }

    public async Task<ResultDto> SendVerificationRequestAsync(ulong discordUserId, Guid tokenId, CancellationToken stoppingToken)
    {
        string message = "";
        DMChannel dmChannel;
        try
        {
            dmChannel = await _client.Rest.GetDMChannelAsync(discordUserId);
        }
        catch (Exception e)
        {
            message = $"Failed to open DM-Channel for {discordUserId}";
            _logger.LogError(e, message);
            return new ResultDto { Message = message };
        }

        string title = $"Passwortänderung bei **'{_applicationName}'**";
        string requestMessage = "\nWillst du wirklich dein Passwort ändern? 🔑 \n⏱️ Diese Anfrage läuft in 5 Minuten ab.\n";
        await dmChannel.SendMessageAsync(new MessageProperties
        {
            Embeds = [EmbedHelper.InfoEmbed(title, requestMessage)],
            Components =
            [
                new ActionRowProperties
                {
                    new ButtonProperties($"verify_confirm:{tokenId}", "Ja", ButtonStyle.Success),
                    new ButtonProperties($"verify_deny:{tokenId}", "Nein", ButtonStyle.Danger)
                }
            ]
        });

        try
        {
            message = "Successfully verified!";
            var confirmed = await _pendingVerificationService.WaitForResultAsync(tokenId, TimeSpan.FromMinutes(5),
                stoppingToken);
            return new ResultDto { Success = true, Message = message, Data = confirmed };
        }
        catch (InvalidOperationException e)
        {
            message = $"Failed to verify: Duplicate request for token {tokenId}";
            _logger.LogWarning(e, message);
            return new ResultDto { Message = message };
        }
        catch (OperationCanceledException)
        {
            message = "Verification expired";
            return new ResultDto { Success = true, Message = message, Data = false };
        }
    }
}