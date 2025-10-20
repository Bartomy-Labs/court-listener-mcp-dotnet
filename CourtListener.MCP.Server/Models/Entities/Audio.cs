using System.Text.Json.Serialization;

namespace CourtListener.MCP.Server.Models.Entities;

/// <summary>
/// Represents an oral argument audio recording from CourtListener.
/// API returns mix of camelCase and snake_case - using attributes for snake_case fields.
/// </summary>
public class Audio
{
    public int? Id { get; set; }

    [JsonPropertyName("absolute_url")]
    public string? AbsoluteUrl { get; set; }

    public string? Panel { get; set; }
    public string? Judges { get; set; }
    public string? Sha1 { get; set; }

    [JsonPropertyName("date_created")]
    public DateTimeOffset? DateCreated { get; set; }

    [JsonPropertyName("date_modified")]
    public DateTimeOffset? DateModified { get; set; }

    public string? Source { get; set; }
    public string? CaseName { get; set; }
    public string? CaseNameShort { get; set; }
    public string? CaseNameFull { get; set; }
    public DateTimeOffset? DateArgued { get; set; }
    public DateTimeOffset? DateReargued { get; set; }
    public DateTimeOffset? DateReargumentDenied { get; set; }
    public string? Docket { get; set; }

    [JsonPropertyName("local_path_mp3")]
    public string? LocalPathMp3 { get; set; }

    [JsonPropertyName("local_path_original_file")]
    public string? LocalPathOriginalFile { get; set; }

    public int? Duration { get; set; }

    [JsonPropertyName("processing_complete")]
    public string? ProcessingComplete { get; set; }
}
