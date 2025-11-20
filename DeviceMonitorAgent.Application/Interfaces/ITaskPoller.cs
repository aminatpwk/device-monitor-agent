using DeviceMonitorAgent.Domain.Models;

namespace DeviceMonitorAgent.Application.Interfaces;

public interface ITaskPoller
{
    Task<IEnumerable<AgentTask>> PollForTasksAsync();
    Task<AgentTask?> GetTaskAsync(Guid taskId);
}