using NetCord;
using NetCord.Gateway;
using NetCord.Rest;
using PrometheusBot.Dtos;
using PrometheusBot.Helper;

namespace PrometheusBot.Services;

public class VerificationService(
    GatewayClient client,
    PendingVerificationService pendingVerificationService,
    ILogger<VerificationService> logger)
{
    public async Task<ResultDto> SendVerificationRequestAsync(ulong discordUserId, Guid tokenId, CancellationToken stoppingToken,
    EmbedContent content)
    {
        string message = "";
        DMChannel dmChannel;
        try
        {
            dmChannel = await client.Rest.GetDMChannelAsync(discordUserId);
        }
        catch (Exception e)
        {
            message = $"Failed to open DM-Channel for '{discordUserId}'";
            logger.LogError(e, message);
            return new ResultDto { Message = message };
        }
        
        await dmChannel.SendMessageAsync(new MessageProperties
        {
            Embeds = [EmbedHelper.CreateEmbed(content)],
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
            var confirmed = await pendingVerificationService.WaitForResultAsync(tokenId, TimeSpan.FromMinutes(5),
                stoppingToken);
            return new ResultDto { Success = true, Message = message, Data = confirmed };
        }
        catch (InvalidOperationException e)
        {
            message = $"Failed to verify: Duplicate request for token {tokenId}";
            logger.LogWarning(e, message);
            return new ResultDto { Message = message };
        }
        catch (OperationCanceledException)
        {
            message = "Verification expired";
            return new ResultDto { Success = true, Message = message, Data = false };
        }
    }
}