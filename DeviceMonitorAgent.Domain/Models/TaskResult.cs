using DeviceMonitorAgent.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitorAgent.Domain.Models
{
    public class TaskResult
    {
        public Guid TaskId { get; set; }
        public Status TaskStatus { get; set; }
        public string Message { get; set; } 
        public DateTime CompletedAt { get; set; }
        public Dictionary<string, string> Data {  get; set; }
    }
}
