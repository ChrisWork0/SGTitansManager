namespace SGTitansManager.Models;

public class Availability : BaseModel
{
    public int Year { get; set; }
    public int CalendarWeek { get; set; }
    public string Monday { get; set; } = "";
    public string Tuesday { get; set; } = "";
    public string Wednesday { get; set; } = "";
    public string Thursday { get; set; } = "";
    public string Friday { get; set; } = "";
    public string Saturday { get; set; } = "";
    public string Sunday { get; set; } = "";
    public Guid PlayerId { get; set; }
}