using System.ComponentModel;
using System.Net;
using System.Text.Json;
using CourtListener.MCP.Server.Models.Errors;
using CourtListener.MCP.Server.Models.Search;
using CourtListener.MCP.Server.Services;
using ModelContextProtocol.Server;

namespace CourtListener.MCP.Server.Tools;

/// <summary>
/// MCP tools for searching the CourtListener API.
/// Implements GAP #5 (PascalCase naming) and GAP #4 (structured errors).
/// </summary>
[McpServerToolType]
public class SearchTools
{
    private readonly ICourtListenerClient _client;
    private readonly ILogger<SearchTools> _logger;

    public SearchTools(ICourtListenerClient client, ILogger<SearchTools> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Search legal opinions and court decisions from CourtListener API.
    /// </summary>
    [McpServerTool(Name = "search_opinions", ReadOnly = true, Idempotent = true)]
    [Description("Search legal opinions and court decisions from CourtListener API")]
    public async Task<string> SearchOpinions(
        [Description("Search query text")] string query,
        [Description("Court identifier")] string? court = null,
        [Description("Case name to search for")] string? caseName = null,
        [Description("Judge name to filter by")] string? judge = null,
        [Description("Filed after date (YYYY-MM-DD)")] string? filedAfter = null,
        [Description("Filed before date (YYYY-MM-DD)")] string? filedBefore = null,
        [Description("Minimum citation count")] int? citedGt = null,
        [Description("Maximum citation count")] int? citedLt = null,
        [Description("Sort order field")] string? orderBy = null,
        [Description("Maximum results to return (1-100)")] int limit = 20,
        CancellationToken cancellationToken = default)
    {
        // Input validation (GAP #4: Validate before API call)
        if (string.IsNullOrWhiteSpace(query))
        {
            return JsonSerializer.Serialize(new ToolError(
                ErrorTypes.ValidationError,
                "Query parameter cannot be empty",
                "Provide a search query term"
            ));
        }

        if (limit <= 0 || limit > 100)
        {
            return JsonSerializer.Serialize(new ToolError(
                ErrorTypes.ValidationError,
                "Limit must be between 1 and 100",
                $"Provided limit: {limit}"
            ));
        }

        // Validate date formats if provided
        if (!string.IsNullOrEmpty(filedAfter) && !IsValidDateFormat(filedAfter))
        {
            return JsonSerializer.Serialize(new ToolError(
                ErrorTypes.ValidationError,
                "Invalid filedAfter date format",
                "Use YYYY-MM-DD format (e.g., 2024-01-15)"
            ));
        }

        if (!string.IsNullOrEmpty(filedBefore) && !IsValidDateFormat(filedBefore))
        {
            return JsonSerializer.Serialize(new ToolError(
                ErrorTypes.ValidationError,
                "Invalid filedBefore date format",
                "Use YYYY-MM-DD format (e.g., 2024-12-31)"
            ));
        }

        // Log request
        _logger.LogInformation(
            "Searching opinions: Query={Query}, Court={Court}, Limit={Limit}",
            query,
            court ?? "all",
            limit
        );

        try
        {
            // Build query string
            var endpoint = BuildSearchEndpoint(
                query,
                court,
                caseName,
                judge,
                filedAfter,
                filedBefore,
                citedGt,
                citedLt,
                orderBy,
                limit
            );

            // Call API
            var result = await _client.GetAsync<OpinionSearchResult>(endpoint, cancellationToken);

            // Handle not found (404 returns null from client)
            if (result == null)
            {
                return JsonSerializer.Serialize(new ToolError(
                    ErrorTypes.NotFound,
                    "No opinions found matching criteria"
                ));
            }

            // Log success
            _logger.LogInformation(
                "Found {Count} opinions for query: {Query}",
                result.Count,
                query
            );

            return JsonSerializer.Serialize(result, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            _logger.LogWarning("Unauthorized API access");
            return JsonSerializer.Serialize(new ToolError(
                ErrorTypes.Unauthorized,
                "Invalid API key",
                "Check COURTLISTENER_API_KEY configuration"
            ));
        }
        catch (HttpRequestException ex) when (ex.StatusCode == (HttpStatusCode)429)
        {
            _logger.LogWarning("Rate limit exceeded");
            return JsonSerializer.Serialize(new ToolError(
                ErrorTypes.RateLimited,
                "Rate limit exceeded",
                "Retry after 60 seconds"
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API error searching opinions: {Message}", ex.Message);
            return JsonSerializer.Serialize(new ToolError(
                ErrorTypes.ApiError,
                $"API error: {ex.Message}",
                "Check logs for details"
            ));
        }
    }

    /// <summary>
    /// Builds the search endpoint with query parameters.
    /// Parameters are snake_case via global JSON naming policy.
    /// </summary>
    private string BuildSearchEndpoint(
        string query,
        string? court,
        string? caseName,
        string? judge,
        string? filedAfter,
        string? filedBefore,
        int? citedGt,
        int? citedLt,
        string? orderBy,
        int limit)
    {
        var queryParams = new List<string>
        {
            "type=o", // Opinion search type
            $"q={Uri.EscapeDataString(query)}",
            $"hit={limit}" // API uses 'hit' not 'limit'
        };

        if (!string.IsNullOrEmpty(court))
            queryParams.Add($"court={Uri.EscapeDataString(court)}");

        if (!string.IsNullOrEmpty(caseName))
            queryParams.Add($"case_name={Uri.EscapeDataString(caseName)}");

        if (!string.IsNullOrEmpty(judge))
            queryParams.Add($"judge={Uri.EscapeDataString(judge)}");

        if (!string.IsNullOrEmpty(filedAfter))
            queryParams.Add($"filed_after={filedAfter}");

        if (!string.IsNullOrEmpty(filedBefore))
            queryParams.Add($"filed_before={filedBefore}");

        if (citedGt.HasValue)
            queryParams.Add($"cited_gt={citedGt.Value}");

        if (citedLt.HasValue)
            queryParams.Add($"cited_lt={citedLt.Value}");

        if (!string.IsNullOrEmpty(orderBy))
            queryParams.Add($"order_by={Uri.EscapeDataString(orderBy)}");

        return $"search/?{string.Join("&", queryParams)}";
    }

    /// <summary>
    /// Validates date format (YYYY-MM-DD).
    /// </summary>
    private bool IsValidDateFormat(string date)
    {
        return DateTime.TryParseExact(
            date,
            "yyyy-MM-dd",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None,
            out _
        );
    }
}
