using NetCord.Services.ApplicationCommands;
using PrometheusBot.Atrributes;
using PrometheusBot.Dtos;

namespace PrometheusBot.Modules;

public class SimpleCommands : ApplicationCommandModule<SlashCommandContext>
{
    [SlashCommand("ping", "Antwort mit Pong zurück.")]
    [RequireCustomPermission(nameof(Permission.BotOwner))]
    public string Ping()
    {
        return "Pong! 🏓";
    }

    [SlashCommand("echo", "Gibt die Nachricht einfach wieder.")]
    public string Echo(
        [SlashCommandParameter(Description = "Deine Nachricht, die wiedergegeben werden soll")]
        string message)
    {
        return message;
    }
}