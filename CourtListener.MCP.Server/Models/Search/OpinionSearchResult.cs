using CourtListener.MCP.Server.Models.Entities;

namespace CourtListener.MCP.Server.Models.Search;

/// <summary>
/// Represents paginated search results for opinions.
/// Search results return OpinionClusters (cases) with nested opinions.
/// </summary>
public class OpinionSearchResult : SearchResultBase<OpinionCluster>
{
}
