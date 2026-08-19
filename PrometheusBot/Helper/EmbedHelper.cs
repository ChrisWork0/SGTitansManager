using NetCord;
using NetCord.Rest;
using PrometheusBot.Dtos;

namespace PrometheusBot.Helper;

public static class EmbedHelper
{
    private static string _copyRight = "© Prometheus created by Trafy";
    
    public static EmbedProperties CreateStudentEmbed(string message, AvatarUrl avatars)
    {
        return new EmbedProperties()
        {
            Author = new EmbedAuthorProperties
            {
                Name = "Prometheus",
                IconUrl = avatars.Bot
            },
            Title = "List of students",
            Description = "Shows list of all students on this discord.",
            Thumbnail = new EmbedThumbnailProperties(avatars.User),
            Image = 
                new EmbedImageProperties("https://www.gaming-grounds.de/wp-content/uploads/2019/09/league-newlogo-banner_babt.jpg"),
            Fields = [
                new EmbedFieldProperties
                {
                    Name = "Your Students:",
                    Value = message
                }
            ],
            Color = new Color(0xffa600),
            Footer = new EmbedFooterProperties {
                IconUrl = avatars.Creator,
                Text = _copyRight
            },
            Timestamp = DateTimeOffset.Now,
        };
    }

    public static EmbedProperties InfoEmbed(string title, string message, AvatarUrl? avatars = null)
    {
        return new EmbedProperties
        {
            Author = new EmbedAuthorProperties
            {
                Name = "Prometheus",
                IconUrl = avatars?.Bot
            },
            Title = title,
            Description = message,
            Footer = new EmbedFooterProperties
            {
                IconUrl = avatars?.Creator,
                Text = _copyRight
            },
            Timestamp = DateTimeOffset.Now,
            Color = new Color(0xffa600),
        };
    }
}