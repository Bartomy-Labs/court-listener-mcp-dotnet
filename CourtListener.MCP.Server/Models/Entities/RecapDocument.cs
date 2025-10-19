using System.Text.Json.Serialization;

namespace CourtListener.MCP.Server.Models.Entities;

/// <summary>
/// Represents a RECAP document from federal court dockets.
/// API returns mix of camelCase and snake_case - using attributes for snake_case fields.
/// </summary>
public class RecapDocument
{
    public int? Id { get; set; }

    [JsonPropertyName("absolute_url")]
    public string? AbsoluteUrl { get; set; }

    public int? Docket { get; set; }
    public string? Court { get; set; }

    [JsonPropertyName("document_number")]
    public int? DocumentNumber { get; set; }

    [JsonPropertyName("attachment_number")]
    public int? AttachmentNumber { get; set; }

    public string? Description { get; set; }

    [JsonPropertyName("date_filed")]
    public DateTimeOffset? DateFiled { get; set; }

    [JsonPropertyName("page_count")]
    public int? PageCount { get; set; }

    [JsonPropertyName("file_path")]
    public string? FilePath { get; set; }

    [JsonPropertyName("is_available")]
    public bool? IsAvailable { get; set; }

    [JsonPropertyName("plain_text")]
    public string? PlainText { get; set; }

    public string? Ocr { get; set; }
}
