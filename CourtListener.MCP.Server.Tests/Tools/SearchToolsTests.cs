using CourtListener.MCP.Server.Services;
using CourtListener.MCP.Server.Tools;
using Microsoft.Extensions.Logging;
using Moq;

namespace CourtListener.MCP.Server.Tests.Tools;

/// <summary>
/// Unit tests for SearchTools class.
/// </summary>
public class SearchToolsTests
{
    private readonly Mock<ICourtListenerClient> _mockClient;
    private readonly Mock<ILogger<SearchTools>> _mockLogger;
    private readonly SearchTools _searchTools;

    public SearchToolsTests()
    {
        _mockClient = new Mock<ICourtListenerClient>();
        _mockLogger = new Mock<ILogger<SearchTools>>();
        _searchTools = new SearchTools(_mockClient.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task SearchOpinions_WithValidQuery_CallsClient()
    {
        // Arrange
        _mockClient
            .Setup(c => c.GetAsync<object>(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new { count = 1, results = new[] { new { id = 123, case_name = "Test v. Case" } } });

        // Act
        var result = await _searchTools.SearchOpinions("test query", cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        _mockClient.Verify(c => c.GetAsync<object>(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchOpinions_WithEmptyQuery_ReturnsError()
    {
        // Act
        var result = await _searchTools.SearchOpinions("", cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        // Should return an error without calling client
        _mockClient.Verify(c => c.GetAsync<object>(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task SearchOpinions_WithInvalidLimit_ReturnsError()
    {
        // Act
        var result = await _searchTools.SearchOpinions("test", limit: 150, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        // Should return an error without calling client
        _mockClient.Verify(c => c.GetAsync<object>(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }
}
