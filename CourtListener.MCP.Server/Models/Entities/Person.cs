using System.Text.Json.Serialization;

namespace CourtListener.MCP.Server.Models.Entities;

/// <summary>
/// Represents a person (judge, attorney, etc.) from CourtListener.
/// API returns mix of camelCase and snake_case - using attributes for snake_case fields.
/// </summary>
public class Person
{
    public int? Id { get; set; }

    [JsonPropertyName("absolute_url")]
    public string? AbsoluteUrl { get; set; }

    public string? Name { get; set; }

    [JsonPropertyName("name_full")]
    public string? NameFull { get; set; }

    [JsonPropertyName("name_first")]
    public string? NameFirst { get; set; }

    [JsonPropertyName("name_middle")]
    public string? NameMiddle { get; set; }

    [JsonPropertyName("name_last")]
    public string? NameLast { get; set; }

    [JsonPropertyName("name_suffix")]
    public string? NameSuffix { get; set; }

    [JsonPropertyName("date_dob")]
    public DateTimeOffset? DateDob { get; set; }

    [JsonPropertyName("date_granularity")]
    public DateTimeOffset? DateGranularity { get; set; }

    [JsonPropertyName("date_death")]
    public DateTimeOffset? DateDeath { get; set; }

    public string? Gender { get; set; }
    public string? Religion { get; set; }

    [JsonPropertyName("ftw_id")]
    public string? FtwId { get; set; }

    public string? Slug { get; set; }
}
