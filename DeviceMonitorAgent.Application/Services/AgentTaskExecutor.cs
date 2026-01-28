using DeviceMonitorAgent.Application.Interfaces;
using DeviceMonitorAgent.Domain.Enum;
using DeviceMonitorAgent.Domain.Models;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DeviceMonitorAgent.Application.Services
{
    public class AgentTaskExecutor : IAgentTaskExecutor
    {
        private readonly ILogger<AgentTaskExecutor> _logger;
        private readonly ILogCollector _logCollector;
        public AgentTaskExecutor(ILogger<AgentTaskExecutor> logger, ILogCollector logCollector)
        {
            _logger = logger;
            _logCollector = logCollector;
        }


        public TaskResult ExecuteAgentTask(AgentTask task)
        {
            _logger.LogInformation($"Executing agent task: {task.TaskType} (Task ID: {task.TaskId})");
            try
            {
                return task.TaskType switch
                {
                    TaskType.RestartAgent => RestartAgent(task),
                    TaskType.DownloadAgentLogs => DownloadAgentLogs(task),
                    _ => new TaskResult
                    {
                        TaskId = task.TaskId,
                        TaskStatus = Status.NonUrgent,
                        Message = $"Task type {task.TaskType} is not supported by AgentTaskExecutor.",
                        CompletedAt = DateTime.UtcNow
                    },
                };
            }
            catch (Exception ex)
            {
                return new TaskResult
                {
                    TaskId = task.TaskId,
                    TaskStatus = Status.Critical,
                    Message = $"Error executing task: {ex.Message}",
                    CompletedAt = DateTime.UtcNow
                };
            }
        }

        private TaskResult RestartAgent(AgentTask task)
        {
            string command = "";
            string arguments = "";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                command = "shutdown";
                arguments = "/r /t 0";
            }else if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux)){
                command = "sudo";
                arguments = "reboot"; //ose 'reboot now' 
            }

            if (string.IsNullOrEmpty(command))
            {
                return new TaskResult
                {
                    TaskId = task.TaskId,
                    TaskStatus = Status.Critical,
                    Message = $"RestartAgent is not supported on {RuntimeInformation.OSDescription}",
                    CompletedAt = DateTime.UtcNow,
                };
            }

            try
            {
                Process.Start(command, arguments);

                return new TaskResult
                {
                    TaskId = task.TaskId,
                    TaskStatus = Status.Good,
                    Message = $"System restart command initiated: {command} {arguments}",
                    CompletedAt = DateTime.UtcNow
                };
            }catch(Exception ex)
            {
                return new TaskResult
                {
                    TaskId = task.TaskId,
                    TaskStatus = Status.Critical,
                    Message = $"Failed to initiate system restart: {ex.Message}",
                    CompletedAt = DateTime.UtcNow
                };
            }
        }

        private TaskResult DownloadAgentLogs(AgentTask task)
        {
            string zippedLogsPath = _logCollector.CollectAndZipLogs();
            if (string.IsNullOrEmpty(zippedLogsPath))
            {
                return new TaskResult
                {
                    TaskId = task.TaskId,
                    TaskStatus = Status.Critical,
                    Message = "Failed to collect and zip logs.",
                    CompletedAt = DateTime.UtcNow
                };
            }

            //TODO: add logic to upload the zipped logs to a server or cloud storage and get a download link.

            return new TaskResult
            {
                TaskId = task.TaskId,
                TaskStatus = Status.Good,
                Message = $"Logs collected and zipped successfully on {zippedLogsPath}.",
                //add the expected download link in the Data dictionary
                CompletedAt = DateTime.UtcNow
            };
        }
    }
}
