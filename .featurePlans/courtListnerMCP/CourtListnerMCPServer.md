# CourtListener MCP Server - .NET Implementation Plan

**Version**: 1.0
**Created**: 2025-10-05
**Framework**: .NET 9 with C# MCP SDK
**Based On**: Python implementation at `C:\Users\tlewers\source\repos\court-listener-mcp-python`
**Project Location**: `C:\Users\tlewers\source\repos\court-listener-mcp\`

---

## 📋 Project Overview

**Goal**: Create a .NET MCP server for the CourtListener Case Law Research API, mirroring the functionality of the existing Python implementation using the latest C# MCP SDK.

**Source Reference**: [C# MCP SDK](https://github.com/modelcontextprotocol/csharp-sdk)

---

## 🎯 Phase 1: Project Foundation & Setup

### Task 1.1: Create .NET Solution and Project Structure

**Objective**: Initialize the .NET solution with proper project structure

**Agent Instructions**:
- Create new .NET solution `CourtListener.MCP.sln` in `C:\Users\tlewers\source\repos\court-listener-mcp\`
- Create ASP.NET Core project `CourtListener.MCP.Server` targeting .NET 9
- Set up project folders: `/Tools`, `/Services`, `/Models`, `/Configuration`
- Create README.md documenting the project purpose and setup

**Success Criteria**:
- Solution builds successfully
- Project structure matches organizational patterns
- README.md is complete with getting started instructions

**Critical Anchors** (Never violate these):
- Use .NET 9 (latest)
- Follow C# MCP SDK patterns from official samples
- Mirror Python implementation functionality
- ASP.NET Core for HTTP transport (matching Python's streamable-http)

---

### Task 1.2: Install NuGet Packages and Dependencies

**Objective**: Add all required NuGet packages for MCP server and CourtListener integration

**Agent Instructions**:
- Add `ModelContextProtocol` (latest version)
- Add `ModelContextProtocol.AspNetCore` for HTTP transport
- Add `Microsoft.Extensions.Http` for HTTP client
- Add `Microsoft.Extensions.Http.Polly` for resilience policies (GAP #1 decision)
- Add `Microsoft.Extensions.Configuration` for settings
- Add **Serilog** packages for structured logging (GAP #3 decision):
  - `Serilog.AspNetCore`
  - `Serilog.Sinks.Console`
  - `Serilog.Sinks.File`
- Add `System.ComponentModel.DataAnnotations` for validation

**Success Criteria**:
- All packages restore successfully
- No dependency conflicts
- Package versions documented in project file

**Critical Anchors**:
- Use official ModelContextProtocol packages from NuGet
- Match logging patterns to Python's loguru (structured logging)

---

### Task 1.3: Configuration and Settings Setup

**Objective**: Create configuration system for API keys, endpoints, and server settings

**Agent Instructions**:
- Create `appsettings.json` with CourtListener API configuration
- Create `appsettings.Development.json` for dev overrides
- Create `.env.example` file documenting required environment variables
- Implement `CourtListenerOptions` class with:
  - `BaseUrl` (default: "https://www.courtlistener.com/api/rest/v4/")
  - `ApiKey` (from environment/user secrets)
  - `Timeout` (default: 30 seconds)
  - `LogLevel`
- Set up User Secrets for development

**Success Criteria**:
- Configuration loads from appsettings.json and environment variables
- API key never committed to source control
- `.env.example` documents all required settings

**Critical Anchors**:
- API keys must use User Secrets or environment variables only
- Match Python configuration structure
- Base URL: `https://www.courtlistener.com/api/rest/v4/`

---

## 🔧 Phase 2: Core Services & HTTP Client

### Task 2.1: CourtListener HTTP Client Service

**Objective**: Create typed HTTP client service for CourtListener API communication

**Agent Instructions**:
- Add `Microsoft.Extensions.Http.Polly` NuGet package for resilience
- Create `ICourtListenerClient` interface
- Create `CourtListenerClient` implementation with:
  - HTTP client injection via `IHttpClientFactory`
  - Automatic authorization header injection
  - Request/response logging
  - **Polly retry policy**: 3 attempts with exponential backoff (2s, 4s, 8s)
  - **Polly circuit breaker**: 5 failures triggers 60s open circuit
  - Rate limit handling: Respect 429 status and Retry-After header
  - Only retry on transient errors (5xx, 408, network failures)
  - Do NOT retry on permanent errors (404, 401, 403)
  - Generic methods for GET/POST operations
- Register as scoped service with DI
- Add extension method `AddCourtListenerClient()` for service registration with Polly policies

**Success Criteria**:
- HTTP client properly configured with base URL and headers
- Authorization token automatically added to requests
- Polly retry and circuit breaker policies configured
- Service registered in DI container
- **Test Scenarios Pass**:
  - ✅ Transient 503 errors recover after retry with exponential backoff
  - ✅ 429 rate limit responses respect Retry-After header
  - ✅ Timeout after 30s returns clear error message
  - ✅ Permanent errors (404/401) fail immediately without retry
  - ✅ Circuit breaker opens after 5 consecutive failures, auto-recovers after 60s

**Critical Anchors**:
- Use `IHttpClientFactory` for proper HTTP client management
- **Polly resilience policies** (DECISION GAP #1):
  - Retry: 3 attempts, exponential backoff (2s, 4s, 8s)
  - Circuit breaker: 5 failures → 60s open
  - Rate limit aware (429 + Retry-After)
  - Retry only transient errors (5xx, 408, network)
- Authorization header: `Token {ApiKey}` format
- Timeout: 30 seconds default
- Structured logging for all requests/responses

---

### Task 2.2: Response Models and DTOs

**Objective**: Create C# models matching CourtListener API response structures

**Agent Instructions**:
- Configure global `JsonSerializerOptions` with `JsonNamingPolicy.SnakeCaseLower` for automatic snake_case ↔ PascalCase conversion
- Create `/Models` folder structure:
  - `/Search` (OpinionSearchResult, DocketSearchResult, etc.)
  - `/Entities` (Opinion, Docket, Audio, Person, Court, Cluster)
  - `/Citations` (CitationLookupResult, CitationInfo)
  - `/Errors` (ToolError record for structured error responses - GAP #4)
- Implement models with:
  - **PascalCase property names** (e.g., `CaseName`, `DateFiled`, `CitationCount`)
  - **No `[JsonPropertyName]` attributes needed** - global snake_case policy handles conversion
  - Nullable reference types (`string?`, `int?`, etc.)
  - Data annotations for validation
  - `DateTime`/`DateTimeOffset` for date fields
- Create base `SearchResultBase<T>` for paginated responses
- Apply global JSON options to HTTP client configuration

**Success Criteria**:
- All API response structures modeled with clean PascalCase properties
- JSON deserialization works correctly without per-property attributes
- Models match Python implementation data structures
- **Test Scenarios Pass**:
  - ✅ snake_case API responses deserialize to PascalCase C# properties automatically
  - ✅ Request parameters serialize from PascalCase to snake_case query strings
  - ✅ Nested objects deserialize correctly at all depth levels
  - ✅ Null/missing properties handled via nullable types
  - ✅ ISO date strings deserialize to DateTime/DateTimeOffset correctly

**Critical Anchors**:
- Use nullable reference types (`#nullable enable`)
- **System.Text.Json with global snake_case naming policy** (DECISION GAP #2):
  - `JsonNamingPolicy.SnakeCaseLower` configured globally
  - No per-property `[JsonPropertyName]` attributes needed
  - Clean PascalCase C# property names
  - Automatic bidirectional conversion (API ↔ C#)
- Match exact API response structure from Python version
- Use `System.Text.Json` (built into .NET 9, no extra dependencies)

---

## 🔍 Phase 3: Search Tools Implementation

### Task 3.1: Opinion Search Tool

**Objective**: Implement search tool for legal opinions and court decisions

**Agent Instructions**:
- Create error response model classes (DECISION GAP #4):
  - `ToolError` record with properties: `Error` (type string), `Message`, `Suggestion` (optional)
  - Error type constants: `NotFound`, `Unauthorized`, `RateLimited`, `ValidationError`, `ApiError`
- Create `SearchTools.cs` with `[McpServerToolType]` attribute
- Implement `SearchOpinions` method with `[McpServerTool]` attribute (DECISION GAP #5: PascalCase tool naming)
- **Tool name exposed to MCP clients**: `SearchOpinions` (PascalCase, idiomatic C#)
- **No Name attribute needed** - method name becomes tool name
- Parameters: query, court, caseName, judge, filedAfter, filedBefore, citedGt, citedLt, orderBy, limit
  - **Parameter naming**: camelCase C# convention (will serialize to snake_case via global JSON policy)
- Add `[Description]` attribute: "Search legal opinions and court decisions"
- **Input validation** before API call (query not empty, limit > 0)
- Return structured error object on validation failure (don't call API)
- Call CourtListener API: `/search/` with `type=o`
- Map parameters to query string (e.g., `hit` for limit)
- **Error handling**: Return structured error objects (not exceptions) for expected failures:
  - 404: Return `{ Error = "NotFound", Message = "No opinions found matching criteria" }`
  - 401: Return `{ Error = "Unauthorized", Message = "Invalid API key", Suggestion = "Check COURTLISTENER_API_KEY configuration" }`
  - 429: Return `{ Error = "RateLimited", Message = "Rate limit exceeded", Suggestion = "Retry after 60s" }`
  - Validation: Return `{ Error = "ValidationError", Message = "Query parameter cannot be empty" }`

**Success Criteria**:
- Tool callable via MCP protocol
- All search parameters properly mapped to API
- Input validation prevents invalid API calls
- Results properly deserialized and returned
- Errors returned as structured objects (LLM-friendly)
- Logging confirms successful searches
- **Test Scenarios Pass** (GAP #4):
  - ✅ Non-existent resource returns NotFound error with helpful message
  - ✅ Missing API key returns Unauthorized error with configuration hint
  - ✅ Rate limit returns RateLimited error with retry guidance
  - ✅ Invalid input returns ValidationError before API call
  - ✅ Malformed API response returns ApiError with context (logged)

**Critical Anchors**:
- Endpoint: `/api/rest/v4/search/`
- Search type parameter: `type=o` for opinions
- Limit parameter maps to `hit` in API
- Match Python tool functionality (not naming)
- **PascalCase tool naming** (DECISION GAP #5):
  - Tool name: `SearchOpinions` (not `search_opinions`)
  - C# idiomatic naming for .NET ecosystem
  - Method name = tool name (no Name attribute)
  - Parameters: camelCase C# convention
- **Structured error handling** (DECISION GAP #4):
  - Return error objects, NOT throw exceptions
  - Include error type, message, and suggestions
  - Validate input before API calls
  - LLM-friendly error format

---

### Task 3.2: Docket Search Tools

**Objective**: Implement docket search and docket-with-documents search tools

**Agent Instructions**:
- Implement `SearchDockets` method:
  - Parameters: query, court, case_name, docket_number, date_filed_after, date_filed_before, party_name, order_by, limit
  - Search type: `type=d`
- Implement `SearchDocketsWithDocuments` method:
  - Same parameters as SearchDockets
  - Search type: `type=r`
  - Returns dockets with up to 3 nested documents
- Add proper descriptions for both tools

**Success Criteria**:
- Both docket search variations working
- Nested documents properly deserialized
- `more_docs` field properly handled

**Critical Anchors**:
- Regular dockets: `type=d`
- Dockets with documents: `type=r`
- Match Python parameter names and behavior

---

### Task 3.3: Additional Search Tools

**Objective**: Implement RECAP documents, audio, and people search tools

**Agent Instructions**:
- Implement `SearchRecapDocuments`:
  - Type: `type=rd`
  - Additional parameters: document_number, attachment_number
- Implement `SearchAudio`:
  - Type: `type=oa`
  - Parameters: argued_after, argued_before
- Implement `SearchPeople`:
  - Type: `type=p`
  - Parameters: position_type, political_affiliation, school, appointed_by, selection_method

**Success Criteria**:
- All search tools functional
- Type-specific parameters properly handled
- Results match Python implementation

**Critical Anchors**:
- RECAP: `type=rd`
- Audio: `type=oa`
- People: `type=p`

---

## 📄 Phase 4: Entity Retrieval Tools

### Task 4.1: Get Entity Tools

**Objective**: Implement direct entity retrieval by ID

**Agent Instructions**:
- Create `GetTools.cs` with `[McpServerToolType]`
- Implement 6 get methods:
  - `GetOpinion(opinionId)` → `/opinions/{id}/`
  - `GetDocket(docketId)` → `/dockets/{id}/`
  - `GetAudio(audioId)` → `/audio/{id}/`
  - `GetCluster(clusterId)` → `/clusters/{id}/`
  - `GetPerson(personId)` → `/people/{id}/`
  - `GetCourt(courtId)` → `/courts/{id}/`
- Add appropriate descriptions for each
- Handle 404 responses gracefully

**Success Criteria**:
- All 6 entity types retrievable by ID
- Proper error handling for not found
- Response models correctly deserialized

**Critical Anchors**:
- Use GET requests to entity-specific endpoints
- ID must be string type (match Python)
- Return detailed entity information

---

## 📚 Phase 5: Citation Tools

> **Note**: Python version uses `citeurl` library. .NET version will use `CiteUrl.NET` library (separate project being developed in parallel).
> **Dependency**: References `CiteUrl.Core` NuGet package from `C:\Users\tlewers\source\repos\citeurl-dotnet\`

### Task 5.1: Citation Lookup Tools

**Objective**: Implement citation lookup using CourtListener API

**Agent Instructions**:
- Create `CitationTools.cs` with `[McpServerToolType]`
- Implement `LookupCitation(citation)`:
  - POST to `/citation-lookup/`
  - Form data: `text={citation}`
- Implement `BatchLookupCitations(citations)`:
  - Same endpoint, join citations with spaces
  - Max 100 citations
- Handle various citation formats (U.S. Reporter, Federal Reporter, etc.)

**Success Criteria**:
- Single citation lookup works
- Batch lookup handles multiple citations
- Citation formats properly recognized

**Critical Anchors**:
- Endpoint: POST `/citation-lookup/`
- Use form-encoded data: `text={citation}`
- Authorization header required

---

### Task 5.2: Citation Validation and Parsing

**Objective**: Implement citation format verification using CiteUrl.NET library

**Agent Instructions**:
- Add reference to `CiteUrl.Core` NuGet package (local package or published)
- Implement `VerifyCitationFormat(citation)`:
  - Use CiteUrl.NET's Template system for format validation
  - Validate common formats (U.S. Reporter, Federal Reporter, etc.)
  - Return validation results with recognized format
- Implement `ParseCitation(citation)`:
  - Use CiteUrl.NET's Citator.cite() method
  - Parse citation into components (volume, reporter, page, pincite)
  - Return structured citation information

**Success Criteria**:
- Citation format validation working with full CiteUrl.NET capabilities
- All legal citation patterns recognized (130+ sources)
- Full feature parity with Python citeurl library

**Critical Anchors**:
- Use `CiteUrl.Core` library (separate project)
- Match Python validation behavior exactly
- Full Bluebook-style citation support
- **Dependency**: CiteUrl.NET must be available (develop in parallel or use minimal stubs initially)

---

### Task 5.3: Enhanced Citation Tools

**Objective**: Implement citation extraction and enhanced lookup using CiteUrl.NET

**Agent Instructions**:
- Implement `ExtractCitationsFromText(text)`:
  - Use CiteUrl.NET's Citator.list_cites() method
  - Extract all citations from text block using YAML templates
  - Return list of found citations with full parsing
- Implement `EnhancedCitationLookup(citation)`:
  - Combine CiteUrl.NET format validation with CourtListener API lookup
  - Return comprehensive citation information (format + case data)

**Success Criteria**:
- Text extraction finds all citations (130+ legal source types)
- Enhanced lookup provides combined data
- Full feature parity with Python implementation

---

## 🏥 Phase 6: System & Health Tools

### Task 6.1: Status and Health Check Tools

**Objective**: Implement server status and health monitoring tools

**Agent Instructions**:
- Implement `Status()` tool returning:
  - Server status and version
  - System metrics (uptime, memory, CPU)
  - Environment info (runtime, .NET version)
  - Available tools count
  - Transport and endpoint info
- Implement `GetApiStatus()` - check CourtListener API health
- Implement `HealthCheck()` - comprehensive health check
- Use `System.Diagnostics.Process` for metrics

**Success Criteria**:
- Status tool returns comprehensive server info
- API health check validates connectivity
- Metrics accurately reflect server state

**Critical Anchors**:
- Match Python status tool response structure
- Include transport type and port info
- Report CourtListener API base URL

---

## 🚀 Phase 7: Server Configuration & Startup

### Task 7.1: ASP.NET Core MCP Server Setup

**Objective**: Configure ASP.NET Core application to host MCP server

**Agent Instructions**:
- Configure `Program.cs`:
  - Add MCP server with `AddMcpServer()`
  - Configure HTTP transport (match Python's streamable-http)
  - Register all tool classes with `WithToolsFromAssembly()`
  - Configure logging and error handling
- Set default endpoint path: `/mcp/`
- Configure CORS if needed
- Set host: `0.0.0.0`, default port: `8000`

**Success Criteria**:
- Server starts successfully
- MCP endpoint accessible at `http://localhost:8000/mcp/`
- All tools discoverable via MCP protocol
- Logging confirms server initialization

**Critical Anchors**:
- Endpoint: `http://localhost:8000/mcp/`
- Match Python transport configuration
- Use ASP.NET Core dependency injection
- Host: `0.0.0.0` (accept external connections)

---

### Task 7.2: Logging and Error Handling

**Objective**: Implement comprehensive logging and error handling

**Agent Instructions**:
- Configure **Serilog** with two sinks (DECISION GAP #3):
  - **Console sink**: Colored output, formatted for readability, enabled in Development
  - **File sink**: JSON format, always enabled, path: `logs/server.log`
- File sink configuration:
  - JSON structured format
  - Rolling file: 1 MB size limit
  - Retention: 7 days (delete older logs)
  - File name pattern: `server-{Date}.log`
- Structured logging properties to include:
  - `{Tool}` - MCP tool name
  - `{Operation}` - API operation (search, get, etc.)
  - `{Duration}` - Request duration in milliseconds
  - `{StatusCode}` - HTTP status code
  - `{Query}` - Search query (sanitized)
- Log enrichers: timestamp, machine name, process ID, environment
- Minimum log level: Information (configurable via appsettings.json)
- Create global exception handler middleware
- Log all API requests/responses at appropriate levels
- Add request/response timing metrics using Serilog's timing operations
- Redact sensitive data: API keys, full response bodies (truncate to 1000 chars)

**Success Criteria**:
- All operations logged with structured context
- File logging with rotation configured correctly
- Errors properly caught and logged with full context
- Performance metrics captured for all API calls
- **Test Scenarios Pass**:
  - ✅ Console shows readable, colored logs during development
  - ✅ Files rotate at 1MB, old logs deleted after 7 days
  - ✅ API requests show complete trace including Polly retries
  - ✅ Errors include context (params, response) without sensitive data
  - ✅ Slow queries logged with timing metrics

**Critical Anchors**:
- **Serilog with Console and File sinks** (DECISION GAP #3)
- Log path: `logs/server.log` (match Python)
- Rotation: 1 MB size (match Python)
- Retention: 1 week (match Python)
- Structured JSON logging in files
- Console sink: formatted/colored for development
- Enrichment: structured properties for filtering/querying

---

## 🧪 Phase 8: Testing & Documentation

### Task 8.1: Create Test Project

**Objective**: Set up unit and integration testing

**Agent Instructions**:
- Create `CourtListener.MCP.Server.Tests` xUnit project
- Add `Moq` NuGet package for HTTP mocking (DECISION GAP #6)
- Create `TestHelpers` class with shared mock setup methods:
  - `CreateMockHttpClient()` - Returns mock HttpClient
  - `MockSuccessResponse<T>(T data)` - Mock successful API response
  - `MockNotFoundResponse()` - Mock 404 error
  - `MockUnauthorizedResponse()` - Mock 401 error
  - `MockRateLimitResponse(int retryAfterSeconds)` - Mock 429 rate limit
  - `MockServerErrorResponse()` - Mock 500 error
  - `VerifyHttpCall(mockHandler, expectedUrl, expectedMethod)` - Verify HTTP calls made
- Add test categories:
  - Unit tests for search tools (SearchOpinions, SearchDockets, etc.)
  - Unit tests for get tools (GetOpinion, GetDocket, etc.)
  - Unit tests for citation tools (LookupCitation, etc.)
  - Unit tests for error handling (all error types)
  - Unit tests for input validation
  - Integration tests for HTTP client with Polly policies
- All tests use TestHelpers for mock setup (reduce boilerplate)
- Tests run offline (no real API calls)
- Create test data fixtures in code (not separate files)

**Success Criteria**:
- Test project builds and runs
- All tests pass offline (< 1 second total execution)
- Basic test coverage for all tool categories (aim for >70%)
- Mocking framework configured with shared helpers
- **Test Scenarios Pass** (GAP #6):
  - ✅ All tests run offline without internet connection
  - ✅ Polly retry behavior verified with mock 503 responses
  - ✅ All error paths tested (404, 401, 429, 500, validation)
  - ✅ JSON deserialization edge cases covered (nulls, missing fields)
  - ✅ Tests run in parallel without conflicts

**Critical Anchors**:
- Use xUnit test framework
- **Moq + Shared Test Helpers** (DECISION GAP #6):
  - Moq for HttpMessageHandler mocking
  - TestHelpers class for reusable mock setups
  - In-memory mocking (no HTTP server)
  - Fast, isolated, parallel-safe tests
- Mock HTTP responses for offline testing
- Target >70% code coverage

---

### Task 8.2: Documentation and README

**Objective**: Create comprehensive project documentation

**Agent Instructions**:
- Update README.md with:
  - Project purpose and features
  - Installation instructions
  - Configuration guide
  - Usage examples for each tool category
  - API endpoint reference
  - Troubleshooting guide
- Document differences from Python version (especially citation tools)
- Create API documentation with XML comments
- Add code examples for common scenarios

**Success Criteria**:
- README complete and accurate
- All tools documented with examples
- Differences from Python version clearly noted
- Contributing guidelines included

**Critical Anchors**:
- Match Python README structure
- Document citation tool limitations if any
- Include .NET-specific setup steps

---

## 🐳 Phase 9: Deployment & DevOps

### Task 9.1: Docker Support

**Objective**: Create Docker configuration for containerized deployment

**Agent Instructions**:
- Create `Dockerfile` for production deployment
- Create `docker-compose.yml` for easy local testing
- Configure environment variable passing
- Set up health check endpoint
- Optimize image size (multi-stage build)

**Success Criteria**:
- Docker image builds successfully
- Container runs and serves MCP requests
- Environment variables properly injected
- Health check endpoint responsive

**Critical Anchors**:
- Match Python Docker setup patterns
- Use official .NET runtime images
- Port 8000 exposed by default

---

### Task 9.2: CI/CD and Release Preparation

**Objective**: Prepare for automated builds and releases

**Agent Instructions**:
- Create `.github/workflows/build.yml` for CI
- Add automated testing in pipeline
- Configure NuGet package restore caching
- Add code quality checks (linting, formatting)
- Create release notes template

**Success Criteria**:
- GitHub Actions build succeeds
- Tests run automatically on PR
- Code quality gates enforced
- Release workflow documented

**Critical Anchors**:
- Target .NET 9
- Run tests on all pushes
- Enforce code formatting standards

---

## 📊 Implementation Summary

### Total Phases: 9
### Total Tasks: 19

### Key Technical Decisions

1. **MCP SDK**: Official C# ModelContextProtocol packages
2. **Framework**: ASP.NET Core for HTTP transport
3. **API Client**: IHttpClientFactory with typed client
4. **Logging**: Structured logging (Serilog recommended)
5. **Configuration**: User Secrets + appsettings.json
6. **Citation Parsing**: CiteUrl.NET library (separate project, NuGet package reference)

### Known Challenges

- ✅ **Citation parsing** (RESOLVED): Porting Python `citeurl` library to .NET as separate `CiteUrl.NET` project
  - Separate solution: `C:\Users\tlewers\source\repos\citeurl-dotnet\`
  - Will be developed in parallel or before Phase 5
  - MCP server will reference CiteUrl.Core NuGet package
  - Full feature parity expected

### Critical Anchors Applied Throughout

- ✅ CourtListener API Base: `https://www.courtlistener.com/api/rest/v4/`
- ✅ Authorization: `Token {ApiKey}` header format
- ✅ Endpoint: `http://localhost:8000/mcp/`
- ✅ Match Python tool signatures and functionality
- ✅ Use official C# MCP SDK patterns
- ✅ .NET 9 target framework

---

## ✅ Confirmed Decisions

1. **Citation Library**: Port Python `citeurl` library to .NET as separate `CiteUrl.NET` project (GAP #7 - RESOLVED)
   - Separate solution at: `C:\Users\tlewers\source\repos\citeurl-dotnet\`
   - Publish as NuGet package for broader .NET ecosystem
   - MCP server references CiteUrl.Core package
   - Full feature parity with Python version
2. **.NET Version**: Target .NET 9 (latest)
3. **Project Location**: `C:\Users\tlewers\source\repos\court-listener-mcp\`
4. **Transport**: HTTP transport on port 8000 at `/mcp/` endpoint ✓

---

## 📝 Tool Reference from Python Implementation

### Search Tools (5 tools)
- `search_opinions` - Search legal opinions and court decisions
- `search_dockets` - Search court cases and dockets
- `search_dockets_with_documents` - Search dockets with nested documents
- `search_recap_documents` - Search RECAP filing documents
- `search_audio` - Search oral argument audio
- `search_people` - Search judges and legal professionals

### Entity Retrieval Tools (6 tools)
- `get_opinion` - Get specific opinion by ID
- `get_docket` - Get specific docket by ID
- `get_audio` - Get oral argument audio by ID
- `get_cluster` - Get opinion cluster by ID
- `get_person` - Get judge/person by ID
- `get_court` - Get court information by ID

### Citation Tools (6 tools)
- `lookup_citation` - Look up a legal citation
- `batch_lookup_citations` - Batch citation lookup (max 100)
- `verify_citation_format` - Validate citation format
- `parse_citation_with_citeurl` - Parse citation structure
- `extract_citations_from_text` - Extract citations from text
- `enhanced_citation_lookup` - Combined citation analysis

### System Tools (3 tools)
- `status` - Server status and metrics
- `get_api_status` - CourtListener API health
- `health_check` - Comprehensive health check

**Total Tools**: 20+

---

## 📁 Recommended Project Structure

```
CourtListener.MCP/
├── CourtListener.MCP.Server/
│   ├── Configuration/
│   │   ├── CourtListenerOptions.cs
│   │   └── ServiceCollectionExtensions.cs
│   ├── Models/
│   │   ├── Entities/
│   │   │   ├── Opinion.cs
│   │   │   ├── Docket.cs
│   │   │   ├── Audio.cs
│   │   │   ├── Person.cs
│   │   │   ├── Court.cs
│   │   │   └── Cluster.cs
│   │   ├── Search/
│   │   │   ├── SearchResultBase.cs
│   │   │   ├── OpinionSearchResult.cs
│   │   │   ├── DocketSearchResult.cs
│   │   │   └── ...
│   │   └── Citations/
│   │       ├── CitationLookupResult.cs
│   │       └── CitationInfo.cs
│   ├── Services/
│   │   ├── ICourtListenerClient.cs
│   │   └── CourtListenerClient.cs
│   ├── Tools/
│   │   ├── SearchTools.cs
│   │   ├── GetTools.cs
│   │   ├── CitationTools.cs
│   │   └── SystemTools.cs
│   ├── Program.cs
│   ├── appsettings.json
│   └── appsettings.Development.json
├── CourtListener.MCP.Server.Tests/
│   ├── SearchToolsTests.cs
│   ├── GetToolsTests.cs
│   ├── CitationToolsTests.cs
│   └── ...
├── .featurePlans/
│   └── courtListnerMCP/
│       └── CourtListnerMCPServer.md (this file)
├── .env.example
├── Dockerfile
├── docker-compose.yml
├── README.md
└── CourtListener.MCP.sln
```

---

## 🎯 Success Metrics

### Phase Completion Criteria
- ✅ All 20+ tools implemented and functional
- ✅ All tools match Python implementation behavior
- ✅ Comprehensive error handling and logging
- ✅ API client properly configured with authentication
- ✅ Server accessible via HTTP transport at `/mcp/` endpoint
- ✅ Docker deployment working
- ✅ Documentation complete and accurate
- ✅ Test coverage for core functionality

### Performance Targets
- Server startup: < 5 seconds
- API request timeout: 30 seconds
- Response time: < 500ms for cached/simple queries
- Memory usage: < 200MB base

### Quality Standards
- All code follows C# conventions
- XML documentation for public APIs
- Unit test coverage > 70%
- No critical security vulnerabilities
- Structured logging throughout

---

**Plan Status**: Ready for Implementation
**Next Step**: Obtain user approval and answers to questions, then begin Phase 1
