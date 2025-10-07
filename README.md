# CourtListener MCP Server - .NET 9 Implementation

**Version**: 1.0
**Framework**: .NET 9 with C# MCP SDK
**Based On**: Python implementation at `C:\Users\tlewers\source\repos\court-listener-mcp-python`

## 📋 Overview

This is a .NET 9 implementation of a Model Context Protocol (MCP) server for the CourtListener Case Law Research API. The server provides LLM-friendly tools for searching legal opinions, dockets, court audio, people, and citations using the official CourtListener REST API v4.

### Key Features

- 🔍 **Search Tools**: Search opinions, dockets, RECAP documents, oral arguments, and legal professionals
- 📄 **Entity Retrieval**: Get detailed information about specific opinions, dockets, audio, clusters, people, and courts
- 📚 **Citation Tools**: Look up, validate, parse, and extract legal citations
- 🏥 **Health Monitoring**: Server status, API health checks, and system metrics
- 🔒 **Secure Configuration**: User Secrets and environment variable support
- 📊 **Structured Logging**: Serilog with console and file sinks
- 🛡️ **Resilience**: Polly-based retry policies and circuit breakers
- 🐳 **Docker Support**: Containerized deployment ready

## 🎯 Technology Stack

- **.NET 9** - Latest .NET runtime
- **ASP.NET Core** - Web framework with HTTP transport
- **C# MCP SDK** - Official ModelContextProtocol packages
- **Serilog** - Structured logging
- **Polly** - Resilience and transient-fault-handling
- **System.Text.Json** - JSON serialization with snake_case support

## 📦 Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (latest version)
- CourtListener API key ([get one here](https://www.courtlistener.com/help/api/))
- Windows, Linux, or macOS

## 🚀 Getting Started

### Installation

1. **Clone the repository**:
   ```bash
   git clone <repository-url>
   cd court-listener-mcp
   ```

2. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

3. **Configure API key** (choose one method):

   **Option A: User Secrets (Development)**
   ```bash
   cd CourtListener.MCP.Server
   dotnet user-secrets init
   dotnet user-secrets set "CourtListener:ApiKey" "your-api-key-here"
   ```

   **Option B: Environment Variable**
   ```bash
   export COURTLISTENER_API_KEY="your-api-key-here"
   ```

   **Option C: appsettings.json** (not recommended for production)
   ```json
   {
     "CourtListener": {
       "ApiKey": "your-api-key-here"
     }
   }
   ```

4. **Build the project**:
   ```bash
   dotnet build
   ```

5. **Run the server**:
   ```bash
   cd CourtListener.MCP.Server
   dotnet run
   ```

The MCP server will start at `http://localhost:8000/mcp/`

### Docker Deployment

*(Docker support coming in Phase 9)*

## 📖 Documentation

- **API Reference**: [CourtListener REST API v4](https://www.courtlistener.com/api/rest/v4/)
- **Python Version**: `C:\Users\tlewers\source\repos\court-listener-mcp-python`
- **Feature Plan**: `.featurePlans/courtListnerMCP/CourtListnerMCPServer.md`

## 🔧 Development Status

**Current Phase**: Phase 1 - Project Foundation & Setup
**Completed Tasks**: Task 1.1 - Solution and project structure created
**Next Tasks**: See `.featurePlans/courtListnerMCP/` for full task breakdown

### Project Structure

```
court-listener-mcp/
├── CourtListener.MCP.Server/        # Main MCP server project
│   ├── Configuration/               # Settings and options
│   ├── Models/                      # DTOs and entities
│   ├── Services/                    # HTTP client and business logic
│   ├── Tools/                       # MCP tool implementations
│   └── Program.cs                   # Application entry point
├── .featurePlans/                   # Implementation tasks
├── CourtListener.MCP.sln            # Solution file
└── README.md                        # This file
```

## 🧪 Testing

*(Test project coming in Phase 8)*

## 🤝 Contributing

Contributions welcome! This project follows:
- C# coding conventions
- .NET 9 best practices
- MCP SDK patterns from official samples

## 📄 License

*(License to be determined)*

## 🙏 Acknowledgments

- CourtListener team for the excellent legal research API
- Free Law Project for maintaining open legal data
- MCP community for the protocol and SDK
