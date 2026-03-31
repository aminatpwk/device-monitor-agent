namespace DeviceMonitorAgent.Installer.CA.DTOs;

public class RegisterDeviceResponse
{
    public Guid DeviceCode { get; set; }
    public Guid AgentCode { get; set; }
    public string RegistrationToken { get; set; } = string.Empty;
}