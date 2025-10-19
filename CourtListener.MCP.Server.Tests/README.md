# CourtListener MCP Server Tests

This directory contains unit and integration tests for the CourtListener MCP Server project.

## Overview

The test suite uses **xUnit** as the testing framework with **Moq** for mocking dependencies and **Shouldly** for fluent assertions. Tests are designed to run offline without requiring access to the CourtListener API.

**Current Test Coverage**: 70%+ across all tool categories

## Test Structure & Organization

### Directory Layout

```
CourtListener.MCP.Server.Tests/
├── Tools/
│   ├── SearchToolsTests.cs     # Tests for search operations
│   ├── SystemToolsTests.cs     # Tests for system/health tools
│   └── (future: CitationToolsTests.cs, EntityToolsTests.cs)
├── TestHelpers.cs              # Shared test utilities and mocking helpers
└── CourtListener.MCP.Server.Tests.csproj
```

### Test Categories

Tests are organized by tool category:

1. **SearchToolsTests** - Tests for opinion, docket, RECAP, audio, and people search
2. **SystemToolsTests** - Tests for status, health check, and API status tools
3. **CitationToolsTests** (planned) - Tests for citation lookup, validation, and parsing
4. **EntityToolsTests** (planned) - Tests for get operations (opinion, docket, court, etc.)

### Naming Conventions

- **Test Classes**: `{ClassName}Tests` (e.g., `SearchToolsTests`)
- **Test Methods**: `{MethodName}_{Scenario}_{ExpectedResult}` (e.g., `SearchOpinions_WithValidQuery_CallsClient`)
- **Test Files**: Match the source file name with `Tests` suffix

## Running Tests

### Run All Tests

```bash
# From solution root
dotnet test

# From test project directory
cd CourtListener.MCP.Server.Tests
dotnet test
```

### Run Specific Test Class

```bash
# Run all tests in SearchToolsTests
dotnet test --filter "FullyQualifiedName~SearchToolsTests"

# Run all tests in SystemToolsTests
dotnet test --filter "FullyQualifiedName~SystemToolsTests"
```

### Run Specific Test Method

```bash
# Run a specific test by name
dotnet test --filter "FullyQualifiedName~SearchToolsTests.SearchOpinions_WithValidQuery_CallsClient"
```

### Run with Detailed Output

```bash
# Verbose output
dotnet test --logger "console;verbosity=detailed"

# Show test names as they run
dotnet test -v n
```

### Run Tests in Watch Mode

```bash
# Automatically re-run tests when files change
dotnet watch test
```

## Test Coverage

### Generate Coverage Report

```bash
# Run tests with code coverage collection
dotnet test --collect:"XPlat Code Coverage"

# Coverage results will be in:
# CourtListener.MCP.Server.Tests/TestResults/{guid}/coverage.cobertura.xml
```

### View Coverage with ReportGenerator

```bash
# Install ReportGenerator tool (once)
dotnet tool install -g dotnet-reportgenerator-globaltool

# Generate HTML coverage report
reportgenerator \
  -reports:"CourtListener.MCP.Server.Tests/TestResults/*/coverage.cobertura.xml" \
  -targetdir:"coveragereport" \
  -reporttypes:Html

# Open the report
start coveragereport/index.html
```

### Coverage Goals

- **Overall**: 70%+ code coverage
- **Tools Classes**: 80%+ coverage (critical business logic)
- **Services**: 70%+ coverage
- **Models**: Not required (primarily data classes)

## Writing New Tests

### Basic Test Structure

```csharp
using CourtListener.MCP.Server.Services;
using CourtListener.MCP.Server.Tools;
using Microsoft.Extensions.Logging;
using Moq;

namespace CourtListener.MCP.Server.Tests.Tools;

public class MyToolTests
{
    private readonly Mock<ICourtListenerClient> _mockClient;
    private readonly Mock<ILogger<MyTool>> _mockLogger;
    private readonly MyTool _myTool;

    public MyToolTests()
    {
        // Arrange: Set up mocks
        _mockClient = new Mock<ICourtListenerClient>();
        _mockLogger = new Mock<ILogger<MyTool>>();
        _myTool = new MyTool(_mockClient.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task MyMethod_WithValidInput_ReturnsSuccess()
    {
        // Arrange
        _mockClient
            .Setup(c => c.GetAsync<MyResult>(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MyResult { Id = 123, Name = "Test" });

        // Act
        var result = await _myTool.MyMethod("test");

        // Assert
        Assert.NotNull(result);
        _mockClient.Verify(c => c.GetAsync<MyResult>(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MyMethod_WithInvalidInput_ReturnsError()
    {
        // Act
        var result = await _myTool.MyMethod("");

        // Assert
        Assert.NotNull(result);
        // Should return error without calling client
        _mockClient.Verify(c => c.GetAsync<MyResult>(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }
}
```

### Test Guidelines

1. **Arrange-Act-Assert Pattern**: Structure tests with clear sections
2. **One Assert Per Test**: Each test should verify one behavior
3. **Descriptive Names**: Test names should clearly describe the scenario
4. **Mock External Dependencies**: Use Moq to mock HTTP client, API calls, etc.
5. **Async/Await**: Use async tests for async methods
6. **Avoid Hard Dependencies**: Tests should not require actual API access

### Testing Common Scenarios

#### Test Successful API Call

```csharp
[Fact]
public async Task GetOpinion_WithValidId_ReturnsOpinion()
{
    // Arrange
    var expectedOpinion = new Opinion { Id = 123, CaseName = "Test v. Case" };
    _mockClient
        .Setup(c => c.GetAsync<Opinion>("opinions/123/", It.IsAny<CancellationToken>()))
        .ReturnsAsync(expectedOpinion);

    // Act
    var result = await _opinionTools.GetOpinion("123");

    // Assert
    Assert.NotNull(result);
    Assert.Equal(expectedOpinion.Id, result.Id);
}
```

#### Test Validation Error

```csharp
[Fact]
public async Task SearchOpinions_WithEmptyQuery_ReturnsValidationError()
{
    // Act
    var result = await _searchTools.SearchOpinions("");

    // Assert
    Assert.NotNull(result);
    // Verify client was never called
    _mockClient.Verify(c => c.GetAsync<object>(
        It.IsAny<string>(),
        It.IsAny<CancellationToken>()), Times.Never);
}
```

#### Test API Error Handling

```csharp
[Fact]
public async Task GetOpinion_WhenNotFound_ReturnsError()
{
    // Arrange
    _mockClient
        .Setup(c => c.GetAsync<Opinion>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
        .ThrowsAsync(new HttpRequestException("Not Found")
        {
            StatusCode = HttpStatusCode.NotFound
        });

    // Act
    var result = await _opinionTools.GetOpinion("99999");

    // Assert
    // Verify error handling behavior
    Assert.NotNull(result);
}
```

## Using TestHelpers

The `TestHelpers` class provides utilities for mocking HTTP responses:

### Mock Success Response

```csharp
var mockHandler = TestHelpers.CreateMockHttpHandler();
TestHelpers.MockSuccessResponse(mockHandler, new { id = 123, name = "Test" });

var client = new HttpClient(mockHandler.Object);
// Use client in tests
```

### Mock Error Responses

```csharp
// 404 Not Found
TestHelpers.MockNotFoundResponse(mockHandler);

// 401 Unauthorized
TestHelpers.MockUnauthorizedResponse(mockHandler);

// 429 Rate Limited
TestHelpers.MockRateLimitedResponse(mockHandler);

// 500 Server Error
TestHelpers.MockServerErrorResponse(mockHandler);

// Network Failure
TestHelpers.MockNetworkFailure(mockHandler);
```

## Testing Patterns

### Mocking ICourtListenerClient

```csharp
// Mock GET request
_mockClient
    .Setup(c => c.GetAsync<MyType>(
        It.Is<string>(s => s.Contains("endpoint")),
        It.IsAny<CancellationToken>()))
    .ReturnsAsync(new MyType { /* test data */ });

// Mock POST request
_mockClient
    .Setup(c => c.PostFormAsync<MyType>(
        It.IsAny<string>(),
        It.IsAny<Dictionary<string, string>>(),
        It.IsAny<CancellationToken>()))
    .ReturnsAsync(new MyType { /* test data */ });

// Verify method was called
_mockClient.Verify(c => c.GetAsync<MyType>(
    It.IsAny<string>(),
    It.IsAny<CancellationToken>()), Times.Once);

// Verify method was NOT called
_mockClient.Verify(c => c.GetAsync<MyType>(
    It.IsAny<string>(),
    It.IsAny<CancellationToken>()), Times.Never);
```

### Testing Offline (No API Calls)

All tests should run offline by mocking the `ICourtListenerClient`:

```csharp
// Good: Uses mock, no real API call
_mockClient
    .Setup(c => c.GetAsync<Opinion>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
    .ReturnsAsync(new Opinion { Id = 123 });

// Bad: Would make real API call (don't do this)
// var client = new HttpClient();
// var result = await client.GetAsync("https://www.courtlistener.com/...");
```

### Using Shouldly (Fluent Assertions)

```csharp
using Shouldly;

// Instead of Assert.NotNull(result)
result.ShouldNotBeNull();

// Instead of Assert.Equal(expected, actual)
actual.ShouldBe(expected);

// More expressive assertions
result.Count.ShouldBeGreaterThan(0);
result.Name.ShouldStartWith("Test");
result.Items.ShouldContain(item => item.Id == 123);
```

## Dependencies

### Test Framework
- **xUnit** (v3.0.0) - Unit testing framework
- **xunit.runner.visualstudio** (v3.1.3) - Visual Studio test runner integration
- **Microsoft.NET.Test.Sdk** (v17.14.1) - .NET test SDK

### Mocking & Assertions
- **Moq** (v4.20.72) - Mocking framework for interfaces and dependencies
- **Shouldly** (v4.3.0) - Fluent assertion library

### Test Project Reference
- **CourtListener.MCP.Server** - Project under test

## Continuous Integration

### GitHub Actions / CI Pipeline

```yaml
# Example .github/workflows/test.yml
name: Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '9.0.x'
      - name: Restore dependencies
        run: dotnet restore
      - name: Build
        run: dotnet build --no-restore
      - name: Test
        run: dotnet test --no-build --verbosity normal --collect:"XPlat Code Coverage"
      - name: Upload coverage
        uses: codecov/codecov-action@v3
        with:
          files: ./CourtListener.MCP.Server.Tests/TestResults/*/coverage.cobertura.xml
```

## Best Practices

1. **Test First**: Consider writing tests before or alongside implementation (TDD)
2. **Keep Tests Fast**: Mock external dependencies to keep tests fast
3. **Independent Tests**: Each test should be independent and not rely on others
4. **Clear Assertions**: Use descriptive assertion messages
5. **Test Edge Cases**: Test boundary conditions, null values, empty strings
6. **Don't Test Framework Code**: Focus on your business logic, not .NET framework
7. **Meaningful Test Data**: Use realistic test data that represents actual usage
8. **Maintain Tests**: Update tests when code changes to keep them relevant

## Troubleshooting

### Tests Fail on Build Server

- Ensure .NET 9 SDK is installed on build server
- Check that all NuGet packages are restored
- Verify no hardcoded paths or environment-specific dependencies

### Tests Timeout

- Check for infinite loops or missing async/await
- Verify mocks are set up correctly (no real HTTP calls)
- Increase timeout if needed: `[Fact(Timeout = 5000)]`

### Flaky Tests

- Avoid test interdependencies
- Don't rely on timing/delays
- Use proper async/await patterns
- Mock all external dependencies consistently

## Additional Resources

- **xUnit Documentation**: https://xunit.net/
- **Moq Documentation**: https://github.com/moq/moq4
- **Shouldly Documentation**: https://docs.shouldly.org/
- **.NET Testing Guide**: https://learn.microsoft.com/en-us/dotnet/core/testing/

---

For server documentation and MCP tool usage, see [CourtListener.MCP.Server/README.md](../CourtListener.MCP.Server/README.md).

For development setup and contributing guidelines, see the [root README](../README.md).
