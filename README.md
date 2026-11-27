# Workvivo CLI

NB: This is an early version of the Workvivo CLI. At this time it is read-only, not all features are implemented.

A .NET 10 command-line tool for managing Workvivo spaces and users via the Workvivo REST API. The tool works on Windows, macOS and Linux.

The repository also includes a [Workvivo MCP server](#mcp-server) that gives AI assistants read-only access to the same users and spaces data.

## Quick Start

**No prerequisites** — self-contained executables include the .NET runtime.

For installation options, see the [Installation Guide](docs/INSTALL.md).

### Configuration

Set your Workvivo API credentials via environment variables:

```bash
export WORKVIVO_APITOKEN="your-api-token-here"
export WORKVIVO_ORGANIZATIONID="your-org-id-here"
```

Or use `appsettings.json`, User Secrets, and more — see the full [Configuration Guide](docs/INSTALL.md#configuration).

## Usage

### General Syntax

```bash
wv [command] [subcommand] [arguments] [options]
```

### Global Options

- `--json`: Output results in JSON format (default: false)
- `--csv`: Output results in CSV format (default: false). Cannot be combined with `--json`
- `--help`, `-h`, `-?`: Show help information
- `--version`: Show version information

### Users Commands

#### List all users

```bash
# List users (table format)
wv users list

# List users in JSON format
wv users list --json

# List with pagination
wv users list --skip 0 --take 25

# Get all users (fetches all pages automatically)
wv users list --all

# Filter users by spaces
wv users list --in-spaces "space-id-1|space-id-2"

# Expand related objects
wv users list --expand teams
```

#### Get a specific user

```bash
# Get user by ID (table format)
wv users get <user-id>

# Get user in JSON format
wv users get <user-id> --json

# Get user with expanded data
wv users get <user-id> --expand teams
```

### Spaces Commands

#### List all spaces

```bash
# List all spaces (table format)
wv spaces list

# List spaces in JSON format
wv spaces list --json

# List with pagination
wv spaces list --skip 0 --take 25

# Get all spaces (fetches all pages automatically)
wv spaces list --all

# Filter by space type
wv spaces list --type Corporate
```

#### Get a specific space

```bash
# Get space by ID (table format)
wv spaces get <space-id>

# Get space in JSON format
wv spaces get <space-id> --json
```

#### List users in a space

```bash
# List users in a specific space (table format)
wv spaces users <space-id>

# List users in JSON format
wv spaces users <space-id> --json

# List with pagination
wv spaces users <space-id> --skip 0 --take 25

# Get all users in space (fetches all pages automatically)
wv spaces users <space-id> --all
```

**Note:** The users command shows the `space_role` for each user (owner, admin, member).

## Output Formats

### Table Format (Default)

Human-readable table output using Spectre.Console:

```
┌──────────┬─────────────────┬──────────────────────┐
│ ID       │ Name            │ Email                │
├──────────┼─────────────────┼──────────────────────┤
│ 12345    │ John Doe        │ john@example.com     │
│ 12346    │ Jane Smith      │ jane@example.com     │
└──────────┴─────────────────┴──────────────────────┘
```

### JSON Format

Machine-readable JSON output for automation:

```json
[
  {
    "id": "12345",
    "displayName": "John Doe",
    "email": "john@example.com",
    "status": "active"
  }
]
```

### CSV Format

Comma-separated output (RFC 4180) with a header row, for spreadsheets and data tools. Each resource type has a fixed set of columns:

```bash
wv users list --csv
```

```csv
id,external_id,email,name,display_name,first_name,last_name,job_title,has_access
```

### Redirecting Output to Files

When redirecting JSON or CSV output to files, only the data is written to stdout:

```bash
# Clean JSON output - no log messages
wv spaces users 82364 --json --all > spaces.json

# If you see any warnings/errors, they'll be on stderr (terminal)
# and won't contaminate your JSON file

# Same for CSV
wv users list --csv --all > users.csv
```

Log messages are always sent to stderr, keeping your JSON and CSV files clean.

## Error Handling

The CLI provides helpful error messages for common issues:

- **Missing Credentials**: Displays configuration instructions
- **API Errors**: Shows HTTP status codes and error messages
- **Network Errors**: Indicates connection problems

## MCP Server

`src/workvivo-mcp-server` is a [Model Context Protocol](https://modelcontextprotocol.io) server that exposes the same Workvivo data to MCP clients such as VS Code with GitHub Copilot, Claude Desktop and Cursor. It is read-only and provides 11 tools for listing, fetching and searching users and spaces.

Unlike the CLI, it is not shipped as a self-contained executable: it runs from source and needs the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).

```bash
dotnet run --project src/workvivo-mcp-server/WorkvivoMcpServer.csproj
```

It reads the same `WORKVIVO_APITOKEN`, `WORKVIVO_ORGANIZATIONID` and `WORKVIVO_BASEURL` environment variables as the CLI. `mcp.json.template` in the repository root is a starting point for client configuration.

See the [MCP server README](src/workvivo-mcp-server/README.md) for client setup, the tool list and troubleshooting.

## Development

For building from source, running tests, project architecture, CI/CD, and contributing, see the [Development Guide](docs/DEVELOPMENT.md).

## Roadmap

- [ ] Add support for Posts management
- [ ] Add support for Teams management
- [ ] Add support for Events management
- [ ] Implement caching for frequently accessed data
- [ ] Add autocomplete support
- [ ] Add batch operations
- [ ] Implement retry logic with exponential backoff

## License

This project is licensed under the GNU General Public License v3.0 or later - see the [LICENSE](LICENSE) file for details.

**What this means:**
- ✅ You can use this tool commercially
- ✅ You can modify it
- ✅ You can distribute it
- ⚠️ **If you distribute modified versions, you must share the source code under GPL v3**
- ⚠️ **Any derivative works must also be GPL v3**

This ensures that improvements to the tool remain available to the community.

## Support

For issues or questions:

- Open an issue on GitHub
- Check the [Workvivo API Documentation](https://developer.workvivo.com)
- Contact Workvivo support at api@workvivo.com

## Acknowledgments

- Built with [Spectre.Console.Cli](https://spectreconsole.net/)
- Table formatting by [Spectre.Console](https://spectreconsole.net/)
- Powered by the [Workvivo API](https://developer.workvivo.com)
