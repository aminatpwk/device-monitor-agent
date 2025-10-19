using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitorAgent.Domain.Models
{
    public class DeviceProviderConfiguration
    {
        public Guid DeviceId { get; set; }
        public string ProviderName { get; set; }
        public Dictionary<string, string> ConnectionSettings { get; set; }
    }
}
