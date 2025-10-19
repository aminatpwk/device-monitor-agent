using DeviceMonitorAgent.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitorAgent.Application.Interfaces
{
    public interface IAgentStateRepository
    {
        //registration (call home)
        AgentRegistration GetAgentRegistration();
        void SaveAgentRegistration(AgentRegistration agentRegistration);
        
        //task queue
        void EnqueueTask(AgentTask task);
        AgentTask DequeueNextTask();
        void RemoveTask(Guid taskId);

        //config
        IEnumerable<DeviceProviderConfiguration> GetAllConfigurations();
        void SaveConfiguration(DeviceProviderConfiguration config);
        void SaveAllConfigurations(IEnumerable<DeviceProviderConfiguration> configurations);
    }
}
