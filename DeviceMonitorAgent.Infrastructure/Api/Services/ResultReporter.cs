using DeviceMonitorAgent.Application.Interfaces;
using DeviceMonitorAgent.Domain.Models;
using DeviceMonitorAgent.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;

namespace DeviceMonitorAgent.Infrastructure.Api.Services;

public class ResultReporter : IResultReporter
{
    private readonly HttpClient _httpClient;
    private readonly PlatformApiConfiguration _config;
    private readonly IAgentStateRepository _stateRepository;
    private readonly ILogger<ResultReporter> _logger;

    public ResultReporter(HttpClient httpClient, PlatformApiConfiguration config, IAgentStateRepository stateRepository, ILogger<ResultReporter> logger)
    {
        _httpClient = httpClient;
        _config = config;
        _stateRepository = stateRepository;
        _logger = logger;
    }


    public Task<bool> ReportBatchResultsAsync(IEnumerable<TaskResult> results)
    {
        //TODO: implement logic here
    }

    public Task<bool> ReportResultAsync(TaskResult result)
    {
        //TODO: implement logic here
    }
}