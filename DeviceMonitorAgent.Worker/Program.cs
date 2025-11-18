using DeviceMonitorAgent.Worker;
using DeviceMonitorAgent.Infrastructure.Data;
using DeviceMonitorAgent.Infrastructure;


var builder = Host.CreateApplicationBuilder(args);
builder.Services.ConfigureInfrastructureServices(builder.Configuration);

builder.Services.AddHostedService<Worker>();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var host = builder.Build();
host.Run();
