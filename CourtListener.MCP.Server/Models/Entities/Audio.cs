namespace CourtListener.MCP.Server.Models.Entities;

/// <summary>
/// Represents an oral argument audio recording from CourtListener.
/// Properties are PascalCase and automatically serialize/deserialize to snake_case.
/// </summary>
public class Audio
{
    public string? Id { get; set; }
    public string? AbsoluteUrl { get; set; }
    public string? Panel { get; set; }
    public string? Judges { get; set; }
    public string? Sha1 { get; set; }
    public DateTimeOffset? DateCreated { get; set; }
    public DateTimeOffset? DateModified { get; set; }
    public string? Source { get; set; }
    public string? CaseName { get; set; }
    public string? CaseNameShort { get; set; }
    public string? CaseNameFull { get; set; }
    public DateTimeOffset? DateArgued { get; set; }
    public DateTimeOffset? DateReargued { get; set; }
    public DateTimeOffset? DateReargumentDenied { get; set; }
    public string? Docket { get; set; }
    public string? LocalPathMp3 { get; set; }
    public string? LocalPathOriginalFile { get; set; }
    public int? Duration { get; set; }
    public string? ProcessingComplete { get; set; }
}
