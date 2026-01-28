using DeviceMonitorAgent.Application.Interfaces;
using System.Runtime.InteropServices;

namespace DeviceMonitorAgent.Infrastructure.SystemTools
{
    public class LogCollector : ILogCollector
    {
        public string CollectAndZipLogs()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.WriteLine("Windows Log Collection: Simulating Event Source retrieval and zipping.");
                return "C:\\Temp\\agent_logs_windows.zip";
            }else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Console.WriteLine("Linux Log Collection: Simulating SystemD log retrieval and zipping.");
                return "/var/log/agent_logs_linux.zip";
            }
            else
            {
                Console.WriteLine($"Log collection not implemented for OS: {RuntimeInformation.OSDescription}");
                return string.Empty;
            }
        }
    }
}
