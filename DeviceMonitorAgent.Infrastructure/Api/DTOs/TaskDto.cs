namespace DeviceMonitorAgent.Infrastructure.Api.DTOs;

public class TaskDto
{
    public Guid TaskId { get; set; }
    public string TaskType { get; set; } = string.Empty;
    public Guid DeviceId { get; set; }
    public DateTime CreatedAt { get; set; }
    public Dictionary<string, string> Parameters { get; set; } = new();
}