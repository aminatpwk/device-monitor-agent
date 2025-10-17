using DeviceMonitorAgent.Application.Abstractions;
using DeviceMonitorAgent.Domain.Enum;
using DeviceMonitorAgent.Domain.Models;
using System.Runtime.InteropServices;

namespace DeviceMonitorAgent.Infrastructure.Providers
{
    public class SystemInfoProvider : IDeviceProvider
    {
        public string ProviderName => "SYSTEMINFO";

        public bool CanHandle(string deviceType)
        {
            return deviceType.Equals("SYSTEM", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<TaskResult> ExecuteTaskAsync(AgentTask task)
        {
            await Task.CompletedTask;
            var result = new TaskResult { 
                TaskId = task.TaskId,
                CompletedAt = DateTime.UtcNow
            };

            switch (task.TaskType)
            {
                case task.TaskType = TaskType.GetInfo:
                    var status = await GetStatusAsync();
                    result.Success = true;
                    result.Message = "System information retrieved successfully";
                    result.Data = status.Properties;
                    break;
                default:

            }
        }

        public async Task<DeviceStatus> GetStatusAsync()
        {
            await Task.CompletedTask;
            var status = new DeviceStatus
            {
                DeviceIdentifier = "local-system",
                DeviceName = Environment.MachineName,
                DeviceType = "System",
                IsConnected = true,
                CollectedAt = DateTime.UtcNow,
                Properties = new Dictionary<string, string>
                {
                    ["OS"] = RuntimeInformation.OSDescription,
                    ["OSArchitecture"] = RuntimeInformation.OSArchitecture.ToString(),
                    ["ProcessArchitecture"] = RuntimeInformation.ProcessArchitecture.ToString(),
                    ["MachineName"] = Environment.MachineName,
                    ["UserName"] = Environment.UserName,
                    ["ProcessorCount"] = Environment.ProcessorCount.ToString(),
                    ["SystemDirectory"] = Environment.SystemDirectory,
                    ["AvailableMemoryGB"] = GetAvailableMemoryGB().ToString("F2")
                }
            };

            return status;
        }

        private double GetAvailableMemoryGB()
        {
            try
            {
                var gcMemoryInfo = GC.GetGCMemoryInfo();
                return gcMemoryInfo.TotalAvailableMemoryBytes / (1024.0 * 1024.0 * 1024.0);
            }
            catch
            {
                return 0;
            }
        }
    }
}
