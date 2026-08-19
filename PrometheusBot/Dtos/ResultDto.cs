namespace PrometheusBot.Dtos;

public class ResultDto
{
    public bool Success { get; set; } = false;
    public string? Message { get; set; }
    public object? Data { get; set; }
}