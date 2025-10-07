namespace CourtListener.MCP.Server.Models;

/// <summary>
/// Generic base class for paginated CourtListener API search results.
/// Properties are PascalCase and automatically serialize/deserialize to snake_case.
/// </summary>
/// <typeparam name="T">The type of entities in the results.</typeparam>
public class SearchResultBase<T>
{
    /// <summary>
    /// Gets or sets the total count of results matching the query.
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// Gets or sets the URL for the next page of results, if available.
    /// </summary>
    public string? Next { get; set; }

    /// <summary>
    /// Gets or sets the URL for the previous page of results, if available.
    /// </summary>
    public string? Previous { get; set; }

    /// <summary>
    /// Gets or sets the list of result entities for the current page.
    /// </summary>
    public List<T> Results { get; set; } = new();
}
