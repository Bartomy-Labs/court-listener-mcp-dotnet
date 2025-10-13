# MCP Client Setup Guide

This guide shows how to configure the CourtListener MCP Server in various MCP clients (Claude Desktop, VS Code, etc.).

## Prerequisites

- .NET 9 SDK installed
- CourtListener API key ([get one here](https://www.courtlistener.com/help/api/))
- MCP client (Claude Desktop, VS Code with MCP extension, etc.)

## Configuration Methods

The server supports multiple ways to pass the API key, listed from most to least recommended for MCP clients:

### Option 1: Command-Line Argument (Recommended for MCP Clients)

**Pros**: Explicit, configured directly in MCP client config, portable
**Cons**: API key visible in config file (use appropriate file permissions)

**Syntax**:
```bash
--api-key YOUR_API_KEY_HERE
```

**Example**:
```bash
dotnet run --api-key "your-api-key-here"
```

This is the recommended method for MCP client configurations because the API key is specified directly in the client's config file.

### Option 2: Environment Variable

**Pros**: Secure, works system-wide
**Cons**: Need to set per-session or system-wide, less explicit

**Setup**:

**Windows (PowerShell)**:
```powershell
$env:COURTLISTENER_API_KEY = "your-api-key-here"
```

**Windows (Command Prompt)**:
```cmd
set COURTLISTENER_API_KEY=your-api-key-here
```

**Linux/macOS**:
```bash
export COURTLISTENER_API_KEY=your-api-key-here
```

### Option 3: User Secrets (Development Only)

**Pros**: Secure for local development
**Cons**: Only works locally, not for MCP client deployments

```bash
cd CourtListener.MCP.Server
dotnet user-secrets set "CourtListener:ApiKey" "your-api-key-here"
```

---

## Claude Desktop Configuration

### Method 1: Using Command-Line Argument (Recommended)

**Direct API Key in Config** - No environment variables needed!

**Configuration**:
```json
{
  "mcpServers": {
    "courtlistener": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "C:\\Users\\YourUsername\\source\\repos\\court-listener-mcp\\CourtListener.MCP.Server\\CourtListener.MCP.Server.csproj",
        "--api-key",
        "your-api-key-here"
      ]
    }
  }
}
```

**Or with published build** (faster startup):
```json
{
  "mcpServers": {
    "courtlistener": {
      "command": "C:\\Users\\YourUsername\\source\\repos\\court-listener-mcp\\CourtListener.MCP.Server\\publish\\CourtListener.MCP.Server.exe",
      "args": [
        "--api-key",
        "your-api-key-here"
      ]
    }
  }
}
```

That's it! Just restart Claude Desktop and the server will use your API key.

### Method 2: Using Environment Variable

**Step 1**: Set environment variable system-wide

**Windows**:
1. Search "Environment Variables" in Start menu
2. Click "Environment Variables"
3. Add new **User** variable:
   - Name: `COURTLISTENER_API_KEY`
   - Value: `your-api-key-here`
4. Click OK and restart Claude Desktop

**macOS**:
1. Edit `~/.zshrc` or `~/.bash_profile`:
   ```bash
   export COURTLISTENER_API_KEY="your-api-key-here"
   ```
2. Restart Claude Desktop

**Step 2**: Add to Claude Desktop config

**Location**:
- **Windows**: `%APPDATA%\Claude\claude_desktop_config.json`
- **macOS**: `~/Library/Application Support/Claude/claude_desktop_config.json`

**Configuration**:
```json
{
  "mcpServers": {
    "courtlistener": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "C:\\Users\\YourUsername\\source\\repos\\court-listener-mcp\\CourtListener.MCP.Server\\CourtListener.MCP.Server.csproj"
      ]
    }
  }
}
```

### Method 3: Using Published Build Without API Key in Config

**Step 1**: Publish the server
```bash
cd CourtListener.MCP.Server
dotnet publish -c Release -o publish
```

**Step 2**: Configure Claude Desktop
```json
{
  "mcpServers": {
    "courtlistener": {
      "command": "C:\\Users\\YourUsername\\source\\repos\\court-listener-mcp\\CourtListener.MCP.Server\\publish\\CourtListener.MCP.Server.exe",
      "args": [
        "--CourtListener:ApiKey=your-api-key-here"
      ]
    }
  }
}
```

**Or with environment variable**:
```json
{
  "mcpServers": {
    "courtlistener": {
      "command": "C:\\Users\\YourUsername\\source\\repos\\court-listener-mcp\\CourtListener.MCP.Server\\publish\\CourtListener.MCP.Server.exe",
      "args": []
    }
  }
}
```

---

## VS Code MCP Extension Configuration

**File**: `.vscode/mcp.json`

**Using Command-Line Argument (Recommended)**:
```json
{
  "servers": {
    "courtlistener": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "./CourtListener.MCP.Server/CourtListener.MCP.Server.csproj",
        "--api-key",
        "your-api-key-here"
      ]
    }
  }
}
```

---

## Docker Configuration

### Option 1: Docker Run with Environment Variable
```bash
docker run -d \
  -p 8000:8000 \
  -e COURTLISTENER_API_KEY=your-api-key-here \
  --name courtlistener-mcp \
  courtlistener-mcp-server
```

### Option 2: Docker Compose
```yaml
version: '3.8'
services:
  courtlistener-mcp:
    build: .
    ports:
      - "8000:8000"
    environment:
      - COURTLISTENER_API_KEY=${COURTLISTENER_API_KEY}
```

Then:
```bash
export COURTLISTENER_API_KEY=your-api-key-here
docker-compose up -d
```

---

## Testing the Configuration

### Step 1: Verify API Key is Loaded

**Start the server**:
```bash
cd CourtListener.MCP.Server
dotnet run
```

**Check logs for**:
- `✓ CourtListener MCP Server starting on http://0.0.0.0:8000/mcp/`
- No "Invalid API key" errors

### Step 2: Test with MCP Inspector

```bash
npx @modelcontextprotocol/inspector
```

Connect to `http://localhost:8000/mcp/` and test the `status` tool.

### Step 3: Test a Search Tool

Try `search_opinions` with:
- `query`: "supreme court"
- `limit`: 1

If you get results, the API key is working!

---

## Troubleshooting

### "Invalid API key" Error

**Problem**: Tools return "Unauthorized" error

**Solutions**:
1. Verify API key is set:
   ```bash
   # Windows
   echo %COURTLISTENER_API_KEY%

   # Linux/macOS
   echo $COURTLISTENER_API_KEY
   ```

2. Check server logs for "Authorization: Token YOUR_KEY"

3. Restart the server after setting environment variable

4. Test API key directly:
   ```bash
   curl -H "Authorization: Token your-api-key" \
     https://www.courtlistener.com/api/rest/v4/courts/
   ```

### Server Won't Start

**Problem**: "Unable to bind to http://0.0.0.0:8000"

**Solution**: Port 8000 is in use, change port:
```bash
dotnet run --urls="http://0.0.0.0:8001"
```

Update MCP client config to use port 8001.

### MCP Client Can't Connect

**Problem**: "Failed to connect to MCP server"

**Solutions**:
1. Check server is running: `curl http://localhost:8000/health`
2. Check server logs for errors
3. Verify project path in MCP config is correct
4. Try absolute path instead of relative path

---

## Security Best Practices

1. **Never commit API keys to source control**
2. **Use environment variables** for production deployments
3. **Use User Secrets** for local development
4. **Rotate API keys** periodically
5. **Use read-only API keys** if available
6. **Monitor API usage** on CourtListener dashboard

---

## Example: Complete Claude Desktop Setup

**Step-by-Step**:

1. **Get API Key**: https://www.courtlistener.com/help/api/

2. **Set Environment Variable** (Windows PowerShell):
   ```powershell
   [System.Environment]::SetEnvironmentVariable(
       'COURTLISTENER_API_KEY',
       'your-api-key-here',
       'User'
   )
   ```

3. **Publish Server**:
   ```bash
   cd C:\Users\YourUsername\source\repos\court-listener-mcp\CourtListener.MCP.Server
   dotnet publish -c Release -o publish
   ```

4. **Configure Claude Desktop**:
   - Location: `%APPDATA%\Claude\claude_desktop_config.json`
   ```json
   {
     "mcpServers": {
       "courtlistener": {
         "command": "C:\\Users\\YourUsername\\source\\repos\\court-listener-mcp\\CourtListener.MCP.Server\\publish\\CourtListener.MCP.Server.exe",
         "args": []
       }
     }
   }
   ```

5. **Restart Claude Desktop**

6. **Test in Claude**:
   - Ask: "Search for recent Supreme Court opinions about habeas corpus"
   - Claude will automatically use the `search_opinions` tool

---

## Available Tools in Claude

Once configured, Claude can use these 21 tools:

### Legal Research
- Search opinions, dockets, audio, people
- Get detailed case information
- Look up citations (e.g., "410 U.S. 113")
- Extract citations from text
- Verify citation formats

### Example Queries
- "Find recent Supreme Court decisions about the First Amendment"
- "Look up Roe v. Wade (410 U.S. 113)"
- "Search for cases involving Judge Sotomayor"
- "Extract all citations from this legal brief: [text]"

---

**Ready to integrate!** The CourtListener MCP Server brings comprehensive legal research capabilities to your MCP client.
