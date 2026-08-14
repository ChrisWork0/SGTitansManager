namespace SGTitansManager.Models;

public class Member
{
    public Guid Id { get; set; }
    public string DiscordName { get; set; } = "";
    public DateOnly MemberSince { get; set; }
    public Player? Player { get; set; }
    public Guid? PlayerId { get; set; }
}

