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

        private async Task<TaskResult> ExecuteDeviceTask(AgentTask task)
        {
            var config = _agentStateRepository.GetAllConfigurations().FirstOrDefault(c => c.DeviceId == task.DeviceId);
            if (config == null)
            {
                return CreateErroResult(task, $"Device configuration not found for DeviceId: {task.DeviceId}");
            }

            _logger.LogDebug("Found configuration for device {DeviceId}, provider: {ProviderName}", task.DeviceId, config.ProviderName);

            var provider = _deviceProviders.FirstOrDefault(p => p.ProviderName == config.ProviderName);
            if (provider == null)
            {
                return CreateErroResult(task, $"Provider '{config.ProviderName}' not found or not registered");
            }

            _logger.LogDebug("Using provider {ProviderName} for task {TaskId}",
                provider.ProviderName, task.TaskId);

            try
            {
                var result = await provider.ExecuteTaskAsync(task);
                _logger.LogInformation("Device task {TaskId} completed with status {Status}", task.TaskId, result.TaskStatus);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Provider {ProviderName} failed to execute task {TaskId}", config.ProviderName, task.TaskId);
                return CreateErroResult(task, $"Provider execution failed: {ex.Message}");
            }
        }

        private void ExecuteAgentTask(AgentTask task)
        {
            _agentTaskExecutor.ExecuteAgentTask(task);
        }

        private TaskResult CreateErroResult(AgentTask task, string errorMessage)
        {
            return new TaskResult
            {
                TaskId = task.TaskId,
                TaskStatus = Status.Critical,
                Message = errorMessage,
                CompletedAt = DateTime.UtcNow,
                Data = new Dictionary<string, string>()
            };
        }
    }
}
