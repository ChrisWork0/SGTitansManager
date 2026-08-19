namespace SGTitansManager.Server.Services;

public class UserService(IHttpClientFactory httpClientFactory, ILogger<UserService> logger)
{
    public string CreateRecoveryCode()
    {
        string codeString = "";
        Random random = new();
        for (int i = 0; i < 10; i++)
            codeString += random.Next(0,9).ToString();
        
        return codeString;
    }
    
    public async Task<bool> VerifyPasswordChange(string discordId, Guid userId)
    {
        var client = httpClientFactory.CreateClient("PrometheusBot");
        try
        {
            var request = await client.PostAsJsonAsync("internal/verification/send",
                new { DiscordId = discordId, TokenId = userId });

            if (!request.IsSuccessStatusCode)
                return false;
            var response = await request.Content.ReadAsStringAsync();
            if (response == "false")
                return false;
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return false;
        }
        
    }
}