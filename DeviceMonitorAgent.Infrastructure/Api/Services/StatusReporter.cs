using DeviceMonitorAgent.Application.Interfaces;
using DeviceMonitorAgent.Domain.Models;
using DeviceMonitorAgent.Infrastructure.Api.DTOs;
using DeviceMonitorAgent.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace DeviceMonitorAgent.Infrastructure.Api.Services;

public class StatusReporter : IStatusReporter
{
    private readonly HttpClient _httpClient;
    private readonly PlatformApiConfiguration _config;
    private readonly IAgentStateRepository _stateRepository;
    private readonly ILogger<StatusReporter> _logger;

    public StatusReporter(HttpClient httpClient, PlatformApiConfiguration config, IAgentStateRepository stateRepository, ILogger<StatusReporter> logger)
    {
        _httpClient = httpClient;
        _config = config;
        _stateRepository = stateRepository;
        _logger = logger;
    }

    public async Task<bool> ReportStatusAsync(DeviceStatus status)
    {
        var registration = _stateRepository.GetAgentRegistration();
        if(registration == null)
        {
            _logger.LogWarning("Agent is not registered. Cannot report status.");
            return false;
        }

        _logger.LogInformation("Reporting status for device: {DeviceName}", status.DeviceName);

        var request = new ReportStatusRequest
        {
            AgentId = registration.PlatformAgentId,
            DeviceIdentifier = status.DeviceIdentifier,
            DeviceName = status.DeviceName,
            DeviceType = status.DeviceType,
            IsConnected = status.IsConnected,
            CollectedAt = status.CollectedAt,
            Properties = status.Properties
        };

        try
        {
            var endpoint = _config.StatusReportEndpoint.Replace("{agentId}", registration.PlatformAgentId.ToString());
            var response = await _httpClient.PostAsJsonAsync(endpoint, request);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Status reported successfully for device: {DeviceName}", status.DeviceName);
                return true;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to report status. Status: {StatusCode}, Error: {Error}", response.StatusCode, errorContent);
                return false;
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error while reporting status");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while reporting status");
            return false;
        }
    }

    public async Task<bool> ReportBatchStatusAsync(IEnumerable<DeviceStatus> statuses)
    {
        var registration = _stateRepository.GetAgentRegistration();
        if (registration == null)
        {
            _logger.LogWarning("Cannot report batch status: Agent not registered");
            return false;
        }

        var statusList = statuses.ToList();
        var requests = statusList.Select(status => new ReportStatusRequest
        {
            AgentId = registration.PlatformAgentId,
            DeviceIdentifier = status.DeviceIdentifier,
            DeviceName = status.DeviceName,
            DeviceType = status.DeviceType,
            IsConnected = status.IsConnected,
            CollectedAt = status.CollectedAt,
            Properties = status.Properties
        }).ToList();

        try
        {
            var endpoint = "add later";
            var response = await _httpClient.PostAsJsonAsync(endpoint, requests);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to report batch status. Status: {StatusCode}, Error: {Error}", response.StatusCode, errorContent);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while reporting batch status");
            return false;
        }
    }
}