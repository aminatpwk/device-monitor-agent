using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitorAgent.Infrastructure.Data
{
    public interface ILiteDbContext : IDisposable
    {
        //the actual embedded database engine. In short, it exposes the active database connection to LiteDB;
        LiteDatabase Database { get; }
        ILiteCollection<T> GetCollection<T>(string name);
    }
}
