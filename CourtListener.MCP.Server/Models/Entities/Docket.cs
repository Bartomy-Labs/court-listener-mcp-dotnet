using System.Text.Json.Serialization;

namespace CourtListener.MCP.Server.Models.Entities;

/// <summary>
/// Represents a court docket from CourtListener.
/// API returns mix of camelCase and snake_case - using attributes for snake_case fields.
/// </summary>
public class Docket
{
    public int? Id { get; set; }

    [JsonPropertyName("absolute_url")]
    public string? AbsoluteUrl { get; set; }

    public string? Court { get; set; }

    [JsonPropertyName("court_id")]
    public string? CourtId { get; set; }

    public string? DocketNumber { get; set; }
    public string? CaseName { get; set; }
    public string? CaseNameShort { get; set; }
    public string? CaseNameFull { get; set; }
    public DateTimeOffset? DateFiled { get; set; }
    public DateTimeOffset? DateTerminated { get; set; }

    [JsonPropertyName("date_last_filing")]
    public DateTimeOffset? DateLastFiling { get; set; }

    [JsonPropertyName("assigned_to")]
    public string? AssignedTo { get; set; }

    [JsonPropertyName("assigned_to_str")]
    public string? AssignedToStr { get; set; }

    [JsonPropertyName("referred_to")]
    public string? ReferredTo { get; set; }

    [JsonPropertyName("referred_to_str")]
    public string? ReferredToStr { get; set; }

    public string? NatureOfSuit { get; set; }
    public string? Cause { get; set; }
    public string? JuryDemand { get; set; }
    public string? JurisdictionType { get; set; }
}
