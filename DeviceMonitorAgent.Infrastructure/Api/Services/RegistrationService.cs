using System.Net.Http.Json;
using DeviceMonitorAgent.Application.Interfaces;
using DeviceMonitorAgent.Domain.Models;
using DeviceMonitorAgent.Infrastructure.Api.DTOs;
using DeviceMonitorAgent.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;

namespace DeviceMonitorAgent.Infrastructure.Api.Services;

public class RegistrationService : IRegistrationService
{
    private readonly HttpClient _httpClient;
    private readonly PlatformApiConfiguration _apiConfig;
    private readonly ILogger<RegistrationService> _logger;

    public RegistrationService(HttpClient httpClient, PlatformApiConfiguration apiConfig, ILogger<RegistrationService> logger)
    {
        _httpClient = httpClient;
        _apiConfig = apiConfig;
        _logger = logger;
    }

    public async Task<AgentRegistration> RegisterAgentAsync(string serialNumber)
    {
        _logger.LogInformation("Attempting to register agent with serial number: {SerialNumber}", serialNumber);

        var request = new RegisterAgentRequest
        {
            SerialNumber = serialNumber,
            MachineName = Environment.MachineName,
            RequestedAt = DateTime.UtcNow
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync(_apiConfig.RegistrationEndpoint, request);
            if (response.IsSuccessStatusCode)
            {
                var registerResponse = await response.Content.ReadFromJsonAsync<AgentRegistration>();

                _logger.LogInformation("Agent registered successfully. AgentId: {AgentId}", registerResponse.PlatformAgentId);
                return new AgentRegistration
                {
                    PlatformAgentId = registerResponse.PlatformAgentId,
                    SerialNumber = serialNumber,
                    RegistrationToken = registerResponse.RegistrationToken
                };
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to register agent. Status Code: {StatusCode}, Response: {Response}", response.StatusCode, errorContent);
                throw new HttpRequestException($"Registration failed with status {response.StatusCode}");
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error during registration");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during registration");
            throw;
        }
    }

    public async Task<bool> ValidateRegistrationAsync(AgentRegistration registration)
    {
        try
        {
            var endpoint = "link here";
            var response = await _httpClient.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }else if (response.StatusCode == System.Net.HttpStatusCode.NotFound || response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _logger.LogWarning("Registration is invalid for AgentId: {AgentId}", registration.PlatformAgentId);
                return false;
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating registration.");
            return false;
        }
    }
}