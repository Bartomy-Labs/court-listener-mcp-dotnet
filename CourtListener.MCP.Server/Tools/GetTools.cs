using System.ComponentModel;
using System.Net;
using CourtListener.MCP.Server.Models.Entities;
using CourtListener.MCP.Server.Models.Errors;
using CourtListener.MCP.Server.Services;
using ModelContextProtocol.Server;

namespace CourtListener.MCP.Server.Tools;

/// <summary>
/// MCP tools for retrieving entities by ID from the CourtListener API.
/// Implements GAP #5 (PascalCase naming) and GAP #4 (structured errors).
/// </summary>
[McpServerToolType]
public class GetTools
{
    private readonly ICourtListenerClient _client;
    private readonly ILogger<GetTools> _logger;

    public GetTools(ICourtListenerClient client, ILogger<GetTools> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get a specific opinion by ID.
    /// </summary>
    [McpServerTool(Name = "get_opinion", ReadOnly = true, Idempotent = true)]
    [Description("Get a specific opinion by ID")]
    public async Task<object> GetOpinion(
        [Description("Opinion ID")] string opinionId,
        CancellationToken cancellationToken = default)
    {
        // Input validation
        if (string.IsNullOrWhiteSpace(opinionId))
        {
            return new ToolError(
                ErrorTypes.ValidationError,
                "Opinion ID cannot be empty",
                "Provide a valid opinion ID"
            );
        }

        // Log request
        _logger.LogInformation("Getting Opinion with ID: {Id}", opinionId);

        try
        {
            // Call API
            var result = await _client.GetAsync<Opinion>($"opinions/{opinionId}/", cancellationToken);

            // Handle not found
            if (result == null)
            {
                _logger.LogWarning("Opinion not found with ID: {Id}", opinionId);
                return new ToolError(
                    ErrorTypes.NotFound,
                    $"Opinion not found with ID: {opinionId}",
                    "Check if the ID is correct"
                );
            }

            // Log success
            _logger.LogInformation("Found Opinion with ID: {Id}", opinionId);
            return result;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            _logger.LogWarning("Unauthorized API access");
            return new ToolError(
                ErrorTypes.Unauthorized,
                "Invalid API key",
                "Check COURTLISTENER_API_KEY configuration"
            );
        }
        catch (HttpRequestException ex) when (ex.StatusCode == (HttpStatusCode)429)
        {
            _logger.LogWarning("Rate limit exceeded");
            return new ToolError(
                ErrorTypes.RateLimited,
                "Rate limit exceeded",
                "Retry after 60 seconds"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API error getting Opinion with ID: {Id}", opinionId);
            return new ToolError(
                ErrorTypes.ApiError,
                $"API error: {ex.Message}",
                "Check logs for details"
            );
        }
    }

    /// <summary>
    /// Get a specific docket by ID.
    /// </summary>
    [McpServerTool(Name = "get_docket", ReadOnly = true, Idempotent = true)]
    [Description("Get a specific docket by ID")]
    public async Task<object> GetDocket(
        [Description("Docket ID")] string docketId,
        CancellationToken cancellationToken = default)
    {
        // Input validation
        if (string.IsNullOrWhiteSpace(docketId))
        {
            return new ToolError(
                ErrorTypes.ValidationError,
                "Docket ID cannot be empty",
                "Provide a valid docket ID"
            );
        }

        // Log request
        _logger.LogInformation("Getting Docket with ID: {Id}", docketId);

        try
        {
            // Call API
            var result = await _client.GetAsync<Docket>($"dockets/{docketId}/", cancellationToken);

            // Handle not found
            if (result == null)
            {
                _logger.LogWarning("Docket not found with ID: {Id}", docketId);
                return new ToolError(
                    ErrorTypes.NotFound,
                    $"Docket not found with ID: {docketId}",
                    "Check if the ID is correct"
                );
            }

            // Log success
            _logger.LogInformation("Found Docket with ID: {Id}", docketId);
            return result;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            _logger.LogWarning("Unauthorized API access");
            return new ToolError(
                ErrorTypes.Unauthorized,
                "Invalid API key",
                "Check COURTLISTENER_API_KEY configuration"
            );
        }
        catch (HttpRequestException ex) when (ex.StatusCode == (HttpStatusCode)429)
        {
            _logger.LogWarning("Rate limit exceeded");
            return new ToolError(
                ErrorTypes.RateLimited,
                "Rate limit exceeded",
                "Retry after 60 seconds"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API error getting Docket with ID: {Id}", docketId);
            return new ToolError(
                ErrorTypes.ApiError,
                $"API error: {ex.Message}",
                "Check logs for details"
            );
        }
    }

    /// <summary>
    /// Get a specific oral argument audio recording by ID.
    /// </summary>
    [McpServerTool(Name = "get_audio", ReadOnly = true, Idempotent = true)]
    [Description("Get a specific oral argument audio recording by ID")]
    public async Task<object> GetAudio(
        [Description("Audio ID")] string audioId,
        CancellationToken cancellationToken = default)
    {
        // Input validation
        if (string.IsNullOrWhiteSpace(audioId))
        {
            return new ToolError(
                ErrorTypes.ValidationError,
                "Audio ID cannot be empty",
                "Provide a valid audio ID"
            );
        }

        // Log request
        _logger.LogInformation("Getting Audio with ID: {Id}", audioId);

        try
        {
            // Call API
            var result = await _client.GetAsync<Audio>($"audio/{audioId}/", cancellationToken);

            // Handle not found
            if (result == null)
            {
                _logger.LogWarning("Audio not found with ID: {Id}", audioId);
                return new ToolError(
                    ErrorTypes.NotFound,
                    $"Audio not found with ID: {audioId}",
                    "Check if the ID is correct"
                );
            }

            // Log success
            _logger.LogInformation("Found Audio with ID: {Id}", audioId);
            return result;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            _logger.LogWarning("Unauthorized API access");
            return new ToolError(
                ErrorTypes.Unauthorized,
                "Invalid API key",
                "Check COURTLISTENER_API_KEY configuration"
            );
        }
        catch (HttpRequestException ex) when (ex.StatusCode == (HttpStatusCode)429)
        {
            _logger.LogWarning("Rate limit exceeded");
            return new ToolError(
                ErrorTypes.RateLimited,
                "Rate limit exceeded",
                "Retry after 60 seconds"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API error getting Audio with ID: {Id}", audioId);
            return new ToolError(
                ErrorTypes.ApiError,
                $"API error: {ex.Message}",
                "Check logs for details"
            );
        }
    }

    /// <summary>
    /// Get a specific opinion cluster by ID.
    /// </summary>
    [McpServerTool(Name = "get_cluster", ReadOnly = true, Idempotent = true)]
    [Description("Get a specific opinion cluster by ID")]
    public async Task<object> GetCluster(
        [Description("Cluster ID")] string clusterId,
        CancellationToken cancellationToken = default)
    {
        // Input validation
        if (string.IsNullOrWhiteSpace(clusterId))
        {
            return new ToolError(
                ErrorTypes.ValidationError,
                "Cluster ID cannot be empty",
                "Provide a valid cluster ID"
            );
        }

        // Log request
        _logger.LogInformation("Getting Cluster with ID: {Id}", clusterId);

        try
        {
            // Call API
            var result = await _client.GetAsync<Cluster>($"clusters/{clusterId}/", cancellationToken);

            // Handle not found
            if (result == null)
            {
                _logger.LogWarning("Cluster not found with ID: {Id}", clusterId);
                return new ToolError(
                    ErrorTypes.NotFound,
                    $"Cluster not found with ID: {clusterId}",
                    "Check if the ID is correct"
                );
            }

            // Log success
            _logger.LogInformation("Found Cluster with ID: {Id}", clusterId);
            return result;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            _logger.LogWarning("Unauthorized API access");
            return new ToolError(
                ErrorTypes.Unauthorized,
                "Invalid API key",
                "Check COURTLISTENER_API_KEY configuration"
            );
        }
        catch (HttpRequestException ex) when (ex.StatusCode == (HttpStatusCode)429)
        {
            _logger.LogWarning("Rate limit exceeded");
            return new ToolError(
                ErrorTypes.RateLimited,
                "Rate limit exceeded",
                "Retry after 60 seconds"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API error getting Cluster with ID: {Id}", clusterId);
            return new ToolError(
                ErrorTypes.ApiError,
                $"API error: {ex.Message}",
                "Check logs for details"
            );
        }
    }

    /// <summary>
    /// Get a specific judge or legal professional by ID.
    /// </summary>
    [McpServerTool(Name = "get_person", ReadOnly = true, Idempotent = true)]
    [Description("Get a specific judge or legal professional by ID")]
    public async Task<object> GetPerson(
        [Description("Person ID")] string personId,
        CancellationToken cancellationToken = default)
    {
        // Input validation
        if (string.IsNullOrWhiteSpace(personId))
        {
            return new ToolError(
                ErrorTypes.ValidationError,
                "Person ID cannot be empty",
                "Provide a valid person ID"
            );
        }

        // Log request
        _logger.LogInformation("Getting Person with ID: {Id}", personId);

        try
        {
            // Call API
            var result = await _client.GetAsync<Person>($"people/{personId}/", cancellationToken);

            // Handle not found
            if (result == null)
            {
                _logger.LogWarning("Person not found with ID: {Id}", personId);
                return new ToolError(
                    ErrorTypes.NotFound,
                    $"Person not found with ID: {personId}",
                    "Check if the ID is correct"
                );
            }

            // Log success
            _logger.LogInformation("Found Person with ID: {Id}", personId);
            return result;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            _logger.LogWarning("Unauthorized API access");
            return new ToolError(
                ErrorTypes.Unauthorized,
                "Invalid API key",
                "Check COURTLISTENER_API_KEY configuration"
            );
        }
        catch (HttpRequestException ex) when (ex.StatusCode == (HttpStatusCode)429)
        {
            _logger.LogWarning("Rate limit exceeded");
            return new ToolError(
                ErrorTypes.RateLimited,
                "Rate limit exceeded",
                "Retry after 60 seconds"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API error getting Person with ID: {Id}", personId);
            return new ToolError(
                ErrorTypes.ApiError,
                $"API error: {ex.Message}",
                "Check logs for details"
            );
        }
    }

    /// <summary>
    /// Get court information by court ID.
    /// </summary>
    [McpServerTool(Name = "get_court", ReadOnly = true, Idempotent = true)]
    [Description("Get court information by court ID")]
    public async Task<object> GetCourt(
        [Description("Court ID (e.g., 'scotus', 'ca9')")] string courtId,
        CancellationToken cancellationToken = default)
    {
        // Input validation
        if (string.IsNullOrWhiteSpace(courtId))
        {
            return new ToolError(
                ErrorTypes.ValidationError,
                "Court ID cannot be empty",
                "Provide a valid court ID (e.g., 'scotus', 'ca9')"
            );
        }

        // Log request
        _logger.LogInformation("Getting Court with ID: {Id}", courtId);

        try
        {
            // Call API
            var result = await _client.GetAsync<Court>($"courts/{courtId}/", cancellationToken);

            // Handle not found
            if (result == null)
            {
                _logger.LogWarning("Court not found with ID: {Id}", courtId);
                return new ToolError(
                    ErrorTypes.NotFound,
                    $"Court not found with ID: {courtId}",
                    "Check if the ID is correct"
                );
            }

            // Log success
            _logger.LogInformation("Found Court with ID: {Id}", courtId);
            return result;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            _logger.LogWarning("Unauthorized API access");
            return new ToolError(
                ErrorTypes.Unauthorized,
                "Invalid API key",
                "Check COURTLISTENER_API_KEY configuration"
            );
        }
        catch (HttpRequestException ex) when (ex.StatusCode == (HttpStatusCode)429)
        {
            _logger.LogWarning("Rate limit exceeded");
            return new ToolError(
                ErrorTypes.RateLimited,
                "Rate limit exceeded",
                "Retry after 60 seconds"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API error getting Court with ID: {Id}", courtId);
            return new ToolError(
                ErrorTypes.ApiError,
                $"API error: {ex.Message}",
                "Check logs for details"
            );
        }
    }
}
