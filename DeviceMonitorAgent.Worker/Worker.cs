using DeviceMonitorAgent.Application.Abstractions;
using DeviceMonitorAgent.Application.Interfaces;
using DeviceMonitorAgent.Domain.Models;

namespace DeviceMonitorAgent.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IAgentStateRepository _agentStateRepository;
    private readonly IAgentTaskExecutor _agentTaskExecutor;
    private readonly IEnumerable<IDeviceProvider> _deviceProviders;

    public Worker(ILogger<Worker> logger, IAgentStateRepository agentStateRepository, IAgentTaskExecutor agentTaskExecutor, IEnumerable<IDeviceProvider> deviceProviders)
    {
        _logger = logger;
        _agentStateRepository = agentStateRepository;
        _agentTaskExecutor = agentTaskExecutor;
        _deviceProviders = deviceProviders;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        EnsureAgentRegistration(stoppingToken);
        CollectAndReportStatus();
        PollAndProcessTasks();
    }

    private void EnsureAgentRegistration(CancellationToken stoppingToken)
    {
        try
        {
            var registration = _agentStateRepository.GetAgentRegistration();
            if (registration == null)
            {
                _logger.LogInformation("Agent not registered. Performing first-time registration.");
                var newRegistration = new AgentRegistration
                {
                    PlatformAgentId = Guid.NewGuid(),
                    SerialNumber = Environment.MachineName,
                    RegistrationToken = Guid.NewGuid().ToString()
                };
                _agentStateRepository.SaveAgentRegistration(newRegistration);
            }
        }catch(Exception ex)
        {
            _logger.LogError(ex, "Error during agent registration.");
            throw;
        }
    }

    private void CollectAndReportStatus()
    {
        _logger.LogInformation("Collecting device status.");
        var configurations = _agentStateRepository.GetAllConfigurations();
        foreach(var config in configurations)
        {
            var provider = _deviceProviders.FirstOrDefault(p => p.ProviderName == config.ProviderName);
            if(provider == null)
            {
                _logger.LogWarning("No provider found for {ProviderName}", config.ProviderName);
                continue;
            }

            try
            {
                var status = provider.GetStatusAsync().GetAwaiter().GetResult();
                _logger.LogInformation("Status collected for {DeviceName}: {IsConnected}", status.DeviceName, status.IsConnected ? "Connected" : "Disconnected");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to collect status from {ProviderName}", config.ProviderName);
            }
        }
    }

    private void PollAndProcessTasks()
    {
        AgentTask task;
        while((task = _agentStateRepository.DequeueNextTask()) != null)
        {
            _logger.LogInformation("Processing task {TaskId} of type {TaskType}", task.TaskId, task.TaskType);
            try
            {
                _agentTaskExecutor.ExecuteAgentTask(task);
                _logger.LogInformation("Task {TaskId} completed", task.TaskId);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error processing task {TaskId}", task.TaskId);
            }
        }
    }
}
