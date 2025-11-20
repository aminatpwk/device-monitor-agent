using DeviceMonitorAgent.Domain.Models;

namespace DeviceMonitorAgent.Application.Interfaces;

/// <summary>
/// Reports device status to the platform
/// </summary>
public interface IStatusReporter
{
    Task<bool> ReportStatusAsync(DeviceStatus status);
    Task<bool> ReportBatchStatusAsync(IEnumerable<DeviceStatus> statuses);
}