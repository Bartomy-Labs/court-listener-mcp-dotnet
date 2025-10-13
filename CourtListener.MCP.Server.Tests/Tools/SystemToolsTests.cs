using CourtListener.MCP.Server.Services;
using CourtListener.MCP.Server.Tools;
using Microsoft.Extensions.Logging;
using Moq;

namespace CourtListener.MCP.Server.Tests.Tools;

/// <summary>
/// Unit tests for SystemTools class.
/// </summary>
public class SystemToolsTests
{
    private readonly Mock<ICourtListenerClient> _mockClient;
    private readonly Mock<ILogger<SystemTools>> _mockLogger;
    private readonly SystemTools _systemTools;

    public SystemToolsTests()
    {
        _mockClient = new Mock<ICourtListenerClient>();
        _mockLogger = new Mock<ILogger<SystemTools>>();
        _systemTools = new SystemTools(_mockClient.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Status_ReturnsNonNull()
    {
        // Act
        var result = await _systemTools.Status();

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetApiStatus_WithSuccessfulResponse_ReturnsNonNull()
    {
        // Arrange
        _mockClient
            .Setup(c => c.GetAsync<object>(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new object());

        // Act
        var result = await _systemTools.GetApiStatus();

        // Assert
        Assert.NotNull(result);
        _mockClient.Verify(c => c.GetAsync<object>(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HealthCheck_ReturnsNonNull()
    {
        // Arrange
        _mockClient
            .Setup(c => c.GetAsync<object>(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new object());

        // Act
        var result = await _systemTools.HealthCheck();

        // Assert
        Assert.NotNull(result);
    }
}
