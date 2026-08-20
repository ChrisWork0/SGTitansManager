namespace SGTitansManager.Server.Services;

public class VerificationService
{
    private readonly HttpClient _client;
    private readonly ILogger<VerificationService> _logger;

    public VerificationService(IHttpClientFactory httpClientFactory, ILogger<VerificationService> logger)
    {
        _client = httpClientFactory.CreateClient("PrometheusBot");
        _logger = logger;
    }

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
        try
        {
            var request = await _client.PostAsJsonAsync("internal/verification/send/password-change-request",
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
            _logger.LogError(ex, ex.Message);
            return false;
        }
    }

    public async Task<bool> VerifyLogin(string discordId)
    {
        var tokenId = Guid.NewGuid();
        try
        {
            var request = await _client.PostAsJsonAsync("internal/verification/send/login-request",
                new { DiscordId = discordId, TokenId = tokenId });
            if (!request.IsSuccessStatusCode)
                return false;
            var response = await request.Content.ReadAsStringAsync();
            if (response == "false")
                return false;
            return true;
        }
        
        catch(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return false;
        }
    }
}