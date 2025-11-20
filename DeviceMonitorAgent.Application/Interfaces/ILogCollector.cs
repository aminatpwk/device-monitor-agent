namespace DeviceMonitorAgent.Application.Interfaces
{
    public interface ILogCollector
    {
        /// <summary>
        /// collects and zips the logs, returning the path to the zipped file
        /// </summary>
        /// <returns></returns>
        string CollectAndZipLogs();
    }
}
