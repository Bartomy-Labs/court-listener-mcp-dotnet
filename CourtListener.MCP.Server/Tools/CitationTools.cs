using System.ComponentModel;
using System.Net;
using CourtListener.MCP.Server.Models.Citations;
using CourtListener.MCP.Server.Models.Errors;
using CourtListener.MCP.Server.Services;
using ModelContextProtocol.Server;

namespace CourtListener.MCP.Server.Tools;

/// <summary>
/// MCP tools for looking up legal citations using the CourtListener API.
/// Implements GAP #5 (PascalCase naming) and GAP #4 (structured errors).
/// </summary>
[McpServerToolType]
public class CitationTools
{
    private readonly ICourtListenerClient _client;
    private readonly ILogger<CitationTools> _logger;

    public CitationTools(ICourtListenerClient client, ILogger<CitationTools> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Look up a legal citation using CourtListener API.
    /// </summary>
    [McpServerTool(Name = "lookup_citation", ReadOnly = true, Idempotent = true)]
    [Description("Look up a legal citation using CourtListener API")]
    public async Task<object> LookupCitation(
        [Description("Legal citation to look up (e.g., '410 U.S. 113')")] string citation,
        CancellationToken cancellationToken = default)
    {
        // Input validation
        if (string.IsNullOrWhiteSpace(citation))
        {
            return new ToolError(
                ErrorTypes.ValidationError,
                "Citation cannot be empty",
                "Provide a valid legal citation (e.g., '410 U.S. 113')"
            );
        }

        // Log request
        _logger.LogInformation("Looking up citation: {Citation}", citation);

        try
        {
            // Build form data
            var formData = new Dictionary<string, string>
            {
                { "text", citation }
            };

            // Call API (POST form data)
            var result = await _client.PostFormAsync<CitationLookupResult>(
                "citation-lookup/",
                formData,
                cancellationToken
            );

            // Handle not found
            if (result == null)
            {
                _logger.LogWarning("No matches found for citation: {Citation}", citation);
                return new ToolError(
                    ErrorTypes.NotFound,
                    $"No matches found for citation: {citation}",
                    "Check if the citation format is correct"
                );
            }

            // Log success
            _logger.LogInformation(
                "Found {Count} matches for citation: {Citation}",
                result.Matches?.Count ?? 0,
                citation
            );
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
            _logger.LogError(ex, "API error looking up citation: {Citation}", citation);
            return new ToolError(
                ErrorTypes.ApiError,
                $"API error: {ex.Message}",
                "Check logs for details"
            );
        }
    }

    /// <summary>
    /// Batch lookup multiple legal citations (max 100).
    /// </summary>
    [McpServerTool(Name = "batch_lookup_citations", ReadOnly = true, Idempotent = true)]
    [Description("Batch lookup multiple legal citations (max 100)")]
    public async Task<object> BatchLookupCitations(
        [Description("Array of citations to look up")] string[] citations,
        CancellationToken cancellationToken = default)
    {
        // Input validation
        if (citations == null || citations.Length == 0)
        {
            return new ToolError(
                ErrorTypes.ValidationError,
                "Citations array cannot be empty",
                "Provide at least one citation"
            );
        }

        if (citations.Length > 100)
        {
            return new ToolError(
                ErrorTypes.ValidationError,
                "Maximum 100 citations allowed per batch",
                $"Provided: {citations.Length} citations"
            );
        }

        // Log request
        _logger.LogInformation("Batch lookup of {Count} citations", citations.Length);

        try
        {
            // Join citations with spaces
            var combinedText = string.Join(" ", citations);

            // Build form data
            var formData = new Dictionary<string, string>
            {
                { "text", combinedText }
            };

            // Call API (POST form data)
            var result = await _client.PostFormAsync<CitationLookupResult>(
                "citation-lookup/",
                formData,
                cancellationToken
            );

            // Handle not found
            if (result == null)
            {
                _logger.LogWarning("No matches found for {Count} citations", citations.Length);
                return new ToolError(
                    ErrorTypes.NotFound,
                    $"No matches found for {citations.Length} citations",
                    "Check if the citation formats are correct"
                );
            }

            // Log success
            _logger.LogInformation(
                "Found {MatchCount} total matches for {InputCount} citations",
                result.Matches?.Count ?? 0,
                citations.Length
            );
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
            _logger.LogError(ex, "API error looking up {Count} citations", citations.Length);
            return new ToolError(
                ErrorTypes.ApiError,
                $"API error: {ex.Message}",
                "Check logs for details"
            );
        }
    }
}
