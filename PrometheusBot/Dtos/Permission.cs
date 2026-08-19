namespace PrometheusBot.Dtos;

public class PermissionSet(IConfiguration config)
{
    public ulong AdminId => Convert.ToUInt64(config.GetSection("Users").GetSection("AdminId").Value ?? "0");
    public ulong CoachId => Convert.ToUInt64(config.GetSection("Users").GetSection("CoachId").Value ?? "0");
}

public enum Permission
{
    BotOwner,
    Coach
}