using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitorAgent.Domain.Models
{
    public class DeviceStatus
    {
        public Guid DeviceId { get; set; } 
        public string DeviceName { get; set; } 
        public string DeviceType { get; set; } 
        public bool IsConnected { get; set; }
        public DateTime CollectedAt { get; set; }
        public Dictionary<string, string> Properties { get; set; } 
    }
}
