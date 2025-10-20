# CourtListener MCP Server - .NET 9 Implementation

[![Build and Test - Master](https://github.com/Bartomy-Labs/court-listener-mcp-dotnet/actions/workflows/build.yml/badge.svg?branch=master)](https://github.com/Bartomy-Labs/court-listener-mcp-dotnet/actions/workflows/build.yml)
[![Build and Test - Develop](https://github.com/Bartomy-Labs/court-listener-mcp-dotnet/actions/workflows/build.yml/badge.svg?branch=develop)](https://github.com/Bartomy-Labs/court-listener-mcp-dotnet/actions/workflows/build.yml)
[![.NET Version](https://img.shields.io/badge/.NET-9.0-512BD4)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![License](https://img.shields.io/badge/license-MIT-blue)](LICENSE)

**Version**: 1.0.0
**Framework**: .NET 9 with C# MCP SDK
**Based On**: Python implementation at [court-listener-mcp](https://github.com/Travis-Prall/court-listener-mcp)

## 🎯 Purpose

The CourtListener MCP Server provides comprehensive access to **legal case data and court opinions** through the extensive CourtListener database. This .NET implementation provides LLM-friendly access to millions of legal opinions from federal and state courts through the official CourtListener API v4.

## 📋 Key Features

- **21 MCP Tools** across 4 categories (Search, Entity Retrieval, Citations, System)
- **Comprehensive Legal Database** - Access millions of court opinions and legal decisions
- **Full Text Content** - Complete opinion text with structured legal document organization
- **High Performance** - ASP.NET Core with Polly resilience and Serilog logging
- **Docker Ready** - Multi-stage Dockerfile for optimized deployment
- **70%+ Test Coverage** - xUnit tests with Moq mocking

## 📖 Full Documentation

**For detailed MCP tool documentation, usage examples, and testing guides, see:**

- **[CourtListener.MCP.Server/README.md](CourtListener.MCP.Server/README.md)** - Complete MCP tools documentation, usage examples, MCP Inspector testing
- **[CourtListener.MCP.Server.Tests/README.md](CourtListener.MCP.Server.Tests/README.md)** - Testing guide, coverage, and contributing to tests

## 📦 Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (latest version)
- CourtListener API key ([get one here](https://www.courtlistener.com/help/api/))
- Windows, Linux, or macOS

## 🚀 Quick Start

### 1. Clone and Build

```bash
# Clone the repository
git clone <repository-url>
cd court-listener-mcp

# Restore dependencies
dotnet restore

# Build the project
dotnet build
```

### 2. Configure API Key

#### Option 1: User Secrets (Recommended for Development)

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

### 3. Run the Server

```bash
cd CourtListener.MCP.Server
dotnet run
```

Server endpoints:
- **Host**: `0.0.0.0` (accepts external connections)
- **Port**: `8000`
- **MCP Endpoint**: `http://localhost:8000/mcp/`
- **Transport**: ASP.NET Core HTTP

### 4. Test with MCP Inspector

```bash
# Install MCP Inspector (once)
npm install -g @modelcontextprotocol/inspector

# Launch inspector
npx @modelcontextprotocol/inspector

# Connect to: http://localhost:8000/mcp/
```

See [Server README](CourtListener.MCP.Server/README.md#testing-with-mcp-inspector) for detailed testing guide.

## 🔌 MCP Client Setup

### Claude Desktop

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

For detailed MCP client integration (VS Code, etc.), see the [Server README](CourtListener.MCP.Server/README.md#mcp-client-integration).

## 🛠️ Development

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

### Run Tests

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test class
dotnet test --filter "FullyQualifiedName~SearchToolsTests"
```

**Test coverage**: 70%+ across all tool categories

For detailed testing documentation, writing new tests, and coverage reports, see the [Test README](CourtListener.MCP.Server.Tests/README.md).

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

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- Based on Python implementation at [court-listener-mcp](https://github.com/Travis-Prall/court-listener-mcp)
- [Travis Prall](https://github.com/Travis-Prall) for his work creating this MCP Server
- [CourtListener](https://www.courtlistener.com/) team for the excellent legal research API
- [Free Law Project](https://free.law/) for open legal data
- MCP community for protocol and SDK

## 📚 Additional Resources

- **CourtListener API**: https://www.courtlistener.com/api/rest/v4/
- **Model Context Protocol**: https://spec.modelcontextprotocol.io/
- **C# MCP SDK**: https://github.com/modelcontextprotocol/csharp-sdk
- **CiteUrl.NET**: https://github.com/[repository]
- **Serilog**: https://serilog.net/
- **Polly**: https://www.thepollyproject.org/

---

**Ready to use!** The CourtListener MCP Server provides production-ready access to millions of legal opinions through 21 comprehensive MCP tools powered by .NET 9.

**Need help?** Check the [Server README](CourtListener.MCP.Server/README.md#troubleshooting) troubleshooting section or open an issue.
