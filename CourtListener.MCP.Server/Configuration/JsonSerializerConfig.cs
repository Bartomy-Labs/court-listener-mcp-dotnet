using System.Text.Json;
using System.Text.Json.Serialization;

namespace CourtListener.MCP.Server.Configuration;

/// <summary>
/// Centralized JSON serialization configuration with snake_case naming policy.
/// </summary>
public static class JsonSerializerConfig
{
    /// <summary>
    /// Gets the default JSON serializer options for CourtListener API responses.
    /// Configured with snake_case naming policy for clean C# PascalCase properties.
    /// </summary>
    public static JsonSerializerOptions Options { get; } = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        },
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };
}
