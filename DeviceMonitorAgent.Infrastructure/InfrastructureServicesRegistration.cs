using DeviceMonitorAgent.Application.Abstractions;
using DeviceMonitorAgent.Application.Interfaces;
using DeviceMonitorAgent.Application.Services;
using DeviceMonitorAgent.Infrastructure.Data;
using DeviceMonitorAgent.Infrastructure.Providers;
using DeviceMonitorAgent.Infrastructure.Repositories;
using DeviceMonitorAgent.Infrastructure.SystemTools;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DeviceMonitorAgent.Infrastructure
{
    public static class InfrastructureServicesRegistration
    {
        public static IServiceCollection ConfigureInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ILiteDbContext>(sp =>
            {
                string connectionString = "Filename=agent_state.db; Connection=shared";
                return new LiteDbContext(connectionString);
            });
            services.AddSingleton<IAgentStateRepository, AgentStateRepository>();
            services.AddSingleton<ILogCollector, LogCollector>();
            services.AddSingleton<IAgentTaskExecutor, AgentTaskExecutor>();
            services.AddSingleton<IDeviceProvider, SystemInfoProvider>();

            return services;
        }
    }
}
