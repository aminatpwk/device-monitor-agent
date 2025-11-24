using DeviceMonitorAgent.Application.Abstractions;
using DeviceMonitorAgent.Application.Interfaces;
using DeviceMonitorAgent.Domain.Models;

namespace DeviceMonitorAgent.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IAgentStateRepository _agentStateRepository;
    private readonly IEnumerable<IDeviceProvider> _deviceProviders;
    public readonly ITaskRouter _taskRouter;
    public readonly IRegistrationService _RegistrationService;
    private readonly IStatusReporter _statusReporter;
    private readonly ITaskPoller _taskPoller;
    private readonly IResultReporter _resultReporter;
    private readonly TimeSpan _statusCollectionInterval = TimeSpan.FromMinutes(5);
    private readonly TimeSpan _taskPollingInterval = TimeSpan.FromMinutes(1);

    public Worker(ILogger<Worker> logger, IAgentStateRepository agentStateRepository, IEnumerable<IDeviceProvider> deviceProviders, ITaskRouter taskRouter,
        IRegistrationService registrationService, IStatusReporter statusReporter, ITaskPoller taskPoller, IResultReporter resultReporter, TimeSpan statusCollectionInterval, TimeSpan taskPollingInterval)
    {
        _logger = logger;
        _agentStateRepository = agentStateRepository;
        _deviceProviders = deviceProviders;
        _taskRouter = taskRouter;
        _RegistrationService = registrationService;
        _statusReporter = statusReporter;
        _taskPoller = taskPoller;
        _resultReporter = resultReporter;
        _statusCollectionInterval = statusCollectionInterval;
        _taskPollingInterval = taskPollingInterval;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        EnsureAgentRegistration(stoppingToken);
        CollectAndReportStatus();
        PollAndProcessTasks();
        await Task.WhenAll();
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
        var statuses = new List<DeviceStatus>();

        foreach (var config in configurations)
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
                statuses.Add(status);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to collect status from {ProviderName}", config.ProviderName);
            }
        }

        if (statuses.Any())
        {
            try
            {
                var success = _statusReporter.ReportBatchStatusAsync(statuses).GetAwaiter().GetResult();
                if (success)
                {
                    _logger.LogInformation("Successfully reported {Count} device statuses to platform", statuses.Count);
                }
                else
                {
                    _logger.LogWarning("Failed to report device statuses to platform");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reporting statuses to platform");
            }
        }
    }

    private void PollAndProcessTasks()
    {
        try
        {
            var newTasks = _taskPoller.PollForTasksAsync().GetAwaiter().GetResult();
            foreach (var agentTask in newTasks)
            {
                _agentStateRepository.EnqueueTask(agentTask);
                _logger.LogInformation("Enqueued new task {TaskId} from platform", agentTask.TaskId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error polling tasks from platform");
        }

        AgentTask task;
        while((task = _agentStateRepository.DequeueNextTask()) != null)
        {
            _logger.LogInformation("Processing task {TaskId} of type {TaskType}", task.TaskId, task.TaskType);
            try
            {
                _taskRouter.RouteAndExecute(task);
                _logger.LogInformation("Task {TaskId} completed", task.TaskId);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error processing task {TaskId}", task.TaskId);
            }
        }
    }
}
