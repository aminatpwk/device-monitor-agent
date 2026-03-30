using LiteDB;

namespace DeviceMonitorAgent.Domain.Models
{
    public class AgentRegistration
    {
        public Guid DeviceCode { get; set; }       // identifies the physical device;
        public string SerialNumber { get; set; }    // may be the same for both;
        [BsonId]
        public Guid AgentCode { get; set; } // identifies the agent service;
        public string RegistrationToken { get; set; }
    }
}
