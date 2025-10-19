# Release Notes Template

## Version X.Y.Z - YYYY-MM-DD

### Overview

Brief description of this release.

### New Features

- Feature 1: Description of new functionality
- Feature 2: Description of new functionality

### Improvements

- Improvement 1: Description of enhancement
- Improvement 2: Description of enhancement

### Bug Fixes

- Fix 1: Description of bug fix
- Fix 2: Description of bug fix

### Breaking Changes

- Breaking change 1: Description and migration path
- Breaking change 2: Description and migration path

### Dependencies

- Updated dependency X to version Y.Z
- Added new dependency A for feature B

### Documentation

- Updated README with new features
- Added examples for feature X
- Updated API documentation

### Known Issues

- Issue 1: Description and workaround
- Issue 2: Description and workaround

---

## Versioning Strategy

This project follows [Semantic Versioning](https://semver.org/):

- **MAJOR**: Incompatible API changes
- **MINOR**: Backwards-compatible functionality additions
- **PATCH**: Backwards-compatible bug fixes

## Release Checklist

Before releasing a new version:

- [ ] All tests pass (`dotnet test`)
- [ ] Code formatted (`dotnet format`)
- [ ] Documentation updated (README.md, XML comments)
- [ ] RELEASE_NOTES.md updated with changes
- [ ] Version bumped in project file
- [ ] Git tag created (`git tag vX.Y.Z`)
- [ ] Docker image built and tested
- [ ] GitHub release created with notes
- [ ] NuGet packages published (if applicable)

---

# Release History

## Version 1.0.0 - 2025-10-12

### Overview

Initial release of CourtListener MCP Server - .NET 9 implementation.

### Features

- **21 MCP Tools** across 4 categories:
  - 6 Search tools (opinions, dockets, RECAP documents, audio, people)
  - 6 Entity retrieval tools (opinion, docket, audio, cluster, person, court)
  - 6 Citation tools (lookup, batch lookup, verify, parse, extract, enhanced)
  - 3 System tools (status, API status, health check)

- **Architecture**:
  - ASP.NET Core with HTTP transport
  - ModelContextProtocol SDK integration
  - Polly resilience policies (retry, circuit breaker)
  - Serilog structured logging (console + JSON files)

- **Citation Support**:
  - CiteUrl.NET integration
  - 130+ legal citation formats
  - Full feature parity with Python implementation

- **Docker Support**:
  - Multi-stage Dockerfile for optimized images
  - Docker Compose configuration
  - Health check endpoint (/health)

- **Testing**:
  - xUnit test framework
  - Moq for HTTP mocking
  - Offline test execution
  - Foundation for 70%+ coverage

- **Documentation**:
  - Restructured documentation into 3 focused READMEs:
    - Root README: Development setup and quick start
    - Server README: Complete MCP tools documentation and usage
    - Test README: Testing guide and contributing to tests
  - XML documentation on all public APIs
  - Installation and configuration guides
  - Troubleshooting guide
  - Contributing guidelines

### Key Improvements

- **Model Property Types**: Fixed critical type mismatches for accurate API deserialization
  - Changed ID fields from `string?` to `int?` (Opinion, Docket, Person, Cluster, RecapDocument)
  - Fixed `Court.Position` from `int?` to `decimal?` (API returns decimal values)
  - Fixed `Opinion.PerCuriam` from `int?` to `bool?`
  - Fixed `Opinion.JoinedBy` from `string?` to `List<int>?`
  - Fixed `Opinion.PageCount` from `string?` to `int?`
  - Fixed `Cluster.DateFiledIsApproximate` from `DateTimeOffset?` to `bool?`
  - Fixed `Cluster.Panel` from `string?` to `List<string>?`
  - Fixed `RecapDocument` document numbers from `string?` to `int?`

- **CitationLookupResult**: Restructured to match actual CourtListener API response
  - Added `Clusters`, `NormalizedCitations`, `StartIndex`, `EndIndex`, `Status`, `ErrorMessage`
  - Created `CitationCluster` model with complete case metadata
  - Updated citation tools to handle API array responses correctly

- **OpinionSearchResult**: Now returns `OpinionCluster` objects to match API behavior

- **API Key Configuration**: Added logging for troubleshooting
  - Logs last 4 characters when API key is configured
  - Warns when no API key is configured

- **Health Check Endpoint**: Changed from `/` to `courts/?limit=1` for better performance

### Technical Details

- **Framework**: .NET 9
- **Transport**: ASP.NET Core HTTP on port 8000
- **Endpoint**: `http://localhost:8000/mcp/`
- **Configuration**: User Secrets, environment variables, appsettings.json
- **Logging**: Serilog with dual sinks (console + files)
- **Resilience**: Polly with exponential backoff (2s, 4s, 8s)
- **Error Handling**: Structured error objects (LLM-friendly)
- **JSON Serialization**: CamelCase with explicit JsonPropertyName attributes for snake_case API fields
- **Model Accuracy**: Fixed type mismatches across all entity models for proper API deserialization

### Dependencies

- ModelContextProtocol (C# SDK)
- ModelContextProtocol.AspNetCore
- Microsoft.Extensions.Http.Polly
- Serilog.AspNetCore
- Serilog.Sinks.Console
- Serilog.Sinks.File
- CiteUrl.Core (citation parsing)
- xUnit (testing)
- Moq (test mocking)
- Shouldly 4.3.0 (test assertions)

### Migration from Python

See README.md "Differences from Python Version" section for detailed comparison.

Key differences:

- Python: FastMCP framework → .NET: ASP.NET Core
- Python: snake_case → .NET: PascalCase (C#), snake_case (MCP tool names)
- Python: Exception-based errors → .NET: Structured error objects
- Python: Manual retry → .NET: Polly policies
- Python: Loguru → .NET: Serilog

### Known Issues

None at this time.

### Acknowledgments

- Based on Python implementation at [court-listener-mcp](https://github.com/Travis-Prall/court-listener-mcp)
- [Travis Prall](https://github.com/Travis-Prall) for his work creating this MCP Server
- [CourtListener](https://www.courtlistener.com/) team for the excellent legal research API
- [Free Law Project](https://free.law/) for open legal data
- MCP community for protocol and SDK
