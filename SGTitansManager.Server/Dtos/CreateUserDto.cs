using SGTitansManager.Models;

namespace SGTitansManager.Server;

public class CreateUserDto
{
    public string UserName { get; set; } = "";
    public string Password { get; set; } = "";
    public Role Role { get; set; }
    public string DiscordId { get; set; } = "";
    public DateOnly MemberSince { get; set; }
}