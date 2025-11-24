namespace DeviceMonitorAgent.Infrastructure.Api.DTOs;

public class ReportResultRequest
{
    public Guid TaskId { get; set; }
    public string TaskStatus { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CompletedAt { get; set; }
    public Dictionary<string, string>? Data { get; set; }
}