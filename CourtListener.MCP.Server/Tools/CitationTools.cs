using System.ComponentModel;
using System.Net;
using CiteUrl.Core.Templates;
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

    /// <summary>
    /// Verify citation format using CiteUrl.NET template system.
    /// </summary>
    [McpServerTool(Name = "verify_citation_format", ReadOnly = true, Idempotent = true)]
    [Description("Verify citation format using CiteUrl.NET template system")]
    public Task<object> VerifyCitationFormat(
        [Description("Citation to verify")] string citation,
        CancellationToken cancellationToken = default)
    {
        // Input validation
        if (string.IsNullOrWhiteSpace(citation))
        {
            return Task.FromResult<object>(new ToolError(
                ErrorTypes.ValidationError,
                "Citation cannot be empty",
                "Provide a valid citation to verify"
            ));
        }

        // Log request
        _logger.LogInformation("Verifying citation format: {Citation}", citation);

        try
        {
            // Use CiteUrl.NET Citator to find matching citation
            var citedCitation = Citator.Cite(citation);

            if (citedCitation != null)
            {
                _logger.LogInformation(
                    "Citation format verified: {Citation} matches {Format}",
                    citation,
                    citedCitation.Template.Name
                );

                return Task.FromResult<object>(new
                {
                    IsValid = true,
                    Citation = citation,
                    Format = citedCitation.Template.Name,
                    MatchedText = citedCitation.Text
                });
            }
            else
            {
                _logger.LogWarning("No matching citation format found for: {Citation}", citation);

                return Task.FromResult<object>(new
                {
                    IsValid = false,
                    Citation = citation,
                    Message = "No matching citation format found"
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying citation format: {Citation}", citation);
            return Task.FromResult<object>(new ToolError(
                ErrorTypes.ApiError,
                $"Validation error: {ex.Message}",
                "Check logs for details"
            ));
        }
    }

    /// <summary>
    /// Parse citation into structured components using CiteUrl.NET.
    /// </summary>
    [McpServerTool(Name = "parse_citation", ReadOnly = true, Idempotent = true)]
    [Description("Parse citation into structured components using CiteUrl.NET")]
    public Task<object> ParseCitation(
        [Description("Citation to parse")] string citation,
        CancellationToken cancellationToken = default)
    {
        // Input validation
        if (string.IsNullOrWhiteSpace(citation))
        {
            return Task.FromResult<object>(new ToolError(
                ErrorTypes.ValidationError,
                "Citation cannot be empty",
                "Provide a valid citation to parse"
            ));
        }

        // Log request
        _logger.LogInformation("Parsing citation: {Citation}", citation);

        try
        {
            // Use CiteUrl.NET Citator to parse citation
            var parsed = Citator.Cite(citation);

            if (parsed != null)
            {
                _logger.LogInformation(
                    "Citation parsed successfully: {Citation}",
                    citation
                );

                // Build result from citation tokens and properties
                return Task.FromResult<object>(new
                {
                    Citation = citation,
                    MatchedText = parsed.Text,
                    TemplateName = parsed.Template.Name,
                    Tokens = parsed.Tokens,
                    Url = parsed.Url,
                    Name = parsed.Name
                });
            }
            else
            {
                _logger.LogWarning("Could not parse citation: {Citation}", citation);

                return Task.FromResult<object>(new ToolError(
                    ErrorTypes.ValidationError,
                    "Could not parse citation",
                    "Citation format may be invalid or unsupported"
                ));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing citation: {Citation}", citation);
            return Task.FromResult<object>(new ToolError(
                ErrorTypes.ApiError,
                $"Parse error: {ex.Message}",
                "Check logs for details"
            ));
        }
    }

    /// <summary>
    /// Extract all citations from a text block using CiteUrl.NET YAML templates.
    /// </summary>
    [McpServerTool(Name = "extract_citations_from_text", ReadOnly = true, Idempotent = true)]
    [Description("Extract all citations from a text block using CiteUrl.NET YAML templates")]
    public Task<object> ExtractCitationsFromText(
        [Description("Text block to extract citations from")] string text,
        CancellationToken cancellationToken = default)
    {
        // Input validation
        if (string.IsNullOrWhiteSpace(text))
        {
            return Task.FromResult<object>(new ToolError(
                ErrorTypes.ValidationError,
                "Text cannot be empty",
                "Provide text to extract citations from"
            ));
        }

        // Log request
        _logger.LogInformation(
            "Extracting citations from text (length: {Length} characters)",
            text.Length
        );

        try
        {
            // Use CiteUrl.NET Citator to extract all citations (.NET uses PascalCase: ListCitations)
            var citations = Citator.ListCitations(text);

            if (citations == null || !citations.Any())
            {
                _logger.LogInformation("No citations found in text");

                return Task.FromResult<object>(new
                {
                    TextPreview = text.Length > 100 ? text.Substring(0, 100) + "..." : text,
                    TextLength = text.Length,
                    CitationsFound = 0,
                    Citations = new List<object>()
                });
            }

            // Build results from found citations
            var results = citations.Select(cite => new
            {
                Citation = cite.Text,
                TemplateName = cite.Template.Name,
                Tokens = cite.Tokens,
                Url = cite.Url,
                Name = cite.Name
            }).ToList();

            // Log success
            _logger.LogInformation(
                "Extracted {Count} citations from text",
                results.Count
            );

            return Task.FromResult<object>(new
            {
                TextPreview = text.Length > 100 ? text.Substring(0, 100) + "..." : text,
                TextLength = text.Length,
                CitationsFound = results.Count,
                Citations = results
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting citations from text");
            return Task.FromResult<object>(new ToolError(
                ErrorTypes.ApiError,
                $"Extraction error: {ex.Message}",
                "Check logs for details"
            ));
        }
    }

    /// <summary>
    /// Enhanced citation lookup combining CiteUrl.NET validation with CourtListener API data.
    /// </summary>
    [McpServerTool(Name = "enhanced_citation_lookup", ReadOnly = true, Idempotent = true)]
    [Description("Enhanced citation lookup combining CiteUrl.NET validation with CourtListener API data")]
    public async Task<object> EnhancedCitationLookup(
        [Description("Citation to look up")] string citation,
        CancellationToken cancellationToken = default)
    {
        // Input validation
        if (string.IsNullOrWhiteSpace(citation))
        {
            return new ToolError(
                ErrorTypes.ValidationError,
                "Citation cannot be empty",
                "Provide a valid legal citation"
            );
        }

        // Log request
        _logger.LogInformation("Enhanced lookup for citation: {Citation}", citation);

        try
        {
            // Step 1: Validate and parse with CiteUrl.NET
            var parsed = Citator.Cite(citation);

            object? validationData = null;
            if (parsed != null)
            {
                validationData = new
                {
                    IsValid = true,
                    Format = parsed.Template.Name,
                    MatchedText = parsed.Text,
                    Tokens = parsed.Tokens,
                    Url = parsed.Url
                };

                _logger.LogInformation(
                    "Citation validated: {Citation} matches {Format}",
                    citation,
                    parsed.Template.Name
                );
            }
            else
            {
                validationData = new
                {
                    IsValid = false,
                    Message = "No matching citation format found"
                };

                _logger.LogWarning("Citation format not recognized: {Citation}", citation);
            }

            // Step 2: Lookup in CourtListener API
            var formData = new Dictionary<string, string>
            {
                { "text", citation }
            };

            var apiResult = await _client.PostFormAsync<CitationLookupResult>(
                "citation-lookup/",
                formData,
                cancellationToken
            );

            // Step 3: Combine results
            _logger.LogInformation(
                "Enhanced lookup complete: validation={Validated}, matches={MatchCount}",
                parsed != null,
                apiResult?.Matches?.Count ?? 0
            );

            return new
            {
                Citation = citation,
                Validation = validationData,
                CourtListenerData = apiResult
            };
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            _logger.LogWarning("Unauthorized API access during enhanced lookup");
            return new ToolError(
                ErrorTypes.Unauthorized,
                "Invalid API key",
                "Check COURTLISTENER_API_KEY configuration"
            );
        }
        catch (HttpRequestException ex) when (ex.StatusCode == (HttpStatusCode)429)
        {
            _logger.LogWarning("Rate limit exceeded during enhanced lookup");
            return new ToolError(
                ErrorTypes.RateLimited,
                "Rate limit exceeded",
                "Retry after 60 seconds"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in enhanced citation lookup: {Citation}", citation);
            return new ToolError(
                ErrorTypes.ApiError,
                $"Lookup error: {ex.Message}",
                "Check logs for details"
            );
        }
    }
}
