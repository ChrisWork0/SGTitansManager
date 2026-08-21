using NetCord.Rest;

namespace PrometheusBot.Dtos;

public class EmbedContent
{
    public string? AuthorName { get; set; }
    public string? AuthorIcon  { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? FooterIcon { get; set; }
    public string? FooterText { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? ImageUrl { get; set; }
    public List<EmbedFieldProperties> Fields { get; set; } = [];
}