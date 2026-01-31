using DeviceMonitorAgent.Application.Interfaces;
using DeviceMonitorAgent.Application.Services;
using DeviceMonitorAgent.Domain.Enum;
using DeviceMonitorAgent.Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentAssertions;

namespace Test
{
    public class AgentTaskExecutorTests
    {
        private readonly Mock<ILogger<AgentTaskExecutor>> _loggerMock;
        private readonly Mock<ILogCollector> _logCollectorMock;
        private readonly AgentTaskExecutor _agentTaskExecutor;

        public AgentTaskExecutorTests()
        {
            _loggerMock = new Mock<ILogger<AgentTaskExecutor>>();
            _logCollectorMock = new Mock<ILogCollector>();
            _agentTaskExecutor = new AgentTaskExecutor(_loggerMock.Object, _logCollectorMock.Object);
        }

        [Fact]
        public void ExecuteAgentTask_WithRestartAgentTask_ReturnsSuccessResult() //kujdes se te fik laptopin!
        {
            var task = new AgentTask
            {
                TaskId = Guid.NewGuid(),
                TaskType = TaskType.RestartAgent,
                CreatedAt = DateTime.UtcNow,
                Parameters = new Dictionary<string, string>()
            };

            var result = _agentTaskExecutor.ExecuteAgentTask(task);
            result.Should().NotBeNull();
            result.TaskId.Should().Be(task.TaskId);
            result.TaskStatus.Should().Be(Status.Good);
            result.Message.Should().Contain("System restart command initiated");
            result.CompletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void ExecuteAgentTask_WithDownloadAgentLogsTask_WhenLogCollectionSucceeds_ReturnsSuccessResult()
        {
            var expectedZipPath = "C:\\logs\\test.zip";
            _logCollectorMock.Setup(x => x.CollectAndZipLogs()).Returns(expectedZipPath);

            var task = new AgentTask
            {
                TaskId = Guid.NewGuid(),
                TaskType = TaskType.DownloadAgentLogs,
                CreatedAt = DateTime.UtcNow,
                Parameters = new Dictionary<string, string>()
            };

            var result = _agentTaskExecutor.ExecuteAgentTask(task);
            result.Should().NotBeNull();
            result.TaskId.Should().Be(task.TaskId);
            result.TaskStatus.Should().Be(Status.Good);
            result.Message.Should().Contain("successfully");
            result.Message.Should().Contain(expectedZipPath);
            _logCollectorMock.Verify(x => x.CollectAndZipLogs(), Times.Once);
        }

        [Fact]
        public void ExecuteAgentTask_WithDownloadAgentLogsTask_WhenLogCollectionFails_ReturnsCriticalResult()
        {
            _logCollectorMock.Setup(x => x.CollectAndZipLogs()).Returns(string.Empty);

            var task = new AgentTask
            {
                TaskId = Guid.NewGuid(),
                TaskType = TaskType.DownloadAgentLogs,
                CreatedAt = DateTime.UtcNow,
                Parameters = new Dictionary<string, string>()
            };

            var result = _agentTaskExecutor.ExecuteAgentTask(task);
            result.Should().NotBeNull();
            result.TaskId.Should().Be(task.TaskId);
            result.TaskStatus.Should().Be(Status.Critical);
            result.Message.Should().Contain("Failed to collect and zip logs");
        }

        [Fact]
        public void ExecuteAgentTask_WithUnsupportedTaskType_ReturnsNonUrgentResult()
        {
            var task = new AgentTask
            {
                TaskId = Guid.NewGuid(),
                TaskType = TaskType.RefreshStatus, // Not supported by AgentTaskExecutor
                CreatedAt = DateTime.UtcNow,
                Parameters = new Dictionary<string, string>()
            };

            var result = _agentTaskExecutor.ExecuteAgentTask(task);
            result.Should().NotBeNull();
            result.TaskId.Should().Be(task.TaskId);
            result.TaskStatus.Should().Be(Status.NonUrgent);
            result.Message.Should().Contain("not supported by AgentTaskExecutor");
        }

        [Fact]
        public void ExecuteAgentTask_WhenExceptionOccurs_ReturnsCriticalResult()
        {
            _logCollectorMock.Setup(x => x.CollectAndZipLogs()).Throws(new InvalidOperationException("Test exception"));

            var task = new AgentTask
            {
                TaskId = Guid.NewGuid(),
                TaskType = TaskType.DownloadAgentLogs,
                CreatedAt = DateTime.UtcNow,
                Parameters = new Dictionary<string, string>()
            };

            var result = _agentTaskExecutor.ExecuteAgentTask(task);
            result.Should().NotBeNull();
            result.TaskId.Should().Be(task.TaskId);
            result.TaskStatus.Should().Be(Status.Critical);
            result.Message.Should().Contain("Error executing task");
            result.Message.Should().Contain("Test exception");
        }

        [Fact]
        public void ExecuteAgentTask_LogsTaskExecution()
        {
            var task = new AgentTask
            {
                TaskId = Guid.NewGuid(),
                TaskType = TaskType.DownloadAgentLogs,
                CreatedAt = DateTime.UtcNow,
                Parameters = new Dictionary<string, string>()
            };

            _logCollectorMock.Setup(x => x.CollectAndZipLogs()).Returns("test.zip");

            _agentTaskExecutor.ExecuteAgentTask(task);
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Executing agent task")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

    }
}
