using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitorAgent.Domain.Enum
{
    public enum TaskType
    {
        RestartDevice,
        RefreshStatus,
        DownloadLogs,
        ConnectRemote,
        DisconnectRemote,
        Execute,
        ConfigureAgent,
        GetInfo
    }
}
