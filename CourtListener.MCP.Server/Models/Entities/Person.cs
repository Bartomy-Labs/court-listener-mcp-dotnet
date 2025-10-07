namespace CourtListener.MCP.Server.Models.Entities;

/// <summary>
/// Represents a person (judge, attorney, etc.) from CourtListener.
/// Properties are PascalCase and automatically serialize/deserialize to snake_case.
/// </summary>
public class Person
{
    public string? Id { get; set; }
    public string? AbsoluteUrl { get; set; }
    public string? Name { get; set; }
    public string? NameFull { get; set; }
    public string? NameFirst { get; set; }
    public string? NameMiddle { get; set; }
    public string? NameLast { get; set; }
    public string? NameSuffix { get; set; }
    public DateTimeOffset? DateDob { get; set; }
    public DateTimeOffset? DateGranularity { get; set; }
    public DateTimeOffset? DateDeath { get; set; }
    public string? Gender { get; set; }
    public string? Religion { get; set; }
    public string? FtwId { get; set; }
    public string? Slug { get; set; }
}
