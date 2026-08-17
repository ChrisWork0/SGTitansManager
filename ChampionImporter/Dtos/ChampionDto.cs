using System.Text.Json.Serialization;

namespace ChampionImporter.Dtos;

public class ChampionDto
{
    [JsonPropertyName("id")] 
    public string Name { get; set; } = "";

    [JsonPropertyName("key")]           
    public int Id { get; set; }
    
    [JsonPropertyName("full")]
    public string ImagePath { get; set; } = "";

    [JsonPropertyName("tags")] 
    public List<string> Tags { get; set; } = [];
} 