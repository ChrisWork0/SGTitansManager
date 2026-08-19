namespace PrometheusBot.Dtos;

public class SendVerificationRequest
{
    public string DiscordId { get; set; } = "";
    public Guid TokenId { get; set; }
}