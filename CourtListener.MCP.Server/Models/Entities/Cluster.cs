using System.Text.Json.Serialization;

namespace CourtListener.MCP.Server.Models.Entities;

/// <summary>
/// Represents an opinion cluster (group of related opinions) from CourtListener.
/// API returns mix of camelCase and snake_case - using attributes for snake_case fields.
/// </summary>
public class Cluster
{
    public int? Id { get; set; }

    [JsonPropertyName("absolute_url")]
    public string? AbsoluteUrl { get; set; }

    public List<string>? Panel { get; set; }

    [JsonPropertyName("non_participating_judges")]
    public List<string>? NonParticipatingJudges { get; set; }

    public string? Attorneys { get; set; }
    public string? Nature { get; set; }
    public string? Posture { get; set; }
    public string? Syllabus { get; set; }
    public string? HeadMatter { get; set; }
    public string? Summary { get; set; }
    public string? History { get; set; }
    public string? OtherDates { get; set; }
    public string? CrossReference { get; set; }
    public string? Correction { get; set; }
    public string? Citation { get; set; }

    [JsonPropertyName("citation_count")]
    public int? CitationCount { get; set; }

    public string? Precedential { get; set; }
    public DateTimeOffset? DateFiled { get; set; }

    [JsonPropertyName("date_filed_is_approximate")]
    public bool? DateFiledIsApproximate { get; set; }

    public string? Slug { get; set; }
    public string? CaseName { get; set; }
    public string? CaseNameShort { get; set; }
    public string? CaseNameFull { get; set; }
    public string? Scdb { get; set; }
    public string? Source { get; set; }
    public string? Procedural { get; set; }
    public string? Disposition { get; set; }

    [JsonPropertyName("sub_opinions")]
    public List<string>? SubOpinions { get; set; }
}
