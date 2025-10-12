using System.ComponentModel;
using System.Diagnostics;
using CourtListener.MCP.Server.Models.Errors;
using CourtListener.MCP.Server.Services;
using ModelContextProtocol.Server;

namespace CourtListener.MCP.Server.Tools;

/// <summary>
/// MCP tools for system status, health checks, and server monitoring.
/// </summary>
[McpServerToolType]
public class SystemTools
{
    private readonly ICourtListenerClient _client;
    private readonly ILogger<SystemTools> _logger;
    private static readonly DateTime _startTime = DateTime.UtcNow;

    public SystemTools(ICourtListenerClient client, ILogger<SystemTools> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get MCP server status, metrics, and configuration.
    /// </summary>
    [McpServerTool(Name = "status", ReadOnly = true, Idempotent = true)]
    [Description("Get MCP server status, metrics, and configuration")]
    public Task<object> Status()
    {
        _logger.LogInformation("Getting server status");

        try
        {
            var process = Process.GetCurrentProcess();
            var uptime = DateTime.UtcNow - _startTime;

            var status = new
            {
                Server = "CourtListener MCP Server",
                Version = "1.0.0",
                Framework = ".NET 9",
                Transport = "HTTP",
                Endpoint = "http://0.0.0.0:8000/mcp/",
                Status = "Running",
                Uptime = $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s",
                Memory = new
                {
                    WorkingSetMB = process.WorkingSet64 / 1024 / 1024,
                    PrivateMemoryMB = process.PrivateMemorySize64 / 1024 / 1024
                },
                Threads = process.Threads.Count,
                ToolsAvailable = 21,
                ApiBaseUrl = "https://www.courtlistener.com/api/rest/v4/"
            };

            _logger.LogInformation(
                "Server status retrieved: {Status}, Uptime: {Uptime}",
                status.Status,
                status.Uptime
            );

            return Task.FromResult<object>(status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving server status");
            return Task.FromResult<object>(new ToolError(
                ErrorTypes.ApiError,
                $"Status error: {ex.Message}",
                "Check server logs for details"
            ));
        }
    }

    /// <summary>
    /// Check CourtListener API health and connectivity.
    /// </summary>
    [McpServerTool(Name = "get_api_status", ReadOnly = true, Idempotent = true)]
    [Description("Check CourtListener API health and connectivity")]
    public async Task<object> GetApiStatus(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Checking CourtListener API status");

        try
        {
            var stopwatch = Stopwatch.StartNew();

            // Make a simple GET request to the API root
            await _client.GetAsync<object>("/", cancellationToken);

            stopwatch.Stop();

            var result = new
            {
                ApiUrl = "https://www.courtlistener.com/api/rest/v4/",
                Status = "Healthy",
                ResponseTimeMs = stopwatch.ElapsedMilliseconds,
                Timestamp = DateTime.UtcNow
            };

            _logger.LogInformation(
                "API status check successful: {ResponseTime}ms",
                result.ResponseTimeMs
            );

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "API health check failed");
            return new ToolError(
                ErrorTypes.ApiError,
                $"API health check failed: {ex.Message}",
                "Check network connectivity and API key configuration"
            );
        }
    }

    /// <summary>
    /// Comprehensive health check of MCP server and dependencies.
    /// </summary>
    [McpServerTool(Name = "health_check", ReadOnly = true, Idempotent = true)]
    [Description("Comprehensive health check of MCP server and dependencies")]
    public async Task<object> HealthCheck(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Performing comprehensive health check");

        var checks = new Dictionary<string, object>();

        // Server health
        try
        {
            var process = Process.GetCurrentProcess();
            checks["Server"] = new
            {
                Status = "Healthy",
                Message = "MCP server is running",
                UptimeSeconds = (DateTime.UtcNow - _startTime).TotalSeconds
            };
        }
        catch (Exception ex)
        {
            checks["Server"] = new
            {
                Status = "Unhealthy",
                Message = $"Server check failed: {ex.Message}"
            };
        }

        // API connectivity
        try
        {
            var stopwatch = Stopwatch.StartNew();
            await _client.GetAsync<object>("/", cancellationToken);
            stopwatch.Stop();

            checks["CourtListenerApi"] = new
            {
                Status = "Healthy",
                Message = "API is accessible",
                ResponseTimeMs = stopwatch.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            checks["CourtListenerApi"] = new
            {
                Status = "Unhealthy",
                Message = $"API check failed: {ex.Message}"
            };
        }

        // Memory check
        try
        {
            var process = Process.GetCurrentProcess();
            var memoryMB = process.WorkingSet64 / 1024 / 1024;
            var threshold = 500;

            checks["Memory"] = new
            {
                Status = memoryMB < threshold ? "Healthy" : "Warning",
                WorkingSetMB = memoryMB,
                ThresholdMB = threshold,
                Message = memoryMB < threshold
                    ? "Memory usage is normal"
                    : "Memory usage is elevated"
            };
        }
        catch (Exception ex)
        {
            checks["Memory"] = new
            {
                Status = "Unhealthy",
                Message = $"Memory check failed: {ex.Message}"
            };
        }

        // Determine overall status
        var allHealthy = checks.All(c =>
        {
            var checkValue = (dynamic)c.Value;
            return checkValue.Status == "Healthy";
        });

        var result = new
        {
            Overall = allHealthy ? "Healthy" : "Degraded",
            Checks = checks,
            Timestamp = DateTime.UtcNow
        };

        _logger.LogInformation(
            "Health check complete: Overall={Overall}, Checks={CheckCount}",
            result.Overall,
            checks.Count
        );

        return result;
    }
}
