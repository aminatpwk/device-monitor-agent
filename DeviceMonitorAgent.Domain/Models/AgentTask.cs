using DeviceMonitorAgent.Domain.Enum;
using LiteDB;

namespace DeviceMonitorAgent.Domain.Models
{
    public class AgentTask
    {
        [BsonId]
        public Guid TaskId { get; set; } 
        public TaskType TaskType { get; set; }
        public Guid DeviceId { get; set; } 
        public TargetType TargetType { get; set; }
        public DateTime CreatedAt { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
    }
}
