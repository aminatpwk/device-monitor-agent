using DeviceMonitorAgent.Domain.Models;

namespace DeviceMonitorAgent.Application.Interfaces;

public interface IRegistrationService
{
    Task<AgentRegistration> RegisterAgentAsync(string serialNumber);
    Task<bool> ValidateRegistrationAsync(AgentRegistration registration);
}