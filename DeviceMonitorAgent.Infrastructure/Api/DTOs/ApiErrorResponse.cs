namespace DeviceMonitorAgent.Infrastructure.Api.DTOs;

public class ApiErrorResponse
{
    public string Error { get; set; } = string.Empty;
    public string? Details { get; set; }
    public int StatusCode { get; set; }
}