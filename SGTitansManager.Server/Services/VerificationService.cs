namespace SGTitansManager.Server.Services;

public class VerificationService(IHttpClientFactory httpClientFactory)
{
    public async Task<bool> VerifyPasswordChange(string discordId, Guid userId)
    {
        var client = httpClientFactory.CreateClient("PrometheusBot");
        var request = await client.PostAsJsonAsync("internal/verification/send",
            new { DiscordId = discordId, TokenId = userId });

        if (!request.IsSuccessStatusCode)
            return false;
        var response = await request.Content.ReadAsStringAsync();
        return true;
    }
}