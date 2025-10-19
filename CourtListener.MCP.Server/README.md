# CourtListener MCP Server Documentation

**Version**: 1.0.0
**Framework**: .NET 9 with C# MCP SDK
**Transport**: ASP.NET Core HTTP

## Overview

The CourtListener MCP Server provides comprehensive access to legal case data and court opinions through the extensive CourtListener database. This implementation provides LLM-friendly access to millions of legal opinions from federal and state courts through the official CourtListener API v4.

## 🛠️ Available MCP Tools

The CourtListener MCP Server provides 21 production-ready tools organized into four categories:

### Opinion & Case Search (6 tools)
- `search_opinions` — Search legal opinions and court decisions
- `search_dockets` — Search court cases and dockets
- `search_dockets_with_documents` — Search dockets with nested documents
- `search_recap_documents` — Search RECAP filing documents
- `search_audio` — Search oral argument audio
- `search_people` — Search judges and legal professionals

### Entity Retrieval (6 tools)
- `get_opinion` — Get specific opinion by ID
- `get_docket` — Get specific docket by ID
- `get_audio` — Get oral argument audio by ID
- `get_cluster` — Get opinion cluster by ID
- `get_person` — Get judge/person by ID
- `get_court` — Get court information by ID

### Citation Tools (6 tools)
- `lookup_citation` — Look up a legal citation via CourtListener API
- `batch_lookup_citations` — Batch citation lookup (max 100 citations)
- `verify_citation_format` — Validate citation format using CiteUrl.NET
- `parse_citation` — Parse citation structure into components
- `extract_citations_from_text` — Extract all citations from text
- `enhanced_citation_lookup` — Combined validation + API lookup

### System & Health (3 tools)
- `status` — Server status, metrics, and configuration
- `get_api_status` — CourtListener API health check
- `health_check` — Comprehensive health monitoring

## 🔌 MCP Client Integration

### Claude Desktop Setup

Add to `claude_desktop_config.json`:

```json
{
  "mcpServers": {
    "courtlistener": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "C:\\path\\to\\CourtListener.MCP.Server\\CourtListener.MCP.Server.csproj",
        "--api-key",
        "your-api-key-here"
      ]
    }
  }
}
```

Restart Claude Desktop and start asking legal research questions!

### Other MCP Clients

When using the HTTP transport, clients can connect using the MCP SDK:

```csharp
// C# client example (using MCP SDK)
using ModelContextProtocol.Client;

var client = new McpClient("http://localhost:8000/mcp/");
var result = await client.CallToolAsync("status");
Console.WriteLine(result);
```

## 💡 Usage Examples

### Search Examples

#### Search Opinions
```json
{
  "tool": "search_opinions",
  "arguments": {
    "query": "habeas corpus",
    "court": "scotus",
    "filed_after": "2020-01-01",
    "limit": 10
  }
}
```

#### Search Dockets
```json
{
  "tool": "search_dockets",
  "arguments": {
    "query": "patent infringement",
    "court": "californiad",
    "date_filed_after": "2023-01-01",
    "limit": 10
  }
}
```

#### Search Dockets with Documents
```json
{
  "tool": "search_dockets_with_documents",
  "arguments": {
    "query": "securities fraud",
    "court": "ca2",
    "limit": 5
  }
}
```

#### Search RECAP Documents
```json
{
  "tool": "search_recap_documents",
  "arguments": {
    "query": "motion to dismiss",
    "court": "nysd",
    "date_filed_after": "2024-01-01",
    "limit": 20
  }
}
```

#### Search Audio (Oral Arguments)
```json
{
  "tool": "search_audio",
  "arguments": {
    "query": "first amendment",
    "court": "scotus",
    "argued_after": "2023-01-01",
    "limit": 10
  }
}
```

#### Search People (Judges)
```json
{
  "tool": "search_people",
  "arguments": {
    "query": "Sotomayor",
    "position_type": "c-jus"
  }
}
```

### Citation Examples

#### Lookup Citation
```json
{
  "tool": "lookup_citation",
  "arguments": {
    "citation": "410 U.S. 113"
  }
}
```

#### Batch Lookup Citations
```json
{
  "tool": "batch_lookup_citations",
  "arguments": {
    "citations": [
      "410 U.S. 113",
      "347 U.S. 483",
      "163 U.S. 537"
    ]
  }
}
```

#### Verify Citation Format
```json
{
  "tool": "verify_citation_format",
  "arguments": {
    "citation": "347 U.S. 483"
  }
}
```

#### Parse Citation
```json
{
  "tool": "parse_citation",
  "arguments": {
    "citation": "410 U.S. 113"
  }
}
```

#### Extract Citations from Text
```json
{
  "tool": "extract_citations_from_text",
  "arguments": {
    "text": "In Brown v. Board of Education, 347 U.S. 483 (1954), the Court held..."
  }
}
```

#### Enhanced Citation Lookup
```json
{
  "tool": "enhanced_citation_lookup",
  "arguments": {
    "citation": "410 U.S. 113"
  }
}
```

### Entity Retrieval Examples

#### Get Opinion by ID
```json
{
  "tool": "get_opinion",
  "arguments": {
    "opinion_id": "12345"
  }
}
```

#### Get Docket by ID
```json
{
  "tool": "get_docket",
  "arguments": {
    "docket_id": "67890"
  }
}
```

#### Get Audio by ID
```json
{
  "tool": "get_audio",
  "arguments": {
    "audio_id": "54321"
  }
}
```

#### Get Cluster by ID
```json
{
  "tool": "get_cluster",
  "arguments": {
    "cluster_id": "98765"
  }
}
```

#### Get Person by ID
```json
{
  "tool": "get_person",
  "arguments": {
    "person_id": "2873"
  }
}
```

#### Get Court Information
```json
{
  "tool": "get_court",
  "arguments": {
    "court_id": "scotus"
  }
}
```

### System Health Examples

#### Server Status
```json
{
  "tool": "status",
  "arguments": {}
}
```

Response:
```json
{
  "Server": "CourtListener MCP Server",
  "Version": "1.0.0",
  "Framework": ".NET 9",
  "Transport": "HTTP",
  "Endpoint": "http://0.0.0.0:8000/mcp/",
  "Status": "Running",
  "Uptime": "2d 5h 37m 42s",
  "Memory": {
    "WorkingSetMB": 156,
    "PrivateMemoryMB": 189
  },
  "Threads": 24,
  "ToolsAvailable": 21,
  "ApiBaseUrl": "https://www.courtlistener.com/api/rest/v4/"
}
```

#### Get API Status
```json
{
  "tool": "get_api_status",
  "arguments": {}
}
```

Response:
```json
{
  "ApiUrl": "https://www.courtlistener.com/api/rest/v4/",
  "Status": "Healthy",
  "ResponseTimeMs": 245,
  "Timestamp": "2024-10-17T12:34:56Z"
}
```

#### Health Check
```json
{
  "tool": "health_check",
  "arguments": {}
}
```

Response:
```json
{
  "Status": "Healthy",
  "Timestamp": "2024-10-17T12:34:56Z",
  "Server": {
    "Status": "Running",
    "Uptime": "2d 5h 37m 42s",
    "MemoryMB": 156
  },
  "Api": {
    "Status": "Healthy",
    "ResponseTimeMs": 245
  },
  "Dependencies": {
    "CourtListenerApi": "Connected",
    "CiteUrlLibrary": "Loaded"
  }
}
```

## 🔍 Testing with MCP Inspector

The **MCP Inspector** is an interactive tool for testing and debugging MCP servers. It provides a web-based interface to explore available tools, test tool calls, and inspect responses.

### Installing MCP Inspector

```bash
# Install globally with npm
npm install -g @modelcontextprotocol/inspector

# Or use npx (no installation required)
npx @modelcontextprotocol/inspector
```

### Connecting to the Server

1. **Start the CourtListener MCP Server**:
   ```bash
   cd CourtListener.MCP.Server
   dotnet run
   ```

   Server will be available at `http://localhost:8000/mcp/`

2. **Launch MCP Inspector**:
   ```bash
   npx @modelcontextprotocol/inspector
   ```

   The inspector will open in your browser (typically `http://localhost:5173`)

3. **Connect to Server**:
   - In the MCP Inspector UI, enter the server URL: `http://localhost:8000/mcp/`
   - Click "Connect"
   - The inspector will discover all 21 available tools

### Using the Inspector

#### Discover Available Tools

Once connected, the inspector displays all tools organized by category:
- **Search Tools** (6): search_opinions, search_dockets, etc.
- **Get Tools** (6): get_opinion, get_docket, etc.
- **Citation Tools** (6): lookup_citation, verify_citation_format, etc.
- **System Tools** (3): status, get_api_status, health_check

#### Test a Tool

**Example: Testing the `status` tool**

1. Select `status` from the tools list
2. No parameters required
3. Click "Execute"
4. Inspect the JSON response:
   ```json
   {
     "Server": "CourtListener MCP Server",
     "Version": "1.0.0",
     "Framework": ".NET 9",
     "Transport": "HTTP",
     "Endpoint": "http://0.0.0.0:8000/mcp/",
     "Status": "Running",
     "Uptime": "0d 0h 5m 32s",
     "ToolsAvailable": 21
   }
   ```

**Example: Testing the `search_opinions` tool**

1. Select `search_opinions` from the tools list
2. Fill in parameters:
   - `query`: "habeas corpus"
   - `court`: "scotus"
   - `limit`: 5
3. Click "Execute"
4. Inspect the search results with opinion metadata

**Example: Testing the `lookup_citation` tool**

1. Select `lookup_citation` from the tools list
2. Fill in parameter:
   - `citation`: "410 U.S. 113"
3. Click "Execute"
4. View the matched case details (Roe v. Wade)

#### Debug Issues

The inspector shows:
- **Request**: Exact JSON sent to the tool
- **Response**: Complete response including errors
- **Timing**: Request/response duration
- **Errors**: Structured error messages with suggestions

**Example Error Response**:
```json
{
  "Error": "Unauthorized",
  "Message": "Invalid API key",
  "Suggestion": "Check COURTLISTENER_API_KEY configuration"
}
```

### Inspector Features

- **Tool Discovery**: Automatically finds all available tools
- **Parameter Validation**: Shows required vs optional parameters
- **Response Inspection**: Pretty-printed JSON with syntax highlighting
- **Error Debugging**: Clear error messages with context
- **Request History**: Review previous tool calls
- **Live Reload**: Reconnects automatically if server restarts

### Troubleshooting with Inspector

#### Inspector Can't Connect

**Problem**: "Failed to connect to http://localhost:8000/mcp/"

**Solutions**:
1. Verify server is running: `curl http://localhost:8000/health`
2. Check server logs for startup errors
3. Ensure port 8000 is not blocked by firewall
4. Try explicit IP: `http://127.0.0.1:8000/mcp/`

#### Tool Returns Error

**Problem**: Tool returns "Unauthorized" error

**Solution**: Configure API key:
```bash
dotnet user-secrets set "CourtListener:ApiKey" "your-api-key-here"
```

**Problem**: Tool returns "ValidationError"

**Solution**: Check parameter formats in inspector:
- Dates must be `YYYY-MM-DD` format
- Limit must be 1-100
- Query parameter cannot be empty

#### No Tools Discovered

**Problem**: Inspector shows "No tools available"

**Solutions**:
1. Verify server started successfully (check console output)
2. Check server logs for tool registration errors
3. Ensure MCP endpoint is `/mcp/` not just `/`
4. Restart server and reconnect inspector

### Testing Workflow

1. **Start Server**: `dotnet run` in CourtListener.MCP.Server
2. **Launch Inspector**: `npx @modelcontextprotocol/inspector`
3. **Connect**: Enter `http://localhost:8000/mcp/`
4. **Test Health**: Run `status` tool to verify connectivity
5. **Test Search**: Try `search_opinions` with a simple query
6. **Test Citation**: Try `lookup_citation` with "410 U.S. 113"
7. **Check Logs**: Review server console for request/response logging
8. **Debug Errors**: Use inspector error messages to troubleshoot

### Inspector vs Production Clients

**MCP Inspector** (Development):
- Interactive web UI
- Manual tool testing
- Visual response inspection
- Development/debugging

**MCP SDK Clients** (Production):
- Programmatic access
- Automated tool calls
- Integration with LLMs
- Production applications

## 🚨 Troubleshooting

### Common Issues

#### API Key Not Found
```
Error: Invalid API key
```
**Solution**: Configure API key using User Secrets or environment variable (see root README Configuration section)

#### Port Already in Use
```
Error: Address already in use 0.0.0.0:8000
```
**Solution**: Change port in `appsettings.json` or environment variable:
```json
{
  "Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://0.0.0.0:8001"
      }
    }
  }
}
```

#### Rate Limiting
```
Error: Rate limit exceeded
```
**Solution**: The server automatically retries with exponential backoff. If persistent, wait 60 seconds or upgrade your CourtListener API plan.

#### SSL/TLS Errors
```
Error: The SSL connection could not be established
```
**Solution**: Ensure .NET 9 runtime is up to date and system certificates are valid.

### Debug Mode

Enable debug logging in `appsettings.Development.json`:
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug"
    }
  }
}
```

## 📊 Differences from Python Version

This .NET implementation maintains feature parity with the Python version while adapting to .NET idioms:

### Architecture
- **Python**: FastMCP framework with streamable-http transport
- **.NET**: ASP.NET Core with ModelContextProtocol SDK

### Citation Tools
- **Python**: Uses `citeurl` Python library for validation and parsing
- **.NET**: Uses `CiteUrl.NET` library (C# port) with identical functionality
  - Full support for 130+ legal citation formats
  - CiteUrl.NET available at: https://github.com/[repository]

### Naming Conventions
- **Python**: `snake_case` for all identifiers
- **.NET**: `PascalCase` for classes/methods, explicit `JsonPropertyName` attributes for API fields
  - Tool names: `search_opinions` (exposed to MCP clients as snake_case)
  - C# methods: `SearchOpinions` (internal naming)
  - C# properties: `PascalCase` (e.g., `DateFiled`, `CaseName`)
  - API fields: Explicit `[JsonPropertyName("date_filed")]` attributes for snake_case API responses
  - JSON serialization: CamelCase base with property-level overrides for accurate API mapping

### Error Handling
- **Python**: Exception-based with custom error classes
- **.NET**: Structured error objects returned from tools
  - Error types: `NotFound`, `Unauthorized`, `RateLimited`, `ValidationError`, `ApiError`
  - LLM-friendly error messages with suggestions

### Resilience
- **Python**: Manual retry logic with exponential backoff
- **.NET**: Polly-based policies with circuit breaker
  - Retry: 3 attempts with exponential backoff (2s, 4s, 8s)
  - Circuit breaker: Opens after 5 failures, auto-recovers after 60s

### Logging
- **Python**: Loguru with colored console output
- **.NET**: Serilog with dual sinks (console + JSON files)
  - Structured logging with enrichment
  - Performance metrics and timing

## 📚 API Documentation

- **CourtListener API**: https://www.courtlistener.com/api/rest/v4/
- **Model Context Protocol**: https://spec.modelcontextprotocol.io/
- **C# MCP SDK**: https://github.com/modelcontextprotocol/csharp-sdk
- **CiteUrl.NET**: https://github.com/[repository]
- **Serilog**: https://serilog.net/
- **Polly**: https://www.thepollyproject.org/

---

For development setup, building, and contributing guidelines, see the [root README](../README.md).

For testing documentation, see [CourtListener.MCP.Server.Tests/README.md](../CourtListener.MCP.Server.Tests/README.md).
