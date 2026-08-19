using System.Reflection;
using NetCord;
using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace PrometheusBot.Services;

/// <summary>
/// Starts/stops the GatewayClient in sync with the host lifecycle,
/// registers slash commands and handles button interactions
/// for the password verification flow.
/// </summary>
public class BotHostedService : IHostedService
{
    private readonly GatewayClient _client;
    private readonly ApplicationCommandService<SlashCommandContext> _commandService;
    private readonly PendingVerificationService _pendingVerificationService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BotHostedService> _logger;

    public BotHostedService(GatewayClient client,
        ApplicationCommandService<SlashCommandContext> commandService,
        PendingVerificationService pendingVerificationService,
        IServiceProvider serviceProvider, ILogger<BotHostedService> logger)
    {
        _client = client;
        _commandService = commandService;
        _pendingVerificationService = pendingVerificationService;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _commandService.AddModules(Assembly.GetExecutingAssembly());
        _client.Ready += ReadyEventHandler;
        _client.InteractionCreate += InteractionCreateEventHandler;
        _client.Disconnect += DisconnectEventHandler;

        await _client.StartAsync(cancellationToken: cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _client.CloseAsync(cancellationToken: cancellationToken);
    }

    private ValueTask ReadyEventHandler(ReadyEventArgs args)
    {
        _logger.LogInformation("Successfully logged in as {0}", args.User.Username);
        _ = _commandService.RegisterCommandsAsync(_client.Rest, args.User.Id);
        return ValueTask.CompletedTask;
    }

    private async ValueTask InteractionCreateEventHandler(Interaction interaction)
    {
        switch (interaction)
        {
            case SlashCommandInteraction slashCommandInteraction:
                await HandleSlashCommandAsync(slashCommandInteraction);
                break;
            case ButtonInteraction buttonInteraction:
                await HandleVerificationButtonAsync(buttonInteraction);
                break;
        }
    }

    private async Task HandleSlashCommandAsync(SlashCommandInteraction slashCommand)
    {
        using var scope = _serviceProvider.CreateScope();
        var result = await _commandService.ExecuteAsync(
            new SlashCommandContext(slashCommand, _client),
            scope.ServiceProvider);

        switch (result)
        {
            case NetCord.Services.IExceptionResult exceptionResult:
                _logger.LogError(exceptionResult.Exception, "Error during executing slash commands.");
                await slashCommand.SendResponseAsync(
                    InteractionCallback.Message(new InteractionMessageProperties
                    {
                        Content = "An error occurred whilst executing the command."
                    }));
                break;
            
            case NetCord.Services.NotFoundResult:
                _logger.LogWarning("Unknown slash command has been called.");
                break;
        }
    }

    private async Task HandleVerificationButtonAsync(ButtonInteraction button)
    {
        var parts = button.Data.CustomId.Split(':');
        if (parts.Length != 2 || parts[0] is not ("verify_confirm" or "verify_deny"))
            return;
        
        if (!Guid.TryParse(parts[1], out var tokenId))
            return;
        
        var confirmed = parts[0] ==  "verify_confirm";
        var resolved = _pendingVerificationService.TryResolve(tokenId, confirmed);

        var message = !resolved
            ? "⚠️ Request already expired."
            : confirmed
                ? "✅ Confirmed. New password set."
                : "❌ Denied.";
        
        await button.SendResponseAsync(InteractionCallback.ModifyMessage(msg =>
        {
            msg.Content = message;
            msg.Components = [];
        }));
    }

    private ValueTask DisconnectEventHandler(DisconnectEventArgs args)
    {
        _logger.LogWarning("Bot disconnected. Automatic Reconnect: {0}", args.Reconnect);
        return ValueTask.CompletedTask;
    }
}