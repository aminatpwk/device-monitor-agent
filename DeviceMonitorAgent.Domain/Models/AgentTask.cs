using DeviceMonitorAgent.Domain.Enum;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitorAgent.Domain.Models
{
    public class AgentTask
    {
        [BsonId]
        public Guid TaskId { get; set; } 
        public TaskType TaskType { get; set; }
        public Guid DeviceId { get; set; } 
        public DateTime CreatedAt { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
    }
}
