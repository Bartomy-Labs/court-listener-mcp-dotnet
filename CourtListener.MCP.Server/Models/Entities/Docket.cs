namespace CourtListener.MCP.Server.Models.Entities;

/// <summary>
/// Represents a court docket from CourtListener.
/// Properties are PascalCase and automatically serialize/deserialize to snake_case.
/// </summary>
public class Docket
{
    public string? Id { get; set; }
    public string? AbsoluteUrl { get; set; }
    public string? Court { get; set; }
    public string? CourtId { get; set; }
    public string? DocketNumber { get; set; }
    public string? CaseName { get; set; }
    public string? CaseNameShort { get; set; }
    public string? CaseNameFull { get; set; }
    public DateTimeOffset? DateFiled { get; set; }
    public DateTimeOffset? DateTerminated { get; set; }
    public DateTimeOffset? DateLastFiling { get; set; }
    public int? AssignedTo { get; set; }
    public string? AssignedToStr { get; set; }
    public int? ReferredTo { get; set; }
    public string? ReferredToStr { get; set; }
    public string? NatureOfSuit { get; set; }
    public string? Cause { get; set; }
    public string? JuryDemand { get; set; }
    public string? JurisdictionType { get; set; }
}
