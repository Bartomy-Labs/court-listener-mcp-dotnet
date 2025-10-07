# Phase 4 Boundary Summary - Entity Retrieval Tools

**Phase**: 4 of 9
**Status**: ✅ COMPLETE
**Tasks Completed**: 1 of 1
**Generated**: 2025-10-06T18:55:00Z
**Boundary Type**: Mandatory Stop for User Approval

---

## Phase 4 Overview

**Title**: Entity Retrieval Tools
**Objective**: Implement direct entity retrieval by ID for all 6 entity types
**Complexity**: Moderate (simpler pattern than search tools)

---

## Tasks Completed

### ✅ Task 4.1: Get Entity Tools
**Status**: Generated
**File**: `4.1.json`
**Deliverables**:
- GetTools.cs class with [McpServerToolType] attribute
- 6 get methods: GetOpinion, GetDocket, GetAudio, GetCluster, GetPerson, GetCourt
- Input validation (ID not empty) for all methods
- 404 handling (null result → NotFound error)
- Structured error responses (ToolError objects)
- Structured logging for all operations

**Tool Names (PascalCase)**:
- GetOpinion → `/opinions/{id}/`
- GetDocket → `/dockets/{id}/`
- GetAudio → `/audio/{id}/`
- GetCluster → `/clusters/{id}/`
- GetPerson → `/people/{id}/`
- GetCourt → `/courts/{id}/`

**Pattern Features**:
- Single string parameter: ID
- ID validation before API call
- GET requests to entity-specific endpoints
- Same error handling as search tools
- Return type: object (entity model or ToolError)

---

## Phase 4 Deliverables Summary

**Get Tools Created** (6 total):
```
CourtListener.MCP.Server/Tools/GetTools.cs
├── GetOpinion(opinionId) → Opinion entity
├── GetDocket(docketId) → Docket entity
├── GetAudio(audioId) → Audio entity
├── GetCluster(clusterId) → Cluster entity
├── GetPerson(personId) → Person entity
└── GetCourt(courtId) → Court entity
```

**Common Features**:
- ✅ PascalCase tool naming
- ✅ Single ID parameter (string type)
- ✅ ID validation (not empty)
- ✅ Entity-specific endpoints
- ✅ 404 handling (null → NotFound error)
- ✅ Structured error responses
- ✅ Structured logging

**Build Status**: ✅ `dotnet build` should succeed
**Total MCP Tools**: 12 (6 search + 6 get)

---

## Phase 4 Verification Checklist

Before proceeding to Phase 5, verify:

**GetTools Class**:
- [ ] GetTools.cs exists with [McpServerToolType]
- [ ] All 6 methods have [McpServerTool] attribute
- [ ] All 6 methods have [Description] attributes
- [ ] All ID parameters have [Description] attributes

**Tool Names**:
- [ ] GetOpinion
- [ ] GetDocket
- [ ] GetAudio
- [ ] GetCluster
- [ ] GetPerson
- [ ] GetCourt

**Endpoints**:
- [ ] GetOpinion: `/opinions/{id}/`
- [ ] GetDocket: `/dockets/{id}/`
- [ ] GetAudio: `/audio/{id}/`
- [ ] GetCluster: `/clusters/{id}/`
- [ ] GetPerson: `/people/{id}/`
- [ ] GetCourt: `/courts/{id}/`

**Validation & Error Handling**:
- [ ] ID validation (not empty) for all methods
- [ ] 404 handling (null result → NotFound error with suggestion)
- [ ] 401 handling (Unauthorized with config hint)
- [ ] 429 handling (RateLimited with retry guidance)
- [ ] General error handling (ApiError with logging)

**Logging**:
- [ ] Request logged with entity type and ID
- [ ] Success logged with entity found
- [ ] Not found logged as warning (not error)
- [ ] Errors logged with full context

**Project State**:
- [ ] Project builds successfully
- [ ] Return type: object (entity or ToolError)

---

## Critical Anchors Maintained Throughout Phase 4

1. ✅ PascalCase tool naming (GetOpinion, GetDocket, etc.)
2. ✅ GET requests to entity-specific endpoints
3. ✅ String ID type (matches Python)
4. ✅ Structured error responses (ToolError objects)
5. ✅ 404 graceful handling (null → NotFound)
6. ✅ Structured logging with context
7. ✅ MCP attributes on all tools
8. ✅ Consistent pattern with search tools

---

## Dependencies for Phase 5

Phase 5 (Citation Tools) requires:

**FROM PHASE 4**:
- ✅ GetTools pattern established (can reference for citation tools)
- ✅ Error handling pattern (same for citation tools)

**FROM PHASE 2**:
- ✅ ICourtListenerClient (for POST form requests)
- ✅ Citation models (CitationLookupResult, CitationMatch)

**NEW REQUIREMENT**:
- ⚠️ **CiteUrl.NET library** (GAP #7) - Separate project being developed in parallel
  - Location: `C:\Users\tlewers\source\repos\citeurl-dotnet\`
  - Package: CiteUrl.Core NuGet
  - **Decision**: Tasks 5.2 and 5.3 depend on CiteUrl.NET
  - **Fallback**: Task 5.1 (API-only citation lookup) can proceed without it

**READY**: Phase 4 complete, Phase 5 dependencies mostly satisfied

---

## Gap Decision References

**Fully Implemented**:
- GAP #1: Polly Resilience (Phase 2)
- GAP #2: Global snake_case JSON (Phase 2)
- GAP #3: Serilog Logging (Phases 1-4)
- GAP #4: Structured Error Objects (Phases 3-4)
- GAP #5: PascalCase Tool Naming (Phases 3-4)

**To Be Implemented**:
- GAP #6: Testing Strategy (Phase 8)
- GAP #7: Citation Parsing → **Phase 5 dependency**

---

## Next Phase Preview

**Phase 5: Citation Tools**

**Tasks** (3 tasks):
- Task 5.1: Citation Lookup Tools (API-only, no CiteUrl.NET dependency)
- Task 5.2: Citation Validation and Parsing (requires CiteUrl.NET)
- Task 5.3: Enhanced Citation Tools (requires CiteUrl.NET)

**Key Deliverables**:
- CitationTools.cs with [McpServerToolType]
- LookupCitation, BatchLookupCitations (API-based, POST form data)
- VerifyCitationFormat, ParseCitation (requires CiteUrl.NET)
- ExtractCitationsFromText, EnhancedCitationLookup (requires CiteUrl.NET)

**Dependencies**:
- ✅ ICourtListenerClient (POST form support)
- ✅ Citation models
- ⚠️ **CiteUrl.NET library** (Tasks 5.2, 5.3)

**Note**: Task 5.1 can be implemented independently. Tasks 5.2 and 5.3 depend on CiteUrl.NET availability.

---

## 🛑 MANDATORY PAUSE

**Action Required**: User approval to proceed to Phase 5

**Question**: Phase 4 complete. All 6 entity retrieval tools generated. Ready to proceed to Phase 5 (Citation Tools)?

**Important Note**: Phase 5 tasks 5.2 and 5.3 depend on CiteUrl.NET library. Task 5.1 (API-only citation lookup) can proceed independently.

**Options**:
- `continue to next phase` - Generate Phase 5 tasks (all 3 tasks)
- `review tasks` - Review Phase 4 task files before proceeding
- `stop` - Stop generation, execute Phase 4 tasks first

---

**Generated by**: L.E.A.S.H. Ingestion v3.2.0-git
**Boundary Protocol**: MANDATORY_PHASE_BOUNDARY_PROTOCOL
**Next Action**: Await user approval
