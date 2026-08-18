using System.Text.Json.Serialization;

namespace ChampionImporter.Dtos;

public class ChampionRootDto
{
    [JsonPropertyName("data")]
    public Dictionary<string, ChampionDto> Data { get; set; } = new ();
}

public class ChampionDto
{
    [JsonPropertyName("id")] 
    public string Name { get; set; } = "";

    [JsonPropertyName("key")] 
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int Id { get; set; }
    
    [JsonPropertyName("image")]
    public ChampionImageDto Image { get; set; } = new();

    [JsonPropertyName("tags")] 
    public List<string> Tags { get; set; } = [];
}

public class ChampionImageDto
{
    [JsonPropertyName("full")]
    public string Full {get; set;} = "";
}