using DeviceMonitorAgent.Domain.Models;

namespace DeviceMonitorAgent.Infrastructure.Api.DTOs;

public class PollTasksResponse
{
    public List<AgentTask> Tasks { get; set; } = new();
    public int TotalCount { get; set; }
}