namespace CourtListener.MCP.Server.Models.Errors;

/// <summary>
/// Constants for MCP tool error types.
/// Implements GAP #4 decision for structured error handling.
/// </summary>
public static class ErrorTypes
{
    /// <summary>
    /// Resource was not found (404).
    /// </summary>
    public const string NotFound = "NotFound";

    /// <summary>
    /// Unauthorized access - API key missing or invalid (401).
    /// </summary>
    public const string Unauthorized = "Unauthorized";

    /// <summary>
    /// Rate limit exceeded (429).
    /// </summary>
    public const string RateLimited = "RateLimited";

    /// <summary>
    /// Validation error - invalid parameters provided (400).
    /// </summary>
    public const string ValidationError = "ValidationError";

    /// <summary>
    /// General API error (5xx or other errors).
    /// </summary>
    public const string ApiError = "ApiError";
}
