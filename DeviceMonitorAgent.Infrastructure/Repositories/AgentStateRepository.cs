using DeviceMonitorAgent.Application.Interfaces;
using DeviceMonitorAgent.Domain.Models;
using DeviceMonitorAgent.Infrastructure.Data;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeviceMonitorAgent.Infrastructure.Repositories
{
    public class AgentStateRepository : IAgentStateRepository
    {
        private readonly ILiteCollection<AgentTask> _taskCollection;
        private readonly ILiteCollection<AgentRegistration> _registrationCollection;
        private readonly ILiteCollection<DeviceProviderConfiguration> _configCollection;

        public AgentStateRepository(ILiteDbContext context)
        {
            _taskCollection = context.GetCollection<AgentTask>(nameof(AgentTask));
            _registrationCollection = context.GetCollection<AgentRegistration>(nameof(AgentRegistration));
            _configCollection = context.GetCollection<DeviceProviderConfiguration>(nameof(DeviceProviderConfiguration));
            _taskCollection.EnsureIndex(x => x.CreatedAt); 
        }

        public AgentRegistration GetAgentRegistration()
        {
            return _registrationCollection.FindAll().FirstOrDefault();
        }

        public void SaveAgentRegistration(AgentRegistration registration)
        {
            _registrationCollection.Upsert(registration);
        }

        public void EnqueueTask(AgentTask task)
        {
            _taskCollection.Insert(task);
        }

        public AgentTask DequeueNextTask()
        {
            var nextTask = _taskCollection.Find(Query.All(nameof(AgentTask.CreatedAt), Query.Ascending)).FirstOrDefault();

            if (nextTask != null)
            {
                _taskCollection.Delete(nextTask.TaskId);
            }
            return nextTask;
        }

        public void RemoveTask(Guid taskId)
        {
            _taskCollection.Delete(taskId);
        }

        public IEnumerable<DeviceProviderConfiguration> GetAllConfigurations()
        {
            return _configCollection.FindAll();
        }

        public void SaveConfiguration(DeviceProviderConfiguration config)
        {
            _configCollection.Upsert(config);
        }

        public void SaveAllConfigurations(IEnumerable<DeviceProviderConfiguration> configurations)
        {
            _configCollection.DeleteAll();
            _configCollection.InsertBulk(configurations);
        }
    }
}
