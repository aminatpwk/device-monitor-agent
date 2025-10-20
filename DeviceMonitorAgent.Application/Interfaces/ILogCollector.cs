using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
