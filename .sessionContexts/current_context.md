# L.E.A.S.H. Session Context Save - FINAL

**Generated**: 2025-10-06T18:45:00Z
**Working Directory**: `C:\Users\tlewers\source\repos\court-listener-mcp`
**Session ID**: courtlistener-mcp-plangaps-20251006-final
**Orchestrator States**: LEASH Planning → PLANGAPS Analysis (Gap #7 - In Progress) → CONTEXTSAVE

---

## Project Overview

**Mission**: Create a .NET 9 MCP server for the CourtListener Case Law Research API, mirroring the functionality of the existing Python implementation using the latest C# MCP SDK.

**Current Focus**: Gap analysis on **Gap #7: Citation Parsing Library Strategy**. User has cloned the `citeurl` repository and is exploring the feasibility of porting it to .NET C#.

**Architecture**:
- .NET 9 ASP.NET Core application
- C# ModelContextProtocol SDK for MCP server implementation
- HTTP transport at `http://localhost:8000/mcp/`
- CourtListener API v4 integration
- 20+ MCP tools exposing search, retrieval, and citation functionality
- **NEW**: Considering porting `citeurl` Python library to .NET C# for citation parsing

**Project Location**: `C:\Users\tlewers\source\repos\court-listener-mcp\`

**Python Reference Implementation**: `C:\Users\tlewers\source\repos\court-listener-mcp-python\`

**CiteURL Repository (Cloned)**: `C:\Users\tlewers\source\repos\citeurl\`

---

## Conversation Summary - Chronological Milestones

### Session Initialization
1. **User invoked `/leash`** - Adopted L.E.A.S.H. Planning Orchestrator methodology
2. **Initial Request**: Create plan for .NET MCP server using latest .NET MCP SDK for CourtListener API
3. **Context Gathering**: Agent read Python implementation to understand existing functionality
4. **Web Research**: Fetched C# MCP SDK documentation from GitHub

### Planning Phase
5. **Plan Created**: Comprehensive 9-phase, 19-task implementation plan generated
6. **User Answers Provided**:
   - Citation Library: Research .NET alternative during Phase 5
   - .NET Version: Target .NET 9 (latest)
   - Project Location: `C:\Users\tlewers\source\repos\court-listener-mcp\`
   - Transport: HTTP on port 8000 at `/mcp/` endpoint
7. **Plan Saved**: Written to `.featurePlans/courtListnerMCP/CourtListnerMCPServer.md`

### Gap Analysis Phase (ACTIVE)
8. **User invoked `/plangaps`** - Adopted L.E.A.S.H. Plan Gap Analysis Protocol
9. **Gap Analysis Initiated**: Systematic review of plan to identify implementation decision points
10. **Gaps Identified**: 7 critical implementation gaps requiring pre-implementation decisions

### Gap Decisions Made (6 of 7)

**GAP #1: HTTP Client Retry and Resilience Strategy** ✅ RESOLVED
**GAP #2: JSON Serialization and Property Naming Strategy** ✅ RESOLVED
**GAP #3: Logging Framework and Strategy** ✅ RESOLVED
**GAP #4: MCP Tool Error Handling and Response Strategy** ✅ RESOLVED
**GAP #5: MCP Tool Naming Convention** ✅ RESOLVED
**GAP #6: Testing Strategy and HTTP Mocking Approach** ✅ RESOLVED

**GAP #7: Citation Parsing Library Strategy** ⏳ IN PROGRESS
- **Status**: Exploring feasibility of porting `citeurl` to .NET C#
- **Discovery**: User cloned citeurl repository at `C:\Users\tlewers\source\repos\citeurl\`
- **Analysis Started**:
  - License: MIT (confirmed - allows porting)
  - Code size: ~2,188 lines of Python across 19 files
  - Architecture: YAML templates + regex patterns + Python processing
  - Templates: 5 YAML files in `citeurl/templates/` directory
  - Core: Citation parsing, URL building, template system
- **Key Finding**: citeurl is YAML-based with regex patterns that could be ported to C#
- **Next**: Agent was analyzing feasibility when user requested context save

### Context Save Requests
11. **First contextsave**: Mid-session save after Gap #3 completion
12. **Second contextsave**: Final save during Gap #7 exploration

---

## Gap Analysis Decisions - Complete Details

### GAP #1: HTTP Client Retry and Resilience Strategy
**Category**: Integration Patterns (CRITICAL)
**Option Chosen**: Option 1 - Polly with Comprehensive Resilience Policies
**Rationale**: Industry-standard resilience, circuit breaker protection, minimal complexity
**Test Scenarios Approved**: All 5 scenarios (transient failures, rate limiting, timeouts, permanent errors, circuit breaker)
**Implementation Constraints**:
- Must use `Microsoft.Extensions.Http.Polly`
- Retry policy: 3 attempts, exponential backoff (2s, 4s, 8s)
- Circuit breaker: 5 failures triggers 60s open circuit
- Only retry transient errors (5xx, 408, network failures)
- Respect 429 rate limits with Retry-After header

**Critical Anchor**: Polly resilience policies applied to all HTTP client operations

---

### GAP #2: JSON Serialization and Property Naming Strategy
**Category**: Data Architecture (CRITICAL)
**Option Chosen**: Option 1 - System.Text.Json with Global Snake_Case Naming Policy
**Rationale**: Clean code, built-in .NET 9 solution, automatic conversion
**Test Scenarios Approved**: All 5 scenarios (deserialization, request params, nested objects, nulls, date parsing)
**Implementation Constraints**:
- Configure `JsonNamingPolicy.SnakeCaseLower` globally
- C# properties use PascalCase (e.g., `CaseName`, `DateFiled`)
- No `[JsonPropertyName]` attributes needed
- Apply to HTTP client configuration

**Critical Anchor**: Global snake_case naming policy, no per-property attributes

---

### GAP #3: Logging Framework and Strategy
**Category**: Technical Implementation (HIGH)
**Option Chosen**: Option 1 - Serilog with Console and File Sinks
**User Clarification**: "Let's keep it simple for now" - chose Option 1 over Option 3 (Seq)
**Rationale**: User preferred simplicity over Seq development UI
**Test Scenarios Approved**: All 5 scenarios (console debugging, file rotation, API tracing, error context, performance)
**Implementation Constraints**:
- Serilog packages: AspNetCore, Sinks.Console, Sinks.File
- Console sink: colored, readable, development only
- File sink: JSON format, `logs/server.log`, 1MB rotation, 7-day retention
- Structured properties: Tool, Operation, Duration, StatusCode, Query
- Enrichers: timestamp, machine name, process ID, environment

**Critical Anchor**: Serilog with dual sinks matching Python loguru functionality

---

### GAP #4: MCP Tool Error Handling and Response Strategy
**Category**: Technical Implementation (CRITICAL)
**Option Chosen**: Option 1 - Structured Error Objects
**Rationale**: LLM-friendly, consistent, machine-readable
**Test Scenarios Approved**: All 5 scenarios (404, 401, 429, validation, deserialization errors)
**Implementation Constraints**:
- Return error objects, NOT throw exceptions
- `ToolError` record with properties: `Error` (type), `Message`, `Suggestion` (optional)
- Error types: `NotFound`, `Unauthorized`, `RateLimited`, `ValidationError`, `ApiError`
- Validate input before API calls
- Include helpful context and suggestions in error messages

**Critical Anchor**: Structured error handling for LLM-friendly responses

---

### GAP #5: MCP Tool Naming Convention
**Category**: Technical Implementation (HIGH)
**Option Chosen**: Option 2 - C# Idiomatic PascalCase Tool Names
**User Clarification**: "This will be a .NET ecosystem going forward, the python server was just for an example"
**Rationale**: .NET ecosystem, Python was reference only, C# conventions preferred
**Test Scenarios Approved**: All 5 scenarios (tool discovery, invocation, coexistence, documentation, evolution)
**Implementation Constraints**:
- Tool names: PascalCase (e.g., `SearchOpinions`, `GetDocket`, `LookupCitation`)
- Method name = tool name (no Name attribute needed)
- Parameters: camelCase C# convention
- Match functionality (not naming) from Python version

**Critical Anchor**: PascalCase tool naming for C# idiomatic API

---

### GAP #6: Testing Strategy and HTTP Mocking Approach
**Category**: Technical Implementation (MEDIUM)
**Option Chosen**: Option 4 - Moq + Shared Test Helpers
**User Consideration**: Discussed WireMock.Net vs Moq - chose Moq for speed and simplicity
**Rationale**: Fast unit tests, parallel-safe, reduced boilerplate with helpers
**Test Scenarios Approved**: All 5 scenarios (offline tests, Polly verification, error paths, edge cases, parallel execution)
**Implementation Constraints**:
- Add `Moq` NuGet package for HTTP mocking
- Create `TestHelpers` class with reusable mock setup methods
- In-memory mocking (no HTTP server)
- Tests run offline (< 1 second total execution)
- Target >70% code coverage

**Critical Anchor**: Moq + Shared Test Helpers for fast, isolated, parallel-safe tests

---

### GAP #7: Citation Parsing Library Strategy ⏳ IN PROGRESS
**Category**: Technical Implementation (MEDIUM - affects Phase 5)
**Status**: Exploring Option - Port citeurl to .NET C#
**User Action**: Cloned citeurl repository to `C:\Users\tlewers\source\repos\citeurl\`
**User Interest**: "Can we convert this to a .net c# version. that maybe a completely different project but let have a look and see if that is an option."

**Web Search Findings**:
- ❌ No dedicated .NET legal citation parsing libraries exist
- ✅ Python: `citeurl`, `eyecite` (FreeLaw Project)
- ✅ JavaScript: `unitedstates/citation`
- ✅ CiteURL is MIT licensed (allows porting)

**CiteURL Analysis (In Progress)**:
- **License**: MIT (Copyright 2020 Simon Raindrum Sherred)
- **Repository**: https://github.com/raindrum/citeurl
- **Code Size**: ~2,188 lines of Python across 19 files
- **Core Files**:
  - `citator.py` - Main citation processing (Template class)
  - `citation.py` - Citation data structure
  - `authority.py` - Authority handling
  - `tokens.py` - Token processing
  - `regex_mods.py` - Regex modifications
- **Templates**: 5 YAML files with regex patterns:
  - `caselaw.yaml` - Case law citations (U.S. reporters)
  - `general federal law.yaml`
  - `specific federal laws.yaml`
  - `state law.yaml`
  - `secondary sources.yaml`
- **Architecture**:
  - YAML-based template system
  - Regex patterns with token substitution
  - URL builders
  - Name builders
  - Shortform and idform pattern support

**Example from caselaw.yaml**:
```yaml
tokens:
  reporter:
    regex: (long regex with 200+ reporter abbreviations)
  volume: {regex: \d+}
  page: {regex: \d+}
  pincite: {regex: '\d+(-\d+)?'}
pattern: '{volume} {reporter} {page}(,?( at)? {pincite})?'
URL builder: https://case.law/caselaw/?reporter={reporter}&volume={volume}&case={page}-01
```

**Porting Feasibility** (Agent was analyzing when interrupted):
- ✅ **License allows**: MIT permits porting
- ✅ **YAML parsing**: .NET has `YamlDotNet` library
- ✅ **Regex compatibility**: Most regex patterns should work in C#
- ⚠️ **Effort**: ~2,200 lines to port, plus testing
- ⚠️ **Maintenance**: Would need to maintain C# version separately

**Options Presented (before porting discussion)**:
1. Custom Regex-Based Parser (Port citeurl patterns)
2. Minimal Validation Only (No Full Parsing)
3. Python Interop via PythonNET
4. External Service / API Call
5. Defer & Document (Implement Later)

**Agent Recommendation** (before porting idea): Option 2 (Minimal Validation)

**Current Exploration**: User wants to assess feasibility of full citeurl port to C# as potential separate project

**Next Steps for Gap #7**:
1. Complete feasibility analysis of porting citeurl
2. Present comprehensive options including "Port citeurl to .NET" as new option
3. User decides on citation strategy
4. Complete gap analysis
5. Move to task generation

---

## Key Decisions Made

### Technology Stack Decisions
1. **.NET 9** - Latest version chosen over .NET 8 LTS
2. **C# MCP SDK** - Official ModelContextProtocol NuGet packages
3. **ASP.NET Core** - For HTTP transport matching Python's streamable-http
4. **System.Text.Json** - Built-in JSON serializer with global snake_case policy
5. **Serilog** - Structured logging framework (Console + File sinks)
6. **Polly** - Resilience and transient fault handling library
7. **Moq** - HTTP mocking for unit tests with shared test helpers
8. **xUnit** - Testing framework

### Implementation Pattern Decisions
1. **Polly Retry Strategy**: 3 attempts, exponential backoff, circuit breaker
2. **JSON Naming**: Global snake_case policy (no per-property attributes)
3. **Logging Strategy**: Console (dev) + File (always), 1MB rotation, 7-day retention
4. **Error Handling**: Structured error objects (LLM-friendly)
5. **Tool Naming**: PascalCase (C# idiomatic) - .NET ecosystem going forward
6. **Testing**: Moq + shared helpers for fast, isolated tests

### Architectural Clarifications
- **Python version**: Reference example only, not for production compatibility
- **.NET ecosystem**: This is the production version going forward
- **Citation parsing**: Exploring porting citeurl to .NET as potential project

---

## Technical Context

### Critical File References

**Plan Document**:
- `.featurePlans/courtListnerMCP/CourtListnerMCPServer.md` - Main implementation plan (updated with 6 gap decisions)

**Session Contexts**:
- `.sessionContexts/context_20251006_current.md` - Mid-session save (after Gap #3)
- `.sessionContexts/context_20251006_final.md` - This file (during Gap #7)

**Python Reference Implementation**:
- `C:\Users\tlewers\source\repos\court-listener-mcp-python\app\server.py`
- `C:\Users\tlewers\source\repos\court-listener-mcp-python\app\config.py`
- `C:\Users\tlewers\source\repos\court-listener-mcp-python\app\tools\search.py`
- `C:\Users\tlewers\source\repos\court-listener-mcp-python\app\tools\get.py`
- `C:\Users\tlewers\source\repos\court-listener-mcp-python\app\tools\citation.py`
- `C:\Users\tlewers\source\repos\court-listener-mcp-python\pyproject.toml`

**CiteURL Repository** (NEW):
- `C:\Users\tlewers\source\repos\citeurl\` - Cloned repository
- `C:\Users\tlewers\source\repos\citeurl\README.md` - Overview
- `C:\Users\tlewers\source\repos\citeurl\LICENSE.md` - MIT License
- `C:\Users\tlewers\source\repos\citeurl\citeurl\citator.py` - Core citation processing
- `C:\Users\tlewers\source\repos\citeurl\citeurl\templates\caselaw.yaml` - Case law citation patterns
- **Total**: 19 Python files, ~2,188 lines of code

**L.E.A.S.H. Methodology Files**:
- `C:\GitRepos\Leash\LeashOrchestratorGuide_V6.json` - Planning orchestrator
- `C:\GitRepos\Leash\plangaps.json` - Gap analysis protocol (currently active)
- `C:\GitRepos\Leash\contextsave.json` - Context preservation protocol (executing now)

### Configuration Details

**CourtListener API**:
- Base URL: `https://www.courtlistener.com/api/rest/v4/`
- Authentication: `Token {ApiKey}` header format
- Timeout: 30 seconds default
- Rate limiting: Respect 429 status and Retry-After header

**MCP Server Configuration**:
- Endpoint: `http://localhost:8000/mcp/`
- Host: `0.0.0.0` (accept external connections)
- Transport: HTTP (matches Python streamable-http)

**Logging Configuration**:
- Console: Colored output (development only)
- File: `logs/server.log` (JSON format)
- Rotation: 1 MB file size limit
- Retention: 7 days
- Minimum level: Information

**JSON Serialization**:
- Global naming policy: `JsonNamingPolicy.SnakeCaseLower`
- Nullable reference types enabled: `#nullable enable`

### Dependencies (NuGet Packages to Install)

**Core MCP**:
- `ModelContextProtocol` (latest)
- `ModelContextProtocol.AspNetCore`

**HTTP & Resilience**:
- `Microsoft.Extensions.Http`
- `Microsoft.Extensions.Http.Polly`

**Configuration**:
- `Microsoft.Extensions.Configuration`

**Logging**:
- `Serilog.AspNetCore`
- `Serilog.Sinks.Console`
- `Serilog.Sinks.File`

**Validation**:
- `System.ComponentModel.DataAnnotations`

**Testing**:
- `xunit`
- `Moq` (for HTTP mocking)

**Citation Parsing** (If porting citeurl):
- `YamlDotNet` (for YAML template parsing)
- Potentially custom regex library or use built-in `System.Text.RegularExpressions`

### External Services

**CourtListener API v4**:
- Purpose: Legal case data, opinions, dockets, citations
- Documentation: https://www.courtlistener.com/api/rest/v4/
- Authentication: API key required (User Secrets or environment variable)

**C# MCP SDK**:
- Repository: https://github.com/modelcontextprotocol/csharp-sdk
- Samples: AspNetCoreMcpServer, QuickstartWeatherServer

**CiteURL**:
- Repository: https://github.com/raindrum/citeurl
- License: MIT
- Purpose: Legal citation parsing and URL generation
- Status: Python version cloned, exploring .NET port

---

## Current Work State

### Planning Status (LEASH)
- ✅ **COMPLETE**: Initial plan created with 9 phases, 19 tasks
- ✅ **SAVED**: Plan document at `.featurePlans/courtListnerMCP/CourtListnerMCPServer.md`
- ✅ **CONFIRMED**: All user questions answered, technical decisions recorded

### Gap Analysis Status (PLANGAPS) - **ACTIVE**
- ✅ **Gap #1 RESOLVED**: HTTP retry strategy (Polly with circuit breaker)
- ✅ **Gap #2 RESOLVED**: JSON naming strategy (global snake_case policy)
- ✅ **Gap #3 RESOLVED**: Logging framework (Serilog with console + file)
- ✅ **Gap #4 RESOLVED**: Error handling (structured error objects)
- ✅ **Gap #5 RESOLVED**: Tool naming (PascalCase - C# idiomatic)
- ✅ **Gap #6 RESOLVED**: Testing strategy (Moq + shared helpers)
- ⏳ **Gap #7 IN PROGRESS**: Citation parsing strategy (exploring citeurl port)

### Task Generation Status (INGEST)
- ❌ **NOT STARTED**: Awaiting gap analysis completion
- Next step after plangaps: Use `/ingest` to generate detailed task files

### Implementation Status (HEEL)
- ❌ **NOT STARTED**: No code has been written yet
- All work is pre-implementation planning and decision-making

---

## Decision Audit Trail - Gap #7 Details

### GAP #7: Citation Parsing Library Strategy ⏳ IN PROGRESS

**Category**: Technical Implementation (MEDIUM - affects 6 citation tools in Phase 5)

**Why This Matters**: Python version uses `citeurl` for citation parsing. No .NET equivalent exists. Need strategy for implementing citation features.

**Web Search Conducted**:
- Query 1: ".NET C# legal citation parsing library Bluebook format 2024 2025"
- Query 2: "C# parse legal citations case law reporter format library NuGet"
- Query 3: "citeurl .NET alternative citation extraction regex library"

**Finding**: No dedicated .NET legal citation parsing libraries exist

**Options Presented** (before citeurl cloning):
1. Custom Regex-Based Parser (Port citeurl patterns) - Moderate to Complex
2. Minimal Validation Only (No Full Parsing) - Simple, reduced functionality
3. Python Interop via PythonNET - Complex, heavy dependency
4. External Service / API Call - External dependency, network latency
5. Defer & Document (Implement Later) - Simple, incomplete

**Agent Recommendation** (initial): Option 2 - Minimal Validation Only

**User Action**: Asked for citeurl GitHub repo URL, then cloned it locally

**New Discovery**: User exploring porting citeurl to .NET as potential project

**CiteURL Repository Analysis**:
- Location: `C:\Users\tlewers\source\repos\citeurl\`
- License: MIT (allows porting)
- Size: 19 Python files, ~2,188 lines
- Architecture: YAML templates + regex + Python processing
- Templates: 5 YAML files with citation patterns
- Features: Supports 130+ sources of U.S. law, Bluebook-style citations

**Porting Feasibility Considerations**:
- **Pros**:
  - MIT license allows porting
  - YAML templates are language-agnostic
  - Regex patterns mostly compatible with .NET
  - Would provide full citation parsing capability
  - Could become separate reusable .NET library
- **Cons**:
  - Significant development effort (~2,200 lines to port)
  - Need to maintain separate C# version
  - Testing required to ensure compatibility
  - Would be a separate project from MCP server
- **Technical Requirements**:
  - YamlDotNet for YAML parsing
  - System.Text.RegularExpressions for regex
  - Port Python logic to C# idioms
  - Create test suite

**Status at Context Save**:
- Agent was analyzing citeurl structure
- Agent counted files and lines of code
- User interrupted to request context save
- Decision on Gap #7 not yet made

**Next Steps**:
1. Complete feasibility analysis of porting citeurl
2. Present revised Gap #7 options including "Option 6: Port citeurl to .NET C#"
3. User decides on citation strategy:
   - If port: This becomes separate project, MCP server references it
   - If not: Choose from original Options 1-5
4. Complete Gap #7 decision
5. Finalize gap analysis (confirm no more gaps)
6. Move to task generation with `/ingest`

---

## Architecture Decisions Summary

### Critical Anchors (Immutable Decisions)
1. **.NET 9** - Target framework
2. **CourtListener API Base**: `https://www.courtlistener.com/api/rest/v4/`
3. **Authorization**: `Token {ApiKey}` header format
4. **Endpoint**: `http://localhost:8000/mcp/`
5. **Match Python functionality** - Not naming (C# conventions)
6. **C# MCP SDK patterns** - Follow official sample patterns
7. **Polly resilience** - All API calls protected with retry + circuit breaker
8. **Global snake_case JSON** - Automatic naming conversion
9. **Serilog structured logging** - Console + file with rotation
10. **Structured error objects** - LLM-friendly responses
11. **PascalCase tool naming** - C# idiomatic (.NET ecosystem)
12. **Moq + test helpers** - Fast, isolated unit tests

### Tool Implementation Requirements
- **Search Tools**: 5 tools (SearchOpinions, SearchDockets, SearchDocketsWithDocuments, SearchRecapDocuments, SearchAudio, SearchPeople)
- **Get Tools**: 6 tools (GetOpinion, GetDocket, GetAudio, GetCluster, GetPerson, GetCourt)
- **Citation Tools**: 6 tools (LookupCitation, BatchLookupCitations, VerifyCitationFormat, ParseCitation, ExtractCitationsFromText, EnhancedCitationLookup)
  - **NOTE**: Citation tools pending Gap #7 decision
- **System Tools**: 3 tools (Status, GetApiStatus, HealthCheck)
- **Total**: 20+ MCP tools

### Known Challenges
1. **Citation Parsing** (Gap #7 - Active): Python uses `citeurl` library
   - No .NET equivalent exists
   - Exploring porting citeurl to .NET C# as potential project
   - Decision pending

---

## Restoration and Next Steps

### To Resume Session

**Immediate Next Action**:
1. **Complete Gap #7 Analysis**:
   - Finish feasibility assessment of porting citeurl to .NET
   - Present revised options including "Port citeurl" option
   - User decides on citation parsing strategy
2. **Finalize Gap Analysis**:
   - Confirm no additional gaps after Gap #7
   - Update plan with all gap decisions
3. **Generate Tasks**:
   - Run `/ingest` to create detailed task files from updated plan
4. **Begin Implementation**:
   - Phase 1: Project Foundation & Setup

### Context Restoration Command
Use `/contextload` or manually review this file

### Critical Reminders for Implementation

**DO NOT START CODING** until:
- ✅ Gap #7 decision completed
- ✅ All gap analysis decisions finalized
- ✅ Plan updated with all implementation constraints
- ✅ User explicitly approves starting implementation

**When Coding Begins**:
- Create solution at: `C:\Users\tlewers\source\repos\court-listener-mcp\`
- Target .NET 9
- Follow all critical anchors listed above
- Install NuGet packages per Gap decisions
- Configure Polly policies per Gap #1
- Configure JSON serialization per Gap #2
- Configure Serilog per Gap #3
- Implement error handling per Gap #4
- Use PascalCase tool naming per Gap #5
- Create test helpers per Gap #6
- Implement citation strategy per Gap #7 (once decided)

### User Preferences Observed
1. Prefers **simplicity** (chose Serilog without Seq)
2. Wants **.NET ecosystem** (Python was reference only)
3. Values **clean code** (chose global JSON policy over attributes)
4. Comfortable with **industry-standard libraries** (Polly, Serilog, Moq)
5. Values **C# conventions** (PascalCase naming)
6. **Proactive exploration** (cloned citeurl to explore porting option)

### Questions Under Consideration

**Gap #7: Citation Parsing**:
- Should we port citeurl to .NET C# as a separate project?
- If yes: Timeline, scope, integration approach?
- If no: Which simplified approach (Options 1-5)?

---

## Session Statistics

**Total Gaps Identified**: 7
**Gaps Resolved**: 6
**Gaps Remaining**: 1 (Gap #7 - in progress)
**Plan Updates Made**: 6 (one per gap decision)
**Files Modified**: 1 (`.featurePlans/courtListnerMCP/CourtListnerMCPServer.md`)
**Files Created**: 2 context saves
**Code Written**: 0 (pre-implementation phase)

**Session Duration**: ~2.5 hours (estimated)
**Current Phase**: Gap Analysis (PLANGAPS) - Gap #7
**Next Milestone**: Complete Gap #7, finalize gap analysis, task generation (INGEST)

---

## Test Scenarios Defined (Cumulative)

### Gap #1: HTTP Retry - Test Scenarios ✅
1. ✅ Transient 503 errors recover after retry with exponential backoff
2. ✅ 429 rate limit responses respect Retry-After header
3. ✅ Timeout after 30s returns clear error message
4. ✅ Permanent errors (404/401) fail immediately without retry
5. ✅ Circuit breaker opens after 5 consecutive failures, auto-recovers after 60s

### Gap #2: JSON Serialization - Test Scenarios ✅
1. ✅ snake_case API responses deserialize to PascalCase C# properties automatically
2. ✅ Request parameters serialize from PascalCase to snake_case query strings
3. ✅ Nested objects deserialize correctly at all depth levels
4. ✅ Null/missing properties handled via nullable types
5. ✅ ISO date strings deserialize to DateTime/DateTimeOffset correctly

### Gap #3: Logging - Test Scenarios ✅
1. ✅ Console shows readable, colored logs during development
2. ✅ Files rotate at 1MB, old logs deleted after 7 days
3. ✅ API requests show complete trace including Polly retries
4. ✅ Errors include context (params, response) without sensitive data
5. ✅ Slow queries logged with timing metrics

### Gap #4: Error Handling - Test Scenarios ✅
1. ✅ Non-existent resource returns NotFound error with helpful message
2. ✅ Missing API key returns Unauthorized error with configuration hint
3. ✅ Rate limit returns RateLimited error with retry guidance
4. ✅ Invalid input returns ValidationError before API call
5. ✅ Malformed API response returns ApiError with context (logged)

### Gap #5: Tool Naming - Test Scenarios ✅
1. ✅ LLM discovers tools with PascalCase names
2. ✅ LLM invokes tools using exact PascalCase names
3. ✅ Tool names are distinguishable and clear
4. ✅ Documentation examples match actual tool names
5. ✅ New tools follow established PascalCase pattern

### Gap #6: Testing Strategy - Test Scenarios ✅
1. ✅ All tests run offline without internet connection
2. ✅ Polly retry behavior verified with mock 503 responses
3. ✅ All error paths tested (404, 401, 429, 500, validation)
4. ✅ JSON deserialization edge cases covered (nulls, missing fields)
5. ✅ Tests run in parallel without conflicts

### Gap #7: Citation Parsing - Test Scenarios ⏳ PENDING
1. ⏳ Verify common citation format (e.g., "410 U.S. 113")
2. ⏳ Extract citations from text
3. ⏳ Parse citation components (volume, reporter, page)
4. ⏳ Handle edge cases and malformed citations
5. ⏳ Document limitations vs Python version

---

## Web Search Results Archive

### Search #1: Legal Citation Libraries
**Query**: ".NET C# legal citation parsing library Bluebook format 2024 2025"
**Finding**: No dedicated .NET libraries found, only Bluebook style guides

### Search #2: C# Parsing Libraries
**Query**: "C# parse legal citations case law reporter format library NuGet"
**Finding**: General parsing libraries (Sprache, Pidgin), no legal-specific

### Search #3: CiteURL Alternatives
**Query**: "citeurl .NET alternative citation extraction regex library"
**Finding**:
- CiteURL GitHub: https://github.com/raindrum/citeurl
- MIT License
- Python-based, uses YAML templates and regex
- No .NET alternatives exist

---

## CiteURL Repository Details

**Repository**: https://github.com/raindrum/citeurl
**Local Clone**: `C:\Users\tlewers\source\repos\citeurl\`
**License**: MIT (Copyright 2020 Simon Raindrum Sherred)

**Key Features**:
- Parses legal citations and generates links to free law sources
- Supports 130+ sources of U.S. law
- Bluebook-style citation support
- YAML-based template system
- Extensible with custom templates

**Architecture**:
- **Templates** (5 YAML files):
  - `caselaw.yaml` - U.S. case law citations
  - `general federal law.yaml` - Federal statutes, CFR
  - `specific federal laws.yaml` - Specific federal rules
  - `state law.yaml` - State statutes and constitutions
  - `secondary sources.yaml` - Law reviews, etc.

- **Core Python Modules** (19 files, ~2,188 lines):
  - `citator.py` - Template and citation processing engine
  - `citation.py` - Citation data structure
  - `authority.py` - Authority/source handling
  - `tokens.py` - Token type definitions and processing
  - `regex_mods.py` - Regex pattern processing
  - `cli.py` - Command-line interface
  - `web/` - Web application (Flask-based)

**Template Structure Example** (from caselaw.yaml):
```yaml
tokens:
  reporter: {regex: (200+ reporter abbreviations)}
  volume: {regex: \d+}
  page: {regex: \d+}
  pincite: {regex: '\d+(-\d+)?'}
pattern: '{volume} {reporter} {page}(,?( at)? {pincite})?'
URL builder: https://case.law/caselaw/?reporter={reporter}&volume={volume}&case={page}-01
name builder: '{volume} {reporter} {page}, {pincite}'
```

**Porting Considerations**:
- YAML parsing: Use `YamlDotNet` NuGet package
- Regex: Most patterns compatible with .NET `System.Text.RegularExpressions`
- Logic: Port Python class structure to C# classes
- Testing: Create comprehensive test suite
- Scope: Could be separate reusable library project

---

**Context Save Complete** ✅

**Status**: Gap #7 in progress - exploring citeurl port feasibility

**Next Session Resume Point**:
1. Complete citeurl porting feasibility analysis
2. Present revised Gap #7 options to user
3. User decides on citation strategy
4. Complete gap analysis
5. Generate tasks with `/ingest`
