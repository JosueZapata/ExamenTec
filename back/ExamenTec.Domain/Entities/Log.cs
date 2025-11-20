namespace ExamenTec.Domain.Entities;

public class Log
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Level { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Exception { get; set; }
    public string? Source { get; set; }
    public string? Method { get; set; }
    public string? User { get; set; }
    public string? RequestPath { get; set; }
    public int? StatusCode { get; set; }
}

