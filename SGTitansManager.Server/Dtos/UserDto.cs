using SGTitansManager.Models;

namespace SGTitansManager.Server;

public class UserDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = "";
    public bool IsActive { get; set; }
    public Role Role { get; set; }
    public Member? Member { get; set; }
}