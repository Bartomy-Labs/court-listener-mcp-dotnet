# CourtListener MCP Server - .NET 9 Implementation

**Version**: 1.0
**Framework**: .NET 9 with C# MCP SDK
**Based On**: Python implementation at `C:\Users\tlewers\source\repos\court-listener-mcp-python`

## 🎯 Purpose

The CourtListener MCP Server provides comprehensive access to **legal case data and court opinions** through the extensive CourtListener database. This .NET implementation provides LLM-friendly access to millions of legal opinions from federal and state courts through the official CourtListener API v4.

## 📋 Key Advantages

- **Comprehensive Legal Database:**
  - Access to millions of court opinions and legal decisions
  - Federal and state court coverage
  - Real-time updates from court systems

- **Full Text Content:**
  - Complete opinion text for citation verification
  - Structured legal document organization
  - Rich metadata including judges, courts, and dates

- **Legal Research:**
  - Search by judge, court, case name, or content
  - Verify exact legal language and precedents
  - Validate legal citations and references

- **.NET 9 Performance:**
  - High-performance ASP.NET Core
  - Polly-based resilience and retry policies
  - Structured logging with Serilog
  - Docker-ready deployment

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

## 📦 Installation

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (latest version)
- CourtListener API key ([get one here](https://www.courtlistener.com/help/api/))
- Windows, Linux, or macOS

### Quick Start

```bash
# Clone the repository
git clone <repository-url>
cd court-listener-mcp

# Restore dependencies
dotnet restore

# Configure API key (User Secrets - recommended for development)
cd CourtListener.MCP.Server
dotnet user-secrets init
dotnet user-secrets set "CourtListener:ApiKey" "your-api-key-here"

# Build the project
dotnet build

# Run the server
dotnet run
```

The MCP server will start at `http://localhost:8000/mcp/`

### Environment Configuration

#### Option 1: User Secrets (Development - Recommended)
```bash
cd CourtListener.MCP.Server
dotnet user-secrets init
dotnet user-secrets set "CourtListener:ApiKey" "your-api-key-here"
```

#### Option 2: Environment Variable
```bash
# Windows (Command Prompt)
set COURTLISTENER_API_KEY=your-api-key-here

# Windows (PowerShell)
$env:COURTLISTENER_API_KEY="your-api-key-here"

# Linux/macOS
export COURTLISTENER_API_KEY="your-api-key-here"
```

#### Option 3: appsettings.json (Not Recommended for Production)
```json
{
  "CourtListener": {
    "ApiKey": "your-api-key-here",
    "BaseUrl": "https://www.courtlistener.com/api/rest/v4/",
    "Timeout": 30
  }
}
```

**Important**: Never commit API keys to source control!

### Running the Server

```bash
cd CourtListener.MCP.Server
dotnet run
```

Server endpoints:
- **Host**: `0.0.0.0` (accepts external connections)
- **Port**: `8000`
- **MCP Endpoint**: `http://localhost:8000/mcp/`
- **Transport**: ASP.NET Core HTTP

## 💡 Usage Examples

### Connecting to the Server

When using the HTTP transport, clients can connect using the MCP SDK:

```csharp
// C# client example (using MCP SDK)
using ModelContextProtocol.Client;

var client = new McpClient("http://localhost:8000/mcp/");
var result = await client.CallToolAsync("status");
Console.WriteLine(result);
```

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

#### Verify Citation Format
```json
{
  "tool": "verify_citation_format",
  "arguments": {
    "citation": "347 U.S. 483"
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

## 🐳 Docker Setup

### Build and Run with Docker

```bash
# Build the image
docker build -t courtlistener-mcp-server .

# Run the container
docker run -d -p 8000:8000 \
  -e COURTLISTENER_API_KEY=your-api-key-here \
  --name courtlistener-mcp \
  courtlistener-mcp-server
```

### Docker Compose

```bash
# Start the server
docker-compose up -d

# View logs
docker-compose logs -f

# Stop the server
docker-compose down
```

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test class
dotnet test --filter "FullyQualifiedName~SearchToolsTests"
```

Test coverage: 70%+ across all tool categories

## 🔧 Development

### Build Commands

```bash
# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Build in Release mode
dotnet build -c Release

# Clean build artifacts
dotnet clean
```

### Code Quality

```bash
# Format code
dotnet format

# Run analyzer
dotnet build /p:EnforceCodeStyleInBuild=true
```

### Logging

Logs are written to:
- **Console**: Formatted output for development
- **Files**: JSON format in `logs/server-{Date}.log`
  - Rotation: 1 MB size limit
  - Retention: 7 days

Log levels can be configured in `appsettings.json`:
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    }
  }
}
```

## 🚨 Troubleshooting

### Common Issues

#### API Key Not Found
```
Error: Invalid API key
```
**Solution**: Configure API key using User Secrets or environment variable (see Configuration section)

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
- **.NET**: `PascalCase` for classes/methods, `camelCase` for parameters
  - Tool names: `search_opinions` (exposed to MCP clients as snake_case)
  - C# methods: `SearchOpinions` (internal naming)
  - Parameters: `filedAfter` (C#) → `filed_after` (API)

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

## 📚 Documentation

- **CourtListener API**: https://www.courtlistener.com/api/rest/v4/
- **Model Context Protocol**: https://spec.modelcontextprotocol.io/
- **C# MCP SDK**: https://github.com/modelcontextprotocol/csharp-sdk
- **CiteUrl.NET**: https://github.com/[repository]
- **Serilog**: https://serilog.net/
- **Polly**: https://www.thepollyproject.org/

## 🤝 Contributing

Contributions are welcome! Please follow these guidelines:

### Code Style
- Follow C# coding conventions
- Use nullable reference types
- Add XML documentation comments to public APIs
- Maintain test coverage above 70%

### Pull Request Process
1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Make your changes with tests
4. Run tests and ensure they pass (`dotnet test`)
5. Format code (`dotnet format`)
6. Commit your changes (`git commit -m 'Add amazing feature'`)
7. Push to your branch (`git push origin feature/amazing-feature`)
8. Open a Pull Request

### Testing Requirements
- All new features must include unit tests
- Maintain or improve code coverage
- All tests must pass before PR approval

## 📄 License

This project license is to be determined.

## 🙏 Acknowledgments

- **CourtListener Team** for the excellent legal research API
- **Free Law Project** for maintaining open legal data
- **MCP Community** for the protocol and SDK
- **Python Implementation** at `C:\Users\tlewers\source\repos\court-listener-mcp-python` for reference

---

**Ready to use!** The CourtListener MCP Server provides production-ready access to millions of legal opinions through 21 comprehensive MCP tools powered by .NET 9.

**Need help?** Check the [troubleshooting section](#troubleshooting) or open an issue.
