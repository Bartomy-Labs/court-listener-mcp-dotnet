namespace CourtListener.MCP.Server.Configuration;

/// <summary>
/// Configuration options for the CourtListener API client.
/// </summary>
public class CourtListenerOptions
{
    /// <summary>
    /// Gets or sets the base URL for the CourtListener API.
    /// Default: https://www.courtlistener.com/api/rest/v4/
    /// </summary>
    public string BaseUrl { get; set; } = "https://www.courtlistener.com/api/rest/v4/";

    /// <summary>
    /// Gets or sets the API key for authentication.
    /// This should be provided via User Secrets or environment variables, never hardcoded.
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Gets or sets the timeout duration in seconds for HTTP requests.
    /// Default: 30 seconds
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Gets or sets the log level for CourtListener operations.
    /// </summary>
    public string? LogLevel { get; set; }
}
