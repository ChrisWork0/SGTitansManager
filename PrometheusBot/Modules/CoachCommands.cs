using NetCord;
using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using PrometheusBot.Atrributes;
using PrometheusBot.Dtos;
using PrometheusBot.Helper;

namespace PrometheusBot.Modules;

public class CoachCommands : ApplicationCommandModule<SlashCommandContext>
{
    private readonly ulong _studentRoleId;
    private readonly ulong _creator;
    private readonly ulong _bot;
    private readonly string _title = "Student Management";
    
    public CoachCommands(IConfiguration config)
    {
        _studentRoleId = Convert.ToUInt64(config.GetSection("Roles").GetSection("Students").Value ?? "0");
        _creator =  Convert.ToUInt64(config.GetSection("Users").GetSection("AdminId").Value ?? "0"); 
        _bot = Convert.ToUInt64(config.GetSection("Users").GetSection("BotId").Value ?? "0");
    }

    [SlashCommand("add-student", "Make someone to your student.")]
    [RequireCustomPermission(nameof(Permission.Coach))]
    [RequireCustomPermission(nameof(Permission.BotOwner))]
    public async Task<InteractionCallbackProperties> AddStudent(
        [SlashCommandParameter(Description = "Select user to make him student.")]
        User selectedUser)
    {
        EmbedProperties embed = new EmbedProperties();
        GuildUser? user = null;
        string message = "";
        var guild = Context.Guild;
        while (true)
        {
            if (guild == null) { 
                message = "Something went wrong! :x:";
                break;
            }
            user = guild.Users.First(u => u.Key == selectedUser.Id).Value;
            if (user == null) {
                message = "User not found! :x:";
                break;
            }

            if (user.RoleIds.Contains(_studentRoleId)) {
                message = "User is already a student! :warning:";
                break;
            }

            if (!guild.Roles.ContainsKey(_studentRoleId)) 
                message = "Role not found! :x:";
            
            break;
        }
        
        if (!string.IsNullOrEmpty(message))
            embed = EmbedHelper.CreateEmbed(CreateContent(_title, message, [], null));
        
        else
        {
            await user.AddRoleAsync(_studentRoleId);
            message = $"<@{user.Id}> successfully added to <@&{_studentRoleId}>! :white_check_mark:";
            embed = EmbedHelper.CreateEmbed(CreateContent(_title, message, [], null));
        }
        
        return InteractionCallback.Message(new InteractionMessageProperties(){ Embeds = [embed], Flags = MessageFlags.Ephemeral});
    }

    [SlashCommand("remove-student", "Remove your student.")]
    [RequireCustomPermission(nameof(Permission.Coach))]
    [RequireCustomPermission(nameof(Permission.BotOwner))]
    public async Task<InteractionCallbackProperties> RemoveStudent(
        [SlashCommandParameter(Description = "Select user to remove him as student.")]
        User selectedUser)
    {
        EmbedProperties embed = new EmbedProperties();
        GuildUser? user = null;
        string message = "";
        var guild = Context.Guild;
        while (true)
        {
            if (guild == null) {
                message = "Something went wrong! :x:";
                break;
            }

            user = guild.Users.First(u => u.Key == selectedUser.Id).Value;
            if (user == null) {
                message = "User not found! :x:";
                break;
            }

            if (!guild.Roles.ContainsKey(_studentRoleId)) {
                message = "Role not found! :x:";
                break;
            }

            if (!user.RoleIds.Contains(_studentRoleId))
                message = "User is no student! :x:";
            
            break;
        }
        
        if (!string.IsNullOrEmpty(message))
            embed = EmbedHelper.CreateEmbed(CreateContent(_title, message, [], null));
        else
        {
            await user.RemoveRoleAsync(_studentRoleId);
            message = $"<@{user.Id}> successfully removed from <@&{_studentRoleId}>! :white_check_mark:";
            embed = EmbedHelper.CreateEmbed(CreateContent(_title, message, [], null));
        }
        return InteractionCallback.Message(new InteractionMessageProperties(){ Embeds = [embed], Flags = MessageFlags.Ephemeral});
    }

    [SlashCommand("get-students", "Get an list with all your students.")]
    [RequireCustomPermission(nameof(Permission.Coach))]
    [RequireCustomPermission(nameof(Permission.BotOwner))]
    public InteractionCallbackProperties GetStudents()
    {
        string message = "";
        var guild = Context.Guild;
        if (guild == null)
            return InteractionCallback.Message(new InteractionMessageProperties());
        List<GuildUser> allUsers = guild.Users.Values.ToList();
        var students = allUsers.Where(u => u.RoleIds.Contains(_studentRoleId)).ToList();
        message = ListAllStudents(students);
        var embed = EmbedHelper.CreateEmbed(CreateContent(null, null, [new EmbedFieldProperties{Name = "Your students:", Value = message}], GetImageUrls(guild)));
        return InteractionCallback.Message(new InteractionMessageProperties(){ Embeds = [embed], Flags = MessageFlags.Ephemeral});
    }

    private string ListAllStudents(List<GuildUser> allUsers)
    {
        string message = "";
        if (allUsers.Count == 0)
            return "No students found.";
        foreach (var user in allUsers)
        {
            message += $"- <@{user.Id}>\n";
        }
        return message;
    }

    private ImageUrls GetImageUrls(Guild guild)
        => new()
        {
            Bot = GetAvatarUrl(guild.Users.Values.First(u => u.Id == _bot)),
            Creator = GetAvatarUrl(guild.Users.Values.First(u => u.Id == _creator)),
            User = GetAvatarUrl((GuildUser)Context.User),
            Image = "https://www.gaming-grounds.de/wp-content/uploads/2019/09/league-newlogo-banner_babt.jpg"
        };
    

    public string GetAvatarUrl(GuildUser user)
    {
        var url = user.GetGuildAvatarUrl();
        if (url == null)
            url = user.GetAvatarUrl();
        if (url == null)
            return "";
        return url.ToString();
    }

    private EmbedContent CreateContent(string? title, string? message, List<EmbedFieldProperties> output, ImageUrls? imageUrls)
        => new()
        {
            AuthorName = "Prometheus",
            AuthorIcon = imageUrls?.Bot,
            Title = title ?? "List of students",
            Description = message ?? "Shows list of all students on this discord.",
            ThumbnailUrl = imageUrls?.User,
            FooterIcon = imageUrls?.Creator,
            ImageUrl = imageUrls?.Image,
            Fields = output
        };
}