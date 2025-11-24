namespace DeviceMonitorAgent.Infrastructure.Api.DTOs
{
    public class RegisterAgentRequest
    {
        public string SerialNumber { get; set; }
        public string MachineName { get; set; }
        public DateTime RequestedAt { get; set; }
    }
}