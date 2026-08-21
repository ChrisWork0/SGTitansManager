using NetCord;
using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using PrometheusBot.Dtos;
using PrometheusBot.Services;

namespace PrometheusBot;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var config = builder.Configuration;
        
        var token = config["Application:BotToken"]
            ?? throw new InvalidOperationException("Bot token not configured");
        var internalApiKey = config.GetSection("Application").GetSection("ApiKey").Value
            ?? throw new InvalidOperationException("API key not configured");

        builder.Services.AddSingleton(new GatewayClient(new BotToken(token), new GatewayClientConfiguration
        {
            Intents = GatewayIntents.All | GatewayIntents.GuildUsers | GatewayIntents.GuildModeration
        }));
        builder.Services.AddSingleton(new RestClient(new BotToken(token)));
        builder.Services.AddSingleton<ApplicationCommandService<SlashCommandContext>>();
        builder.Services.AddSingleton<PendingVerificationService>();
        builder.Services.AddHostedService<BotHostedService>();
        builder.Services.AddScoped<PermissionSet>();
        builder.Services.AddScoped<VerificationService>();
        
        builder.Services.AddControllers().AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        });
        builder.Services.AddAuthentication();
        builder.Services.AddAuthorization();
        builder.Services.AddOpenApi();

        var app = builder.Build();
        app.Use(async (context, next) =>
        {
            if (context.Request.Path.StartsWithSegments("/internal"))
            {
                if (!context.Request.Headers.TryGetValue("X-Internal-Api-Key", out var providedKey) ||
                    providedKey != internalApiKey)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
            }
            await next();
        });
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}