namespace DeviceMonitorAgent.Infrastructure.Api.DTOs
{
    public class RegisterAgentResponse
    {
        public Guid AgentCode { get; set; }
        public Guid DeviceCode { get; set; }
        public string RegistrationToken { get; set; } = string.Empty;
    }
}
