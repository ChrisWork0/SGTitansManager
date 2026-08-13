namespace SGTitansManager.Server;

public class ResultDto
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public object? Model { get; set; }
    public int? StatusCode { get; set; }
}