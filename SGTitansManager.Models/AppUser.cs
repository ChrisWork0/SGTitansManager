namespace SGTitansManager.Models;

public class AppUser : BaseModel
{
    public string UserName { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public bool IsActive { get; set; }
    public Role Role { get; set; }
    public Guid MemberId { get; set; }
    public Member? Member { get; set; }
    public string? RecoveryCode { get; set; }
}

public enum Role
{
    Admin,
    Manager,
    Coach,
    CorePlayer,
    Player,
    Caster
}