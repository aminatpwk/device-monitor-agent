using DeviceMonitorAgent.Domain.Models;

namespace DeviceMonitorAgent.Application.Interfaces
{
    public interface ITaskRouter
    {
        void RouteAndExecute(AgentTask task);
        bool IsAgentTask(AgentTask task);
        bool IsDeviceTask(AgentTask task);
    }
}
