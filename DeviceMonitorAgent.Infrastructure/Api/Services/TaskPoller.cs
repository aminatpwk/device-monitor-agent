using System.Net;
using System.Net.Http.Json;
using DeviceMonitorAgent.Application.Interfaces;
using DeviceMonitorAgent.Domain.Enum;
using DeviceMonitorAgent.Domain.Models;
using DeviceMonitorAgent.Infrastructure.Api.DTOs;
using DeviceMonitorAgent.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;

namespace DeviceMonitorAgent.Infrastructure.Api.Services;

public class TaskPoller : ITaskPoller
{
    private readonly HttpClient _httpClient;
    private readonly PlatformApiConfiguration _config;
    private readonly IAgentStateRepository _stateRepository;
    private readonly ILogger<TaskPoller> _logger;

    public TaskPoller(HttpClient httpClient, PlatformApiConfiguration config, IAgentStateRepository stateRepository, ILogger<TaskPoller> logger)
    {
        _httpClient = httpClient;
        _config = config;
        _stateRepository = stateRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<AgentTask>> PollForTasksAsync()
    {
        var registration = _stateRepository.GetAgentRegistration();
        if (registration == null)
        {
            _logger.LogWarning("Cannot poll tasks: Agent not registered");
            return Enumerable.Empty<AgentTask>();
        }

        try
        {
            var endpoint = _config.TaskPollEndpoint.Replace("{agentId}", registration.PlatformAgentId.ToString());
            var response = await _httpClient.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                var pollResponse = await response.Content.ReadFromJsonAsync<PollTasksResponse>();
                if (pollResponse?.Tasks != null && pollResponse.Tasks.Any())
                {
                    var tasks = pollResponse.Tasks.Select(dto => new AgentTask
                    {
                        TaskId = dto.TaskId,
                        TaskType = ParseTaskType(dto.TaskType.ToString()),
                        DeviceId = dto.DeviceId,
                        CreatedAt = dto.CreatedAt,
                        Parameters = dto.Parameters ?? new Dictionary<string, string>()
                    }).ToList();

                    return tasks;
                }
                else
                {
                    _logger.LogDebug("No new tasks available");
                    return Enumerable.Empty<AgentTask>();
                }
            } else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogDebug("No tasks found for agent");
                return Enumerable.Empty<AgentTask>();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to poll tasks. Status: {StatusCode}, Error: {Error}", response.StatusCode, errorContent);
                return Enumerable.Empty<AgentTask>();
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error while polling tasks");
            return Enumerable.Empty<AgentTask>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while polling tasks");
            return Enumerable.Empty<AgentTask>();
        }
    }

    private TaskType ParseTaskType(string taskTypeString)
    {
        if (Enum.TryParse<TaskType>(taskTypeString, true, out var taskType))
        {
            return taskType;
        }

        _logger.LogWarning("Unknown task type string: {TaskType}", taskTypeString);
        return TaskType.GetInfo;
    }

    public async Task<AgentTask?> GetTaskAsync(Guid taskId)
    {
        var registration = _stateRepository.GetAgentRegistration();
        if (registration == null)
        {
            _logger.LogWarning("Cannot get task: Agent not registered");
            return null;
        }

        try
        {
            var endpoint = "add later";
            var response = await _httpClient.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                var taskDto = await response.Content.ReadFromJsonAsync<TaskDto>();
                if (taskDto != null)
                {
                    return new AgentTask
                    {
                        TaskId = taskDto.TaskId,
                        TaskType = ParseTaskType(taskDto.TaskType),
                        DeviceId = taskDto.DeviceId,
                        CreatedAt = taskDto.CreatedAt,
                        Parameters = taskDto.Parameters ?? new Dictionary<string, string>()
                    };
                }
            } else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Task {TaskId} not found", taskId);
                return null;
            }
            else
            {
                _logger.LogError("Failed to get task {TaskId}. Status: {StatusCode}", taskId, response.StatusCode);
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting task {TaskId}", taskId);
            return null;
        }

        return null;
    }
}