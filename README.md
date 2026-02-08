# Workvivo CLI

NB: This is an early version of the Workvivo CLI. At this time it is read-only, not all features are implemented.

A .NET 10 command-line tool for managing Workvivo spaces and users via the Workvivo REST API.

## Cross-Platform Support

✅ **Cross-platform** - Works on:
- 🪟 **Windows** (10/11, Server 2019+)
- 🍎 **macOS** (10.15+)
- 🐧 **Linux** (Ubuntu, Debian, Fedora, RHEL, etc.)

## Quick Start

**No prerequisites** — self-contained executables include the .NET runtime.

```powershell
# Windows (via winget)
winget install wv
```

For macOS, Linux, and other installation options, see the [Installation Guide](docs/INSTALL.md).

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

### Redirecting Output to Files

When redirecting JSON output to files, only the data is written to stdout:

```bash
# Clean JSON output - no log messages
wv spaces users 82364 --json --all > spaces.json

# If you see any warnings/errors, they'll be on stderr (terminal)
# and won't contaminate your JSON file
```

Log messages are always sent to stderr, keeping your JSON files clean.

## Error Handling

The CLI provides helpful error messages for common issues:

- **Missing Credentials**: Displays configuration instructions
- **API Errors**: Shows HTTP status codes and error messages
- **Network Errors**: Indicates connection problems

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