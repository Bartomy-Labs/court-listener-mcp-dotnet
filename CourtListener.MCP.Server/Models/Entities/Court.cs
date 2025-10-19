using System.Text.Json.Serialization;

namespace CourtListener.MCP.Server.Models.Entities;

/// <summary>
/// Represents a court from CourtListener.
/// API returns mix of camelCase and snake_case - using attributes for snake_case fields.
/// </summary>
public class Court
{
    public string? Id { get; set; }

    [JsonPropertyName("absolute_url")]
    public string? AbsoluteUrl { get; set; }

    public string? FullName { get; set; }
    public string? ShortName { get; set; }
    public string? Citation { get; set; }
    public string? Jurisdiction { get; set; }

    [JsonPropertyName("date_modified")]
    public DateTimeOffset? DateModified { get; set; }

    [JsonPropertyName("in_use")]
    public bool? InUse { get; set; }

    [JsonPropertyName("has_opinion_scraper")]
    public bool? HasOpinionScraper { get; set; }

    [JsonPropertyName("has_oral_argument_scraper")]
    public bool? HasOralArgumentScraper { get; set; }

    public decimal? Position { get; set; }  // API returns decimal (e.g., 1.0) not integer
}
