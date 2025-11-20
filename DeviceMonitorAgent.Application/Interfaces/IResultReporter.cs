using DeviceMonitorAgent.Domain.Models;

namespace DeviceMonitorAgent.Application.Interfaces;

public interface IResultReporter
{
    Task<bool> ReportResultAsync(TaskResult result);
    Task<bool> ReportBatchResultsAsync(IEnumerable<TaskResult> results);
}