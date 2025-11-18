using DeviceMonitorAgent.Application.Abstractions;
using DeviceMonitorAgent.Application.Interfaces;
using DeviceMonitorAgent.Domain.Enum;
using DeviceMonitorAgent.Domain.Models;
using Microsoft.Extensions.Logging;

namespace DeviceMonitorAgent.Application.Services
{
    public class TaskRouter : ITaskRouter
    {
        private readonly ILogger<TaskRouter> _logger;
        private readonly IAgentTaskExecutor _agentTaskExecutor;
        private readonly IAgentStateRepository _agentStateRepository;
        private readonly IEnumerable<IDeviceProvider> _deviceProviders;

        public TaskRouter(ILogger<TaskRouter> logger, IAgentTaskExecutor agentTaskExecutor, IAgentStateRepository agentStateRepository, IEnumerable<IDeviceProvider> deviceProviders)
        {
            _logger = logger;
            _agentTaskExecutor = agentTaskExecutor;
            _agentStateRepository = agentStateRepository;
            _deviceProviders = deviceProviders;
        }

        public bool IsAgentTask(AgentTask task)
        {
            return task.TaskType switch
            {
                TaskType.RestartAgent => true,
                TaskType.DownloadAgentLogs => true,
                TaskType.ConfigureAgent => true,
                _ => false
            };
        }

        public bool IsDeviceTask(AgentTask task)
        {
            return task.TaskType switch
            {
                TaskType.RestartDevice => true,
                TaskType.RefreshStatus => true,
                TaskType.DownloadLogs => true,
                TaskType.ConnectRemote => true,
                TaskType.DisconnectRemote => true,
                TaskType.Execute => true,
                TaskType.GetInfo => true,
                _ => false
            };

        }

        public void RouteAndExecute(AgentTask task)
        {
            _logger.LogInformation("Routing task {TaskId} of type {TaskType}", task.TaskId, task.TaskType);
            if (IsAgentTask(task))
            {
                ExecuteAgentTask(task);
            }
            else if(IsDeviceTask(task))
            {
                ExecuteDeviceTask(task);
            }
            else
            {
                _logger.LogWarning("Unknown task type {TaskType} for task {TaskId}", task.TaskType, task.TaskId);
            }
        }

        private void ExecuteDeviceTask(AgentTask task)
        {
            var config = _agentStateRepository.GetAllConfigurations().FirstOrDefault(c => c.DeviceId == task.DeviceId);

        }

        private void ExecuteAgentTask(AgentTask task)
        {
            _agentTaskExecutor.ExecuteAgentTask(task);
        }
    }
}
