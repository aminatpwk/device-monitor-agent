namespace DeviceMonitorAgent.Infrastructure.Api.DTOs;

public class ReportStatusRequest
{
    public Guid AgentId { get; set; }
    public string DeviceIdentifier { get; set; } = string.Empty;
    public string DeviceName { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty;
    public bool IsConnected { get; set; }
    public DateTime CollectedAt { get; set; }
    public Dictionary<string, string> Properties { get; set; } = new();
}