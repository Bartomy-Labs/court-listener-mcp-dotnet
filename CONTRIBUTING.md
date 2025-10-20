# Contributing to CourtListener MCP Server

Thank you for your interest in contributing to the CourtListener MCP Server! This document provides guidelines and information for contributors.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Development Workflow](#development-workflow)
- [Commit Message Convention](#commit-message-convention)
- [Pull Request Process](#pull-request-process)
- [Testing Guidelines](#testing-guidelines)
- [Code Style](#code-style)
- [Release Process](#release-process)

## Code of Conduct

This project follows a standard code of conduct. Please be respectful and professional in all interactions.

## Getting Started

### Prerequisites

- **.NET 9 SDK** (latest version)
- **Git** for version control
- **CourtListener API key** ([get one here](https://www.courtlistener.com/help/api/))
- **Visual Studio 2022**, **VS Code**, or **Rider** (recommended IDEs)
- **Docker** (optional, for containerized development)

### Setting Up Your Development Environment

1. Fork the repository on GitHub
2. Clone your fork locally:
   ```bash
   git clone https://github.com/YOUR_USERNAME/court-listener-mcp-dotnet.git
   cd court-listener-mcp
   ```
3. Add the upstream repository as a remote:
   ```bash
   git remote add upstream https://github.com/Bartomy-Labs/court-listener-mcp-dotnet.git
   ```
4. Create a feature branch:
   ```bash
   git checkout -b feat/your-feature-name
   ```

### Building the Project

```bash
# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run all tests
dotnet test
```

### Running the Server

```bash
# Set up your API key (choose one method):

# Option 1: User Secrets (recommended for development)
cd CourtListener.MCP.Server
dotnet user-secrets set "CourtListener:ApiKey" "your-api-key-here"

# Option 2: Environment Variable
export COURTLISTENER_API_KEY="your-api-key-here"  # Linux/macOS
$env:COURTLISTENER_API_KEY="your-api-key-here"    # Windows PowerShell

# Run the server
dotnet run --project CourtListener.MCP.Server
```

## Development Workflow

1. **Sync with upstream** before starting work:
   ```bash
   git fetch upstream
   git rebase upstream/master
   ```

2. **Create a feature branch** using conventional commit prefixes:
   - `feat/` for new features (e.g., `feat/add-docket-search`)
   - `fix/` for bug fixes (e.g., `fix/citation-lookup-timeout`)
   - `docs/` for documentation (e.g., `docs/update-readme`)
   - `test/` for test additions (e.g., `test/add-integration-tests`)
   - `refactor/` for refactoring (e.g., `refactor/simplify-http-client`)

3. **Make your changes** following the [Code Style](#code-style) guidelines

4. **Write tests** for your changes (see [Testing Guidelines](#testing-guidelines))

5. **Commit your changes** using [Conventional Commits](#commit-message-convention)

6. **Push to your fork** and create a pull request

## Commit Message Convention

**IMPORTANT**: This project uses [Conventional Commits](https://www.conventionalcommits.org/) for automated release management. Your commit messages **must** follow this format:

### Format

```
<type>(<scope>): <description>

[optional body]

[optional footer(s)]
```

### Types

- **feat**: A new feature (triggers **minor** version bump: 1.0.0 → 1.1.0)
- **fix**: A bug fix (triggers **patch** version bump: 1.0.0 → 1.0.1)
- **docs**: Documentation changes only
- **test**: Adding or updating tests
- **refactor**: Code changes that neither fix bugs nor add features
- **perf**: Performance improvements
- **build**: Changes to build system or dependencies
- **ci**: Changes to CI/CD configuration
- **chore**: Other changes that don't modify src or test files

### Breaking Changes

For breaking changes that require a **major** version bump (1.0.0 → 2.0.0), add `BREAKING CHANGE:` in the footer or append `!` after the type:

```
feat!: redesign MCP tool API for better error handling

BREAKING CHANGE: All tool methods now return Result<T> instead of throwing exceptions.
```

### Examples

```bash
# Feature addition (minor bump: 1.0.0 → 1.1.0)
git commit -m "feat: add support for RECAP document downloads"

# Bug fix (patch bump: 1.0.0 → 1.0.1)
git commit -m "fix: resolve timeout errors in opinion search"

# Documentation update (no version bump)
git commit -m "docs: update README with MCP Inspector setup"

# Breaking change (major bump: 1.0.0 → 2.0.0)
git commit -m "feat!: require API key in constructor

BREAKING CHANGE: CourtListenerClient now requires API key in constructor instead of configuration"
```

### Multi-line Commits

For complex changes, provide details in the body:

```bash
git commit -m "fix: correct pagination in search results

The pagination was using incorrect offset calculation causing duplicate
results on subsequent pages. This fix adjusts the offset to use proper
page-based calculation.

Fixes #42"
```

## Pull Request Process

1. **Ensure all tests pass** locally before creating a PR:
   ```bash
   dotnet test
   dotnet format --verify-no-changes
   ```

2. **Update documentation** if you've changed:
   - MCP tools → Update tool descriptions and README.md
   - API endpoints → Update server documentation
   - Configuration → Update setup instructions

3. **Create a Pull Request** against the `master` branch with:
   - Clear title using conventional commit format
   - Description of what changed and why
   - Link to any related issues
   - Screenshots/examples if UI/output changed

4. **Required Checks**:
   - ✅ All CI tests must pass
   - ✅ Code formatting verified (`dotnet format`)
   - ✅ Code must build successfully
   - ✅ Branch must be up-to-date with master

5. **Code Review**:
   - Address review feedback promptly
   - Use conventional commits for review fixes
   - Squash fixup commits if requested

6. **Merge**:
   - PRs are squash-merged to maintain clean history
   - Your PR title becomes the commit message (use conventional format!)

## Testing Guidelines

### Test Framework

We use **xUnit** with **Moq** for mocking and **Shouldly** for assertions.

### Test Structure

```csharp
[Fact]
public async Task MethodName_Scenario_ExpectedBehavior()
{
    // Arrange
    var client = CreateTestClient();
    var request = new SearchRequest { Query = "patent" };

    // Act
    var result = await client.SearchOpinions(request);

    // Assert
    result.ShouldNotBeNull();
    result.Results.ShouldNotBeEmpty();
}
```

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test class
dotnet test --filter "FullyQualifiedName~SearchToolsTests"

# Run with code coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Test Coverage

- All new features **must** include tests
- Bug fixes **should** include regression tests
- Aim for >70% code coverage
- MCP tools should have comprehensive integration tests

## Code Style

### General Guidelines

- **C# 13** language features are allowed (.NET 9)
- **Nullable reference types** enabled - handle nulls explicitly
- **Async/await** for all I/O operations
- **Dependency injection** for services
- **XML documentation** required for public APIs
- **Logging** using Serilog for all operations

### Naming Conventions

- **Classes/Methods**: `PascalCase`
- **Private fields**: `_camelCase` with underscore
- **Local variables**: `camelCase`
- **Constants**: `PascalCase`

### Pattern Preferences

```csharp
// ✅ Preferred: Async methods
public async Task<string> SearchOpinionsAsync(
    string query,
    CancellationToken cancellationToken = default)
{
    return await _httpClient.GetStringAsync(url, cancellationToken);
}

// ✅ Preferred: Dependency injection
public class SearchTools
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<SearchTools> _logger;

    public SearchTools(
        IHttpClientFactory httpClientFactory,
        ILogger<SearchTools> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }
}

// ✅ Preferred: Structured logging
_logger.LogInformation(
    "Searching opinions: Query={Query}, Limit={Limit}",
    query, limit);
```

### Code Formatting

- Use `dotnet format` to auto-format code
- EditorConfig settings are enforced in CI
- Follow standard C# conventions

## Release Process

This project uses **Release Please** for automated releases:

1. **Make changes** with conventional commits
2. **Create PR** to `master` branch
3. **Merge PR** after approval and passing CI
4. **Release Please** automatically:
   - Creates a "Release PR" with version bump and changelog
   - Updates version in `.csproj` files
   - Updates CHANGELOG.md
5. **Review and merge** the Release PR when ready to publish
6. **Automated deployment**:
   - GitHub Release is created
   - Docker images are built and published (if configured)
   - Release artifacts are uploaded

### Version Bumping

- `feat:` commits → **minor** version bump (1.0.0 → 1.1.0)
- `fix:` commits → **patch** version bump (1.0.0 → 1.0.1)
- `BREAKING CHANGE:` footer → **major** version bump (1.0.0 → 2.0.0)
- Other types (docs, test, etc.) → no version bump

## Questions?

- Check existing documentation in [README.md](README.md)
- Review [Server README](CourtListener.MCP.Server/README.md) for MCP tool docs
- Open an issue for bugs or feature requests
- Ask questions in pull request discussions

## License

By contributing to the CourtListener MCP Server, you agree that your contributions will be licensed under the MIT License.
