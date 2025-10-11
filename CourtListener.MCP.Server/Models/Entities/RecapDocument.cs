namespace CourtListener.MCP.Server.Models.Entities;

/// <summary>
/// Represents a RECAP document from federal court dockets.
/// Properties are PascalCase and automatically serialize/deserialize to snake_case.
/// </summary>
public class RecapDocument
{
    public string? Id { get; set; }
    public string? AbsoluteUrl { get; set; }
    public string? Docket { get; set; }
    public string? Court { get; set; }
    public string? DocumentNumber { get; set; }
    public string? AttachmentNumber { get; set; }
    public string? Description { get; set; }
    public DateTimeOffset? DateFiled { get; set; }
    public int? PageCount { get; set; }
    public string? FilePath { get; set; }
    public bool? IsAvailable { get; set; }
    public string? PlainText { get; set; }
    public string? Ocr { get; set; }
}
