namespace SGTitansManager.Models;

public class BaseModel
{
    public Guid Id { get; set; }
    public DateTime Created { get; set; } = DateTime.Now.ToUniversalTime();
    public DateTime? Deleted {get; set;}
}