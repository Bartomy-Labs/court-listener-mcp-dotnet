# L.E.A.S.H. Session Context Save

**Generated**: 2025-10-06T15:30:00Z
**Working Directory**: `C:\Users\tlewers\source\repos\court-listener-mcp`
**Session ID**: courtlistener-mcp-plangaps-20251006
**Orchestrator States**: LEASH Planning → PLANGAPS Analysis (Active)

---

## Project Overview

**Mission**: Create a .NET 9 MCP server for the CourtListener Case Law Research API, mirroring the functionality of the existing Python implementation using the latest C# MCP SDK.

**Current Focus**: Conducting systematic gap analysis (/plangaps) to identify and resolve implementation decision points before task generation. Currently on **Gap #4 of 8** - awaiting user decision on MCP Tool Error Handling strategy.

**Architecture**:
- .NET 9 ASP.NET Core application
- C# ModelContextProtocol SDK for MCP server implementation
- HTTP transport at `http://localhost:8000/mcp/`
- CourtListener API v4 integration
- 20+ MCP tools exposing search, retrieval, and citation functionality

**Project Location**: `C:\Users\tlewers\source\repos\court-listener-mcp\`

**Python Reference Implementation**: `C:\Users\tlewers\source\repos\court-listener-mcp-python\`

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
10. **Gaps Identified**: 8 critical implementation gaps requiring pre-implementation decisions

### Gap Decisions Made (3 of 8 completed)

**GAP #1: HTTP Client Retry and Resilience Strategy** ✅ RESOLVED
- **Decision**: Option 1 - Polly with Comprehensive Resilience Policies
- **Impact**: All 20+ tools benefit from retry logic, circuit breaker, rate limiting
- **Details**:
  - 3 retry attempts with exponential backoff (2s, 4s, 8s)
  - Circuit breaker: 5 failures → 60s open circuit
  - Respect 429 rate limits and Retry-After headers
  - Only retry transient errors (5xx, 408, network failures)

**GAP #2: JSON Serialization and Property Naming Strategy** ✅ RESOLVED
- **Decision**: Option 1 - System.Text.Json with Global Snake_Case Naming Policy
- **Impact**: Clean C# model code without per-property attributes
- **Details**:
  - `JsonNamingPolicy.SnakeCaseLower` configured globally
  - Automatic snake_case ↔ PascalCase conversion
  - No `[JsonPropertyName]` attributes needed on properties
  - Built into .NET 9, no extra dependencies

**GAP #3: Logging Framework and Strategy** ✅ RESOLVED
- **Decision**: Option 1 - Serilog with Console and File Sinks
- **User Note**: User confirmed "Let's keep it simple for now" after discussion about Seq
- **Impact**: Structured logging with rotation matching Python loguru functionality
- **Details**:
  - Console sink: colored, formatted for development
  - File sink: JSON format, 1MB rotation, 7-day retention
  - Path: `logs/server.log`
  - Structured properties: Tool, Operation, Duration, StatusCode, Query
  - Enrichers: timestamp, machine name, process ID

**GAP #4: MCP Tool Error Handling and Response Strategy** ⏳ AWAITING USER DECISION
- **Status**: Presented to user with 5 options
- **Recommended**: Option 1 - Structured Error Objects
- **Test Scenarios Defined**:
  1. Not Found (404) - clear error for missing resources
  2. Authentication Failure (401) - API key issues
  3. Rate Limiting (429) - after retries exhausted
  4. Invalid Input - client-side validation errors
  5. Deserialization Failure - unexpected API responses

### Remaining Gaps (4-8, Not Yet Presented)
- Gap #5-8: To be identified and presented after Gap #4 decision

---

## Key Decisions Made

### Technology Stack Decisions
1. **.NET 9** - Latest version chosen over .NET 8 LTS
2. **C# MCP SDK** - Official ModelContextProtocol NuGet packages
3. **ASP.NET Core** - For HTTP transport matching Python's streamable-http
4. **System.Text.Json** - Built-in JSON serializer with global snake_case policy
5. **Serilog** - Structured logging framework (Console + File sinks)
6. **Polly** - Resilience and transient fault handling library
7. **xUnit** - Testing framework (matches Python pytest patterns)

### Implementation Pattern Decisions
1. **Polly Retry Strategy**: 3 attempts, exponential backoff, circuit breaker
2. **JSON Naming**: Global snake_case policy (no per-property attributes)
3. **Logging Strategy**: Console (dev) + File (always), 1MB rotation, 7-day retention

### Rejected Approaches
1. ❌ Newtonsoft.Json - Rejected in favor of built-in System.Text.Json
2. ❌ Seq for Development - Decided to keep logging simple (Option 1 instead of Option 3)
3. ❌ Manual retry logic - Rejected in favor of Polly library
4. ❌ Per-property JsonPropertyName attributes - Global policy is cleaner

---

## Technical Context

### Critical File References

**Plan Document**:
- `.featurePlans/courtListnerMCP/CourtListnerMCPServer.md` - Main implementation plan (updated with gap decisions)

**Python Reference Implementation**:
- `C:\Users\tlewers\source\repos\court-listener-mcp-python\app\server.py` - Python MCP server structure
- `C:\Users\tlewers\source\repos\court-listener-mcp-python\app\config.py` - Configuration patterns
- `C:\Users\tlewers\source\repos\court-listener-mcp-python\app\tools\search.py` - Search tool implementations
- `C:\Users\tlewers\source\repos\court-listener-mcp-python\app\tools\get.py` - Get tool implementations
- `C:\Users\tlewers\source\repos\court-listener-mcp-python\app\tools\citation.py` - Citation tool implementations
- `C:\Users\tlewers\source\repos\court-listener-mcp-python\pyproject.toml` - Python dependencies reference

**L.E.A.S.H. Methodology Files**:
- `C:\GitRepos\Leash\LeashOrchestratorGuide_V6.json` - Planning orchestrator methodology
- `C:\GitRepos\Leash\plangaps.json` - Gap analysis protocol (currently active)
- `C:\GitRepos\Leash\contextsave.json` - Context preservation protocol (currently executing)

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

### External Services

**CourtListener API v4**:
- Purpose: Legal case data, opinions, dockets, citations
- Documentation: https://www.courtlistener.com/api/rest/v4/
- Authentication: API key required (User Secrets or environment variable)

**C# MCP SDK**:
- Repository: https://github.com/modelcontextprotocol/csharp-sdk
- Samples: AspNetCoreMcpServer, QuickstartWeatherServer

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
- ⏳ **Gap #4 IN PROGRESS**: MCP tool error handling (awaiting user decision)
- 🔲 **Gaps #5-8**: Not yet presented (estimated 4-5 more gaps)

### Task Generation Status (INGEST)
- ❌ **NOT STARTED**: Awaiting gap analysis completion
- Next step after plangaps: Use `/ingest` to generate detailed task files

### Implementation Status (HEEL)
- ❌ **NOT STARTED**: No code has been written yet
- All work is pre-implementation planning and decision-making

---

## Decision Audit Trail

### Gap Analysis Decisions (Structured Format)

#### GAP #1: HTTP Client Retry and Resilience Strategy
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

#### GAP #2: JSON Serialization and Property Naming Strategy
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

#### GAP #3: Logging Framework and Strategy
**Category**: Technical Implementation (HIGH)
**Option Chosen**: Option 1 - Serilog with Console and File Sinks
**Rationale**: User preferred simplicity over Seq development UI
**User Quote**: "Let's just keep it simple for now with option 1"
**Test Scenarios Approved**: All 5 scenarios (console debugging, file rotation, API tracing, error context, performance)
**Implementation Constraints**:
- Serilog packages: AspNetCore, Sinks.Console, Sinks.File
- Console sink: colored, readable, development only
- File sink: JSON format, `logs/server.log`, 1MB rotation, 7-day retention
- Structured properties: Tool, Operation, Duration, StatusCode, Query
- Enrichers: timestamp, machine name, process ID, environment

**Critical Anchor**: Serilog with dual sinks matching Python loguru functionality

---

## Architecture Decisions Summary

### Critical Anchors (Immutable Decisions)
1. **.NET 9** - Target framework
2. **CourtListener API Base**: `https://www.courtlistener.com/api/rest/v4/`
3. **Authorization**: `Token {ApiKey}` header format
4. **Endpoint**: `http://localhost:8000/mcp/`
5. **Match Python tool signatures** - All 20+ tools must mirror Python implementation
6. **C# MCP SDK patterns** - Follow official sample patterns
7. **Polly resilience** - All API calls protected with retry + circuit breaker
8. **Global snake_case JSON** - Automatic naming conversion
9. **Serilog structured logging** - Console + file with rotation

### Tool Implementation Requirements
- **Search Tools**: 5 tools (opinions, dockets, dockets_with_documents, recap_documents, audio, people)
- **Get Tools**: 6 tools (opinion, docket, audio, cluster, person, court)
- **Citation Tools**: 6 tools (lookup, batch_lookup, verify_format, parse, extract, enhanced_lookup)
- **System Tools**: 3 tools (status, api_status, health_check)
- **Total**: 20+ MCP tools

### Known Challenges
1. **Citation Parsing**: Python uses `citeurl` library - .NET equivalent needs research (Phase 5)
2. **Citation Tools**: May have limitations vs Python version if no suitable .NET library found

---

## Restoration and Next Steps

### To Resume Session

**Immediate Next Action**:
1. **AWAITING USER DECISION** on Gap #4: MCP Tool Error Handling Strategy
   - User needs to choose Option 1-5
   - Recommended: Option 1 (Structured Error Objects)
   - See Gap #4 details above for options and test scenarios

**After Gap #4 Decision**:
2. Continue gap analysis - present remaining gaps (estimated 4-5 more)
3. Complete all gap analysis decisions
4. Update plan with all gap decisions documented
5. Run `/ingest` to generate detailed task files from updated plan
6. Begin implementation (Phase 1: Project Foundation & Setup)

### Context Restoration Command
Use `/contextload` or manually review this file

### Critical Reminders for Implementation

**DO NOT START CODING** until:
- ✅ All gap analysis decisions completed (currently 3 of ~8)
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
- Implement error handling per Gap #4 (once decided)

### User Preferences Observed
1. Prefers **simplicity** (chose Serilog without Seq)
2. Wants to **match Python functionality** closely
3. Values **clean code** (chose global JSON policy over attributes)
4. Comfortable with **industry-standard libraries** (Polly, Serilog)

---

## Session Statistics

**Total Gaps Identified**: ~8 (estimated)
**Gaps Resolved**: 3
**Gaps Remaining**: ~5
**Plan Updates Made**: 3 (one per gap decision)
**Files Modified**: 1 (`.featurePlans/courtListnerMCP/CourtListnerMCPServer.md`)
**Files Created**: 1 (this context save)
**Code Written**: 0 (pre-implementation phase)

**Session Duration**: ~45 minutes (estimated)
**Current Phase**: Gap Analysis (PLANGAPS)
**Next Milestone**: Complete gap analysis, then task generation (INGEST)

---

## Test Scenarios Defined

### Gap #1: HTTP Retry - Test Scenarios
1. ✅ Transient 503 errors recover after retry with exponential backoff
2. ✅ 429 rate limit responses respect Retry-After header
3. ✅ Timeout after 30s returns clear error message
4. ✅ Permanent errors (404/401) fail immediately without retry
5. ✅ Circuit breaker opens after 5 consecutive failures, auto-recovers after 60s

### Gap #2: JSON Serialization - Test Scenarios
1. ✅ snake_case API responses deserialize to PascalCase C# properties automatically
2. ✅ Request parameters serialize from PascalCase to snake_case query strings
3. ✅ Nested objects deserialize correctly at all depth levels
4. ✅ Null/missing properties handled via nullable types
5. ✅ ISO date strings deserialize to DateTime/DateTimeOffset correctly

### Gap #3: Logging - Test Scenarios
1. ✅ Console shows readable, colored logs during development
2. ✅ Files rotate at 1MB, old logs deleted after 7 days
3. ✅ API requests show complete trace including Polly retries
4. ✅ Errors include context (params, response) without sensitive data
5. ✅ Slow queries logged with timing metrics

### Gap #4: Error Handling - Test Scenarios (PENDING USER DECISION)
1. ⏳ Not Found (404) - clear error for missing resources
2. ⏳ Authentication Failure (401) - API key issues
3. ⏳ Rate Limiting (429) - after retries exhausted
4. ⏳ Invalid Input - client-side validation errors
5. ⏳ Deserialization Failure - unexpected API responses

---

**Context Save Complete** ✅
**Next Session Start**: Resume with Gap #4 decision, then continue gap analysis
