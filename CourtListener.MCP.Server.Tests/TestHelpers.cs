using System.Net;
using System.Text.Json;
using Moq;
using Moq.Protected;

namespace CourtListener.MCP.Server.Tests;

/// <summary>
/// Shared test helpers for mocking HTTP responses and common test scenarios.
/// </summary>
public static class TestHelpers
{
    /// <summary>
    /// Creates a mock HttpMessageHandler for testing HTTP client interactions.
    /// </summary>
    public static Mock<HttpMessageHandler> CreateMockHttpHandler()
    {
        return new Mock<HttpMessageHandler>();
    }

    /// <summary>
    /// Configures mock handler to return successful response with data.
    /// </summary>
    public static void MockSuccessResponse<T>(Mock<HttpMessageHandler> handler, T data)
    {
        var json = JsonSerializer.Serialize(data);

        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
            });
    }

    /// <summary>
    /// Configures mock handler to return 404 Not Found.
    /// </summary>
    public static void MockNotFoundResponse(Mock<HttpMessageHandler> handler)
    {
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = new StringContent("{\"detail\":\"Not found\"}", System.Text.Encoding.UTF8, "application/json")
            });
    }

    /// <summary>
    /// Configures mock handler to return 401 Unauthorized.
    /// </summary>
    public static void MockUnauthorizedResponse(Mock<HttpMessageHandler> handler)
    {
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Unauthorized,
                Content = new StringContent("{\"detail\":\"Invalid API key\"}", System.Text.Encoding.UTF8, "application/json")
            });
    }

    /// <summary>
    /// Configures mock handler to return 429 Rate Limited.
    /// </summary>
    public static void MockRateLimitedResponse(Mock<HttpMessageHandler> handler)
    {
        var response = new HttpResponseMessage
        {
            StatusCode = (HttpStatusCode)429,
            Content = new StringContent("{\"detail\":\"Rate limit exceeded\"}", System.Text.Encoding.UTF8, "application/json")
        };

        response.Headers.Add("Retry-After", "60");

        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(response);
    }

    /// <summary>
    /// Configures mock handler to return 500 Internal Server Error.
    /// </summary>
    public static void MockServerErrorResponse(Mock<HttpMessageHandler> handler)
    {
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new StringContent("{\"detail\":\"Internal server error\"}", System.Text.Encoding.UTF8, "application/json")
            });
    }

    /// <summary>
    /// Configures mock handler to throw an exception (simulates network failure).
    /// </summary>
    public static void MockNetworkFailure(Mock<HttpMessageHandler> handler)
    {
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new HttpRequestException("Network failure"));
    }
}
