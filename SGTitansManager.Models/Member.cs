namespace SGTitansManager.Models;

public class Member : BaseModel
{
    public string DiscordName { get; set; } = "";
    public DateOnly MemberSince { get; set; }
    public Player? Player { get; set; }
    public Guid? PlayerId { get; set; }
}

