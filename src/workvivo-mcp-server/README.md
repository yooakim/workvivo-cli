# Workvivo MCP Server

A [Model Context Protocol (MCP)](https://modelcontextprotocol.io) server for the Workvivo API. This server enables AI assistants like Claude, GitHub Copilot, and VS Code to interact with your Workvivo organization data.

## What is MCP?

Model Context Protocol is an open standard that standardizes how AI applications connect to external data sources and tools. Think of it as a universal way to give AI assistants access to your organization's data in a secure, standardized way.

## What Can This Server Do?

The Workvivo MCP server exposes 11 tools that AI assistants can use to help you:

### User Tools
- **ListUsers** - List users with pagination and filtering
- **GetUser** - Get detailed information about a specific user
- **SearchUsersByName** - Find users by name (fuzzy matching)
- **SearchUsersByEmail** - Find users by email address
- **GetAllUsers** - Fetch all users in your organization

### Space Tools
- **ListSpaces** - List spaces with pagination and filtering
- **GetSpace** - Get detailed information about a specific space
- **ListSpaceUsers** - List users in a space (includes roles: owner/admin/member)
- **GetAllSpaceUsers** - Get all users in a specific space
- **GetAllSpaces** - Fetch all spaces in your organization
- **SearchSpacesByName** - Find spaces by name

## Prerequisites

- .NET 10 SDK
- Workvivo API credentials (API Token and Organization ID)
- An MCP-compatible client (VS Code, Claude Desktop, Cursor, etc.)

## Configuration

The MCP server supports three methods of configuration (in order of precedence):

### 1. Environment Variables (Recommended for Production)

```bash
# Windows PowerShell
$env:WORKVIVO_APITOKEN = "your-api-token"
$env:WORKVIVO_ORGANIZATIONID = "your-org-id"
$env:WORKVIVO_BASEURL = "https://api.workvivo.com/v1"

# Linux/macOS
export WORKVIVO_APITOKEN="your-api-token"
export WORKVIVO_ORGANIZATIONID="your-org-id"
export WORKVIVO_BASEURL="https://api.workvivo.com/v1"
```

### 2. User Secrets (Recommended for Local Development)

```bash
dotnet user-secrets set "ApiToken" "your-api-token" --project src/workvivo-mcp-server/WorkvivoMcpServer.csproj
dotnet user-secrets set "OrganizationId" "your-org-id" --project src/workvivo-mcp-server/WorkvivoMcpServer.csproj
dotnet user-secrets set "BaseUrl" "https://api.workvivo.com/v1" --project src/workvivo-mcp-server/WorkvivoMcpServer.csproj
```

### 3. appsettings.json (Not Recommended for Credentials)

Edit `appsettings.json` (but never commit credentials to version control):

```json
{
  "ApiToken": "your-api-token",
  "OrganizationId": "your-org-id",
  "BaseUrl": "https://api.workvivo.com/v1"
}
```

## Installation

### VS Code / GitHub Copilot

1. Copy `mcp.json.template` from the repository root to `.vscode/mcp.json`
2. Update the credentials in `.vscode/mcp.json`
3. Restart VS Code
4. Open GitHub Copilot Agent Mode
5. Click the tools selector - you should see "workvivo" with 11 tools

### Claude Desktop

Add this to your Claude Desktop configuration file:

**Windows**: `%APPDATA%\Claude\claude_desktop_config.json`
**macOS**: `~/Library/Application Support/Claude/claude_desktop_config.json`

```json
{
  "mcpServers": {
    "workvivo": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "C:/path/to/workvivo-cli/src/workvivo-mcp-server/WorkvivoMcpServer.csproj"
      ],
      "env": {
        "WORKVIVO_APITOKEN": "your-api-token",
        "WORKVIVO_ORGANIZATIONID": "your-org-id",
        "WORKVIVO_BASEURL": "https://api.workvivo.com/v1"
      }
    }
  }
}
```

### Cursor

Add this to `.cursor/mcp.json` in your workspace:

```json
{
  "mcpServers": {
    "workvivo": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "src/workvivo-mcp-server/WorkvivoMcpServer.csproj"
      ],
      "env": {
        "WORKVIVO_APITOKEN": "your-api-token",
        "WORKVIVO_ORGANIZATIONID": "your-org-id",
        "WORKVIVO_BASEURL": "https://api.workvivo.com/v1"
      }
    }
  }
}
```
### Warp.dev
Add this new MCP server to Warp:

```json
{
  "workvivo": {
    "args": [
      "mcp"
    ],
    "command": "WorkvivoMcpServer.exe",
    "env": {
      "WORKVIVO_APITOKEN": "your-api-token",
      "WORKVIVO_BASEURL": "https://api.eu2.workvivo.com/v1",
      "WORKVIVO_ORGANIZATIONID": "your-org-id"
    }
  }
}
```

### Zed.dev 
Add this new MCP server to Zed:

```json
{
  "workvivo": {
    "args": [
      "mcp"
    ],
    "command": "WorkvivoMcpServer.exe",
    "env": {
      "WORKVIVO_APITOKEN": "your-api-token",
      "WORKVIVO_BASEURL": "https://api.eu2.workvivo.com/v1",
      "WORKVIVO_ORGANIZATIONID": "your-org-id"
    }
  }
}
```

## Testing with MCP Inspector

The MCP Inspector is a tool for testing MCP servers:

```bash
# Install and run the MCP Inspector
npx @modelcontextprotocol/inspector dotnet run --project src/workvivo-mcp-server/WorkvivoMcpServer.csproj
```

This will open a web interface where you can:
- See all available tools
- Test each tool with different parameters
- View request/response data
- Debug any issues

## Example AI Prompts

Once configured, you can ask your AI assistant:

### User Queries
- "Show me all users in the Marketing space"
- "Find users with 'admin' in their job title"
- "How many users have logged in?"
- "Search for a user named John Smith"
- "Get details about user ID 12345"

### Space Queries
- "List all corporate spaces"
- "How many users are in the Engineering space?"
- "Show me all spaces and their member counts"
- "Find spaces with 'Sales' in the name"
- "Who are the owners of space ID 82364?"

### Combined Queries
- "How many spaces does user john@example.com belong to?"
- "List all users who are owners of corporate spaces"
- "Show me inactive users (HasAccess = false)"

## Architecture

- **Transport**: STDIO (Standard Input/Output) - for local use with MCP clients
- **Authentication**: Workvivo API token + Organization ID
- **Error Handling**: All errors logged to stderr (critical for STDIO transport)
- **Dependencies**: Shares code with the Workvivo CLI via `Workvivo.Shared` library

## Troubleshooting

### "Configuration Error" when starting

Make sure you've configured credentials using one of the three methods above. The server validates credentials on startup.

### Tools not appearing in VS Code/Copilot

1. Check that `.vscode/mcp.json` exists and has correct configuration
2. Restart VS Code completely
3. Check VS Code output panel for MCP-related errors
4. Verify the server starts correctly: `dotnet run --project src/workvivo-mcp-server/WorkvivoMcpServer.csproj`

### API authentication errors

- Verify your API token is correct
- Verify your Organization ID is correct
- Check that the BaseUrl is correct (default: https://api.workvivo.com/v1)

### Server crashes or hangs

- Check stderr output for error messages
- Ensure you're not writing to stdout (this breaks STDIO transport)
- Test with MCP Inspector to isolate the issue

## Development

To modify or extend the server:

1. Tool classes are in `Tools/` directory
2. Mark classes with `[McpServerToolType]`
3. Mark methods with `[McpServerTool]` and add `[Description]` attributes
4. All logging must go to `Console.Error` (never `Console.Out`)
5. Use dependency injection for `IWorkvivoApiClient`

## Security Notes

- This server is **read-only** - it cannot modify any data in Workvivo
- API credentials are never written to stdout
- The `.vscode/mcp.json` file is gitignored to prevent credential commits
- Use user secrets for local development, environment variables for production

## License

This project is licensed under the GNU General Public License v3.0 or later - see the LICENSE file for details.

## Support

For issues or questions:
- Check the [main README](../README.md) for general setup
- Review the [MCP specification](https://modelcontextprotocol.io)
- Open an issue on GitHub
