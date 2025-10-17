using DeviceMonitorAgent.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitorAgent.Application.Abstractions
{
    public interface IDeviceProvider
    {
        string ProviderName { get; }

        /// <summary>
        /// Gets the current status of the device
        /// </summary>
        Task<DeviceStatus> GetStatusAsync();

        /// <summary>
        /// Executes a task on the device
        /// </summary>
        Task<TaskResult> ExecuteTaskAsync(AgentTask task);

        /// <summary>
        /// Checks if this provider can handle the given device type
        /// </summary>
        bool CanHandle(string deviceType);
    }
}
