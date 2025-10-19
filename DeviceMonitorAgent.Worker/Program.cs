using DeviceMonitorAgent.Worker;
using DeviceMonitorAgent.Infrastructure.Data;


var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddSingleton<ILiteDbContext>(sp =>
{
    string connectionString = "Filename=agent_state.db; Connection=shared";
    return new LiteDbContext(connectionString);
});

var host = builder.Build();
host.Run();
