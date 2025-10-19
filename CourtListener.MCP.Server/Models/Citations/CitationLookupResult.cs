namespace CourtListener.MCP.Server.Models.Citations;

/// <summary>
/// Represents the result of a citation lookup request from the CourtListener API.
/// The API returns an array of these objects, one per citation found in the text.
/// Properties are PascalCase and automatically serialize/deserialize to snake_case.
/// </summary>
public class CitationLookupResult
{
    /// <summary>
    /// Gets or sets the citation text that was found and looked up.
    /// </summary>
    public string? Citation { get; set; }

    /// <summary>
    /// Gets or sets the normalized (corrected/standardized) citation formats.
    /// </summary>
    public List<string>? NormalizedCitations { get; set; }

    /// <summary>
    /// Gets or sets the starting character position where the citation was found in the text.
    /// </summary>
    public int StartIndex { get; set; }

    /// <summary>
    /// Gets or sets the ending character position where the citation was found in the text.
    /// </summary>
    public int EndIndex { get; set; }

    /// <summary>
    /// Gets or sets the HTTP status code indicating the result of the lookup.
    /// 200 = Success, 300 = Multiple matches, 429 = Rate limit exceeded.
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// Gets or sets any error message from the lookup. Empty string if successful.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the list of opinion clusters that match this citation.
    /// Each cluster represents a legal opinion/case.
    /// </summary>
    public List<CitationCluster>? Clusters { get; set; }
}

/// <summary>
/// Represents an opinion cluster (case) that matches a citation.
/// Properties are PascalCase and automatically serialize/deserialize to snake_case.
/// </summary>
public class CitationCluster
{
    /// <summary>
    /// Gets or sets the CourtListener ID for this cluster.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the URL to the cluster on CourtListener.
    /// </summary>
    public string? AbsoluteUrl { get; set; }

    /// <summary>
    /// Gets or sets the name of the case.
    /// </summary>
    public string? CaseName { get; set; }

    /// <summary>
    /// Gets or sets the short name of the case.
    /// </summary>
    public string? CaseNameShort { get; set; }

    /// <summary>
    /// Gets or sets the full name of the case.
    /// </summary>
    public string? CaseNameFull { get; set; }

    /// <summary>
    /// Gets or sets the court identifier.
    /// </summary>
    public string? Court { get; set; }

    /// <summary>
    /// Gets or sets the date the case was filed.
    /// </summary>
    public string? DateFiled { get; set; }

    /// <summary>
    /// Gets or sets the Federal Cite 1 reference.
    /// </summary>
    public string? FederalCite1 { get; set; }

    /// <summary>
    /// Gets or sets the Federal Cite 2 reference.
    /// </summary>
    public string? FederalCite2 { get; set; }

    /// <summary>
    /// Gets or sets the Federal Cite 3 reference.
    /// </summary>
    public string? FederalCite3 { get; set; }

    /// <summary>
    /// Gets or sets the state citation reference.
    /// </summary>
    public string? StateCite { get; set; }

    /// <summary>
    /// Gets or sets the neutral citation reference.
    /// </summary>
    public string? NeutralCite { get; set; }

    /// <summary>
    /// Gets or sets the Lexis citation reference.
    /// </summary>
    public string? LexisCite { get; set; }

    /// <summary>
    /// Gets or sets the West citation reference.
    /// </summary>
    public string? WestCite { get; set; }

    /// <summary>
    /// Gets or sets the docket number.
    /// </summary>
    public string? DocketNumber { get; set; }
}
