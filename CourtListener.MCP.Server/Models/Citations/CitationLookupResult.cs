namespace CourtListener.MCP.Server.Models.Citations;

/// <summary>
/// Represents the result of a citation lookup request.
/// Properties are PascalCase and automatically serialize/deserialize to snake_case.
/// </summary>
public class CitationLookupResult
{
    /// <summary>
    /// Gets or sets the citation that was looked up.
    /// </summary>
    public string? Citation { get; set; }

    /// <summary>
    /// Gets or sets the list of matches found for the citation.
    /// </summary>
    public List<CitationMatch>? Matches { get; set; }
}

/// <summary>
/// Represents a single match from a citation lookup.
/// Properties are PascalCase and automatically serialize/deserialize to snake_case.
/// </summary>
public class CitationMatch
{
    /// <summary>
    /// Gets or sets the URL to the matched case.
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Gets or sets the name of the matched case.
    /// </summary>
    public string? CaseName { get; set; }

    /// <summary>
    /// Gets or sets the court identifier.
    /// </summary>
    public string? Court { get; set; }

    /// <summary>
    /// Gets or sets the year of the case.
    /// </summary>
    public int? Year { get; set; }
}
