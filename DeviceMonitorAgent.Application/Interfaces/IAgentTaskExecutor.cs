using DeviceMonitorAgent.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitorAgent.Application.Interfaces
{
    public interface IAgentTaskExecutor
    {
        TaskResult ExecuteAgentTask (AgentTask task);
    }
}
