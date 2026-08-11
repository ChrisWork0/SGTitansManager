namespace SGTitansManager.Models;

public class User : BaseModel
{
    public string UserName { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public bool LoggedIn { get; set; }
    public bool IsActive { get; set; }
    public Role Role { get; set; }
    public Guid MemberId { get; set; }
    public Member? Member { get; set; }
}

public enum Role
{
    Manager,
    Coach,
    Player,
    Caster
}