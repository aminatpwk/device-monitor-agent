using DeviceMonitorAgent.Domain.Models;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitorAgent.Infrastructure.Data
{
    public class LiteDbContext : ILiteDbContext
    {
        private readonly LiteDatabase _database;

        public LiteDbContext(string connectionString)
        {
            _database = new LiteDatabase(connectionString);

            //_database.GetCollection<AgentTask>("AgentTask").EnsureIndex(x => x.CreatedAt);
            //_database.GetCollection<AgentRegistration>("AgentRegistration").EnsureIndex(x => x.RegistrationToken);

        }

        public LiteDatabase Database => _database;
        public ILiteCollection<T> GetCollection<T>(string name) => _database.GetCollection<T>(name);

        public void Dispose()
        {
            _database.Dispose();
        }
    }
}
