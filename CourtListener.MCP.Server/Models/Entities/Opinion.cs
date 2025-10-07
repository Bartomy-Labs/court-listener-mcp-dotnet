namespace CourtListener.MCP.Server.Models.Entities;

/// <summary>
/// Represents a legal opinion document from CourtListener.
/// Properties are PascalCase and automatically serialize/deserialize to snake_case.
/// </summary>
public class Opinion
{
    public string? Id { get; set; }
    public string? AbsoluteUrl { get; set; }
    public string? Cluster { get; set; }
    public string? Author { get; set; }
    public string? AuthorStr { get; set; }
    public int? PerCuriam { get; set; }
    public string? JoinedBy { get; set; }
    public string? Type { get; set; }
    public string? Sha1 { get; set; }
    public string? PageCount { get; set; }
    public string? DownloadUrl { get; set; }
    public string? LocalPath { get; set; }
    public string? PlainText { get; set; }
    public string? Html { get; set; }
    public string? HtmlWithCitations { get; set; }
    public string? XmlHarvard { get; set; }
    public int? ExtractedByCitations { get; set; }
}
