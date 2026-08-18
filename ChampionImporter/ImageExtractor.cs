namespace ChampionImporter;

public static class ImageExtractor
{
    private static readonly HttpClient _httpClient = new();
    
    public static async Task<string> GetImageFromClient(string url, string champion)
    {
        url += $"/{champion}";
        byte[] imageBytes = await _httpClient.GetByteArrayAsync(url);
        string base64String = Convert.ToBase64String(imageBytes);
        return base64String;
    }
}