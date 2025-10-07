namespace CourtListener.MCP.Server.Models.Errors;

/// <summary>
/// Represents a structured error response from an MCP tool.
/// Implements GAP #4 decision for error handling.
/// </summary>
/// <param name="Error">The error type (e.g., "NotFound", "Unauthorized").</param>
/// <param name="Message">A human-readable error message.</param>
/// <param name="Suggestion">Optional suggestion for resolving the error.</param>
public record ToolError(
    string Error,
    string Message,
    string? Suggestion = null
);
