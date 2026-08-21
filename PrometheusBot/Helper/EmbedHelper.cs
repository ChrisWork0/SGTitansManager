using NetCord;
using NetCord.Rest;
using PrometheusBot.Dtos;

namespace PrometheusBot.Helper;

public static class EmbedHelper
{
    private static string _copyRight = "© Prometheus created by Trafy";
    
    public static EmbedProperties CreateEmbed(EmbedContent content)
    {
        return new EmbedProperties()
        {
            Author = new EmbedAuthorProperties
            {
                Name = content.AuthorName,
                IconUrl = content.AuthorIcon
            },
            Title = content.Title,
            Description = content.Description,
            Thumbnail = new EmbedThumbnailProperties(content.ThumbnailUrl),
            Image = 
                new EmbedImageProperties(content.ImageUrl),
            Fields = content.Fields,
            Color = new Color(0xffa600),
            Footer = new EmbedFooterProperties {
                IconUrl = content.FooterIcon,
                Text = _copyRight
            },
            Timestamp = DateTimeOffset.Now,
        };
    }
}