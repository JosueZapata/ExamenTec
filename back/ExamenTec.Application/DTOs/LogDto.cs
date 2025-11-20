namespace ExamenTec.Application.DTOs;

public class LogDto
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string Level { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Exception { get; set; }
    public string? Source { get; set; }
    public string? Method { get; set; }
    public string? User { get; set; }
    public string? RequestPath { get; set; }
    public int? StatusCode { get; set; }
}
