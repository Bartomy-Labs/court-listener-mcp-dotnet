namespace CourtListener.MCP.Server.Models.Entities;

/// <summary>
/// Represents a court from CourtListener.
/// Properties are PascalCase and automatically serialize/deserialize to snake_case.
/// </summary>
public class Court
{
    public string? Id { get; set; }
    public string? AbsoluteUrl { get; set; }
    public string? FullName { get; set; }
    public string? ShortName { get; set; }
    public string? Citation { get; set; }
    public string? Jurisdiction { get; set; }
    public DateTimeOffset? DateModified { get; set; }
    public bool? InUse { get; set; }
    public bool? HasOpinionScraper { get; set; }
    public bool? HasOralArgumentScraper { get; set; }
    public decimal? Position { get; set; }  // API returns decimal (e.g., 1.0) not integer
}
