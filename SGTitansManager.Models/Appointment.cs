namespace SGTitansManager.Models;

public class Appointment : BaseModel
{
    public AppointmentType AppointmentType { get; set; }
    public DateTime TimeFrom { get; set; }
    public DateTime? TimeTo { get; set; }
}

public enum AppointmentType
{
    Tournament,
    Scrim,
    Tryout,
    Clash,
    Meeting
}