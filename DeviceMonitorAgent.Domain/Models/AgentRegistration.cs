using LiteDB;

namespace DeviceMonitorAgent.Domain.Models
{
    public class AgentRegistration
    {
        public string SerialNumber { get; set; }
        [BsonId]
        public Guid PlatformAgentId { get; set; }
        public string RegistrationToken { get; set; }
    }
}
