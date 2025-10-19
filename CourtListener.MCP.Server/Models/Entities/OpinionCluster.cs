using System.Text.Json.Serialization;

namespace CourtListener.MCP.Server.Models.Entities;

/// <summary>
/// Represents an opinion cluster (case) from CourtListener search results.
/// A cluster groups related opinions for a single case.
/// API returns mix of camelCase and snake_case - using attributes for snake_case fields.
/// </summary>
public class OpinionCluster
{
    [JsonPropertyName("absolute_url")]
    public string? AbsoluteUrl { get; set; }

    public string? Attorney { get; set; }
    public string? CaseName { get; set; }
    public string? CaseNameFull { get; set; }
    public List<string>? Citation { get; set; }
    public int? CiteCount { get; set; }

    [JsonPropertyName("cluster_id")]
    public int? ClusterId { get; set; }

    public string? Court { get; set; }

    [JsonPropertyName("court_citation_string")]
    public string? CourtCitationString { get; set; }

    [JsonPropertyName("court_id")]
    public string? CourtId { get; set; }

    [JsonPropertyName("court_jurisdiction")]
    public string? CourtJurisdiction { get; set; }
    public DateTimeOffset? DateArgued { get; set; }
    public DateTimeOffset? DateFiled { get; set; }
    public DateTimeOffset? DateReargued { get; set; }
    public DateTimeOffset? DateReargumentDenied { get; set; }
    public string? DocketNumber { get; set; }

    [JsonPropertyName("docket_id")]
    public int? DocketId { get; set; }

    public string? Judge { get; set; }
    public string? LexisCite { get; set; }
    public object? Meta { get; set; }
    public string? NeutralCite { get; set; }

    [JsonPropertyName("non_participating_judge_ids")]
    public List<int>? NonParticipatingJudgeIds { get; set; }

    public List<OpinionSummary>? Opinions { get; set; }

    [JsonPropertyName("panel_ids")]
    public List<int>? PanelIds { get; set; }

    [JsonPropertyName("panel_names")]
    public List<string>? PanelNames { get; set; }

    public string? Posture { get; set; }
    public string? ProceduralHistory { get; set; }
    public string? ScdbId { get; set; }

    [JsonPropertyName("sibling_ids")]
    public List<int>? SiblingIds { get; set; }

    public string? Source { get; set; }
    public string? Status { get; set; }
    public string? SuitNature { get; set; }
    public string? Syllabus { get; set; }
}

/// <summary>
/// Represents a summary of an opinion within a cluster (from search results).
/// </summary>
public class OpinionSummary
{
    [JsonPropertyName("author_id")]
    public int? AuthorId { get; set; }

    public List<int>? Cites { get; set; }

    [JsonPropertyName("download_url")]
    public string? DownloadUrl { get; set; }

    public int? Id { get; set; }

    [JsonPropertyName("joined_by_ids")]
    public List<int>? JoinedByIds { get; set; }

    [JsonPropertyName("local_path")]
    public string? LocalPath { get; set; }

    public object? Meta { get; set; }

    [JsonPropertyName("ordering_key")]
    public int? OrderingKey { get; set; }

    [JsonPropertyName("per_curiam")]
    public bool? PerCuriam { get; set; }

    public string? Sha1 { get; set; }
    public string? Snippet { get; set; }
    public string? Type { get; set; }
}
