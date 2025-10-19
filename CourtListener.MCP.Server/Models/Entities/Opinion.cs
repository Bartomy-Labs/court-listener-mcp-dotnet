using System.Text.Json.Serialization;

namespace CourtListener.MCP.Server.Models.Entities;

/// <summary>
/// Represents a legal opinion document from CourtListener.
/// API returns mix of camelCase and snake_case - using attributes for snake_case fields.
/// </summary>
public class Opinion
{
    [JsonPropertyName("resource_uri")]
    public string? ResourceUri { get; set; }

    public int? Id { get; set; }

    [JsonPropertyName("absolute_url")]
    public string? AbsoluteUrl { get; set; }

    [JsonPropertyName("cluster_id")]
    public int? ClusterId { get; set; }

    public string? Cluster { get; set; }

    [JsonPropertyName("author_id")]
    public int? AuthorId { get; set; }

    public string? Author { get; set; }

    [JsonPropertyName("author_str")]
    public string? AuthorStr { get; set; }

    [JsonPropertyName("per_curiam")]
    public bool? PerCuriam { get; set; }

    [JsonPropertyName("joined_by")]
    public List<int>? JoinedBy { get; set; }

    [JsonPropertyName("joined_by_str")]
    public string? JoinedByStr { get; set; }

    [JsonPropertyName("date_created")]
    public DateTimeOffset? DateCreated { get; set; }

    [JsonPropertyName("date_modified")]
    public DateTimeOffset? DateModified { get; set; }

    public string? Type { get; set; }
    public string? Sha1 { get; set; }

    [JsonPropertyName("page_count")]
    public int? PageCount { get; set; }

    [JsonPropertyName("download_url")]
    public string? DownloadUrl { get; set; }

    [JsonPropertyName("local_path")]
    public string? LocalPath { get; set; }

    [JsonPropertyName("plain_text")]
    public string? PlainText { get; set; }

    public string? Html { get; set; }

    [JsonPropertyName("html_with_citations")]
    public string? HtmlWithCitations { get; set; }

    [JsonPropertyName("xml_harvard")]
    public string? XmlHarvard { get; set; }

    [JsonPropertyName("extracted_by_citations")]
    public int? ExtractedByCitations { get; set; }
}
