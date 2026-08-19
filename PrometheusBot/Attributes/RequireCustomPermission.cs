using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;
using PrometheusBot.Dtos;

namespace PrometheusBot.Atrributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class RequireCustomPermission : ParameterPreconditionAttribute<SlashCommandContext>
{
    private readonly string _requiredPermission;

    public RequireCustomPermission(string requiredPermission)
    {
        _requiredPermission = requiredPermission;
    }
    
    public override ValueTask<PreconditionResult> EnsureCanExecuteAsync(object? value, SlashCommandContext context, 
        IServiceProvider? serviceProvider)
    {
        if (context.User is not NetCord.GuildUser guildUser)
            return ValueTask.FromResult<PreconditionResult>(PreconditionResult.Fail("Command only on server usable."));

        bool hasPermission = CheckUserPermission(guildUser, _requiredPermission, serviceProvider);

        if (hasPermission)
            return ValueTask.FromResult<PreconditionResult>(PreconditionResult.Success);
        
        return ValueTask.FromResult<PreconditionResult>(PreconditionResult.Fail($"Missing permission: `{_requiredPermission}`"));
    }

    private bool CheckUserPermission(NetCord.GuildUser user, string permissionName, IServiceProvider? serviceProvider)
    {
        if (serviceProvider == null)
            return false;
        var permissionSet = serviceProvider.GetRequiredService<PermissionSet>();
        if (permissionName == nameof(Permission.BotOwner) && user.Id == permissionSet.AdminId)
            return true;
        if (permissionName == nameof(Permission.Coach) && user.Id == permissionSet.CoachId)
            return true;

        return false;
    }       
}