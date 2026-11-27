# Development Guide

This guide covers building, testing, and contributing to the Workvivo CLI and the Workvivo MCP server that lives in the same repository.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later

## Build from Source

```bash
# Clone the repository
git clone https://github.com/yooakim/workvivo-cli.git
cd workvivo-cli

# Restore and build everything (shared library, CLI, MCP server, tests)
dotnet restore WorkvivoCli.slnx
dotnet build WorkvivoCli.slnx

# Build only the CLI
dotnet build src/workvivo-cli/WorkvivoCli.csproj -c Release

# Run the CLI without installing
dotnet run --project src/workvivo-cli/WorkvivoCli.csproj -- users list --help

# Run the MCP server (STDIO transport; requires credentials)
dotnet run --project src/workvivo-mcp-server/WorkvivoMcpServer.csproj
```

## Build Scripts

Build self-contained CLI executables for all platforms (the build scripts publish `src/workvivo-cli` only):

**macOS/Linux:**
```bash
./build-all.sh
```

**Windows (PowerShell):**
```powershell
.\build-all.ps1
```

**Windows (CMD):**
```cmd
build-all.cmd
```

Output will be in `nupkg/` directory as platform-specific archives (`.tar.gz` for macOS/Linux, `.zip` for Windows).

### Build for a Specific Platform

```bash
dotnet publish src/workvivo-cli/WorkvivoCli.csproj -c Release -r win-x64 --self-contained
dotnet publish src/workvivo-cli/WorkvivoCli.csproj -c Release -r osx-x64 --self-contained
dotnet publish src/workvivo-cli/WorkvivoCli.csproj -c Release -r linux-x64 --self-contained
```

### Test the Built Executable

```bash
./nupkg/wv --help
```

## Project Structure

The solution (`WorkvivoCli.slnx`) contains three source projects and one test project. The CLI and the MCP server both reference `Workvivo.Shared`; they do not reference each other.

```
workvivo-cli/
├── src/
│   ├── Workvivo.Shared/           # Class library shared by both executables
│   │   ├── Configuration/         # Config model and validation
│   │   │   └── WorkvivoSettings.cs
│   │   ├── Models/                # DTOs for API responses
│   │   │   ├── User.cs
│   │   │   ├── Space.cs
│   │   │   └── PagedResponse.cs
│   │   ├── Serialization/         # JSON source generators (trim-safe)
│   │   │   └── AppJsonSerializerContext.cs
│   │   ├── Services/              # API client
│   │   │   ├── IWorkvivoApiClient.cs
│   │   │   └── WorkvivoApiClient.cs
│   │   └── Workvivo.Shared.csproj
│   ├── workvivo-cli/              # The `wv` command-line tool
│   │   ├── Commands/              # Command implementations
│   │   │   ├── UsersCommand.cs
│   │   │   ├── SpacesCommand.cs
│   │   │   └── GlobalSettings.cs
│   │   ├── Infrastructure/        # DI bridge for Spectre.Console.Cli
│   │   │   ├── TypeRegistrar.cs
│   │   │   └── TypeResolver.cs
│   │   ├── Output/                # Formatters (table, JSON, CSV)
│   │   │   ├── IOutputFormatter.cs
│   │   │   ├── OutputFormat.cs
│   │   │   ├── OutputFormatterFactory.cs
│   │   │   ├── TableOutputFormatter.cs
│   │   │   ├── JsonOutputFormatter.cs
│   │   │   └── CsvOutputFormatter.cs
│   │   ├── Program.cs             # Entry point
│   │   ├── appsettings.json       # Configuration template
│   │   └── WorkvivoCli.csproj
│   └── workvivo-mcp-server/       # MCP server for AI assistants
│       ├── Tools/                 # MCP tool classes
│       │   ├── UserTools.cs
│       │   └── SpaceTools.cs
│       ├── Program.cs             # Entry point (STDIO host)
│       ├── appsettings.json       # Configuration template
│       ├── README.md              # MCP server setup and usage
│       └── WorkvivoMcpServer.csproj
├── tests/
│   └── WorkvivoCli.Tests/         # Unit tests (CLI + shared library)
├── docs/                          # Documentation
├── winget/                        # Windows Package Manager manifests
├── .editorconfig                  # Code formatting rules
├── mcp.json.template              # MCP client config template
├── build-all.sh                   # Build script (macOS/Linux)
├── build-all.ps1                  # Build script (Windows PowerShell)
├── build-all.cmd                  # Build script (Windows CMD)
└── WorkvivoCli.slnx               # Solution file
```

**Namespaces:** `Workvivo.Shared.*` for the shared library, `WorkvivoCli.*` for the CLI, `WorkvivoMcpServer.*` for the MCP server.

## Configuration for Development

### User Secrets (Recommended for Local Development)

User secrets keep credentials out of source control. They are stored on your machine, outside the repository, per project. The CLI and the MCP server each have their own secrets store, so set them for whichever you run (for the MCP server, use `--project src/workvivo-mcp-server/WorkvivoMcpServer.csproj`).

```bash
dotnet user-secrets set "ApiToken" "your-api-token-here" --project src/workvivo-cli/WorkvivoCli.csproj
dotnet user-secrets set "OrganizationId" "your-org-id-here" --project src/workvivo-cli/WorkvivoCli.csproj
dotnet user-secrets set "BaseUrl" "https://api.workvivo.com/v1" --project src/workvivo-cli/WorkvivoCli.csproj
```

**Manage secrets:**
```bash
# List all secrets
dotnet user-secrets list --project src/workvivo-cli/WorkvivoCli.csproj

# Remove a secret
dotnet user-secrets remove "ApiToken" --project src/workvivo-cli/WorkvivoCli.csproj

# Clear all secrets
dotnet user-secrets clear --project src/workvivo-cli/WorkvivoCli.csproj
```

**User secrets location:**
- **Windows**: `%APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json`
- **macOS/Linux**: `~/.microsoft/usersecrets/<user_secrets_id>/secrets.json`

### Configuration Precedence

**Highest to lowest:** Environment variables → User secrets → appsettings.json

See the [README](../README.md#configuration) for all configuration options.

## Code Formatting

```bash
# Format code (auto-fix)
dotnet format WorkvivoCli.slnx

# Verify formatting without changes (not enforced by any workflow)
dotnet format WorkvivoCli.slnx --verify-no-changes
```

## Running Tests

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run a single test by filter
dotnet test --filter "FullyQualifiedName~WorkvivoSettingsTests.Validate_WithValidSettings_ShouldNotThrow"
dotnet test --filter "DisplayName~Validate_WithValidSettings"
```

### Test Conventions

- **Framework**: xUnit with FluentAssertions for assertions, NSubstitute for mocking
- **Structure**: Arrange-Act-Assert with comments
- **Naming**: `MethodName_Scenario_ExpectedOutcome` (e.g., `Validate_WithValidSettings_ShouldNotThrow`)
- **Parameterized tests**: Use `[Theory]` with `[InlineData]`

## Architecture

### Distribution Strategy

- **Self-contained executables**: No .NET runtime required on target machines
- **Platform-specific builds**: win-x64, osx-x64, osx-arm64, linux-x64, linux-arm64
- **Single-file deployment**: All dependencies bundled into one executable (~13MB per platform)
- **Trimmed for size**: Uses IL trimming to reduce file size while preserving functionality

### Projects

- **`Workvivo.Shared`**: settings, models, JSON serializer context and the API client. Anything both front ends need goes here.
- **`workvivo-cli`**: the `wv` executable (commands, DI bridge, output formatters).
- **`workvivo-mcp-server`**: a read-only MCP server exposing the same API as tools.

The sections below describe the CLI unless noted otherwise.

### Command Structure

- **Spectre.Console.Cli** for CLI framework
- **`CommandApp`** → **Branches** (`users`, `spaces`) → **`AsyncCommand<TSettings>`** classes (`list`, `get`)
- Each command is a separate class inheriting from `AsyncCommand<TSettings>`
- Command options/arguments defined declaratively via `[CommandOption]` and `[CommandArgument]` attributes on nested `Settings` classes
- All commands inherit from `GlobalSettings` base class which provides shared options (`--json`, `--csv`)

### Dependency Injection Flow

1. `Program.cs` builds configuration via `ConfigurationBuilder` (appsettings.json, user-secrets, env vars)
2. `WorkvivoSettings` bound and validated before DI container is built (skipped for `--help`/`--version`)
3. `ServiceCollection` configured with settings singleton and `HttpClient` factory for `IWorkvivoApiClient`
4. `TypeRegistrar` (in `src/workvivo-cli/Infrastructure/`) bridges `IServiceCollection` with Spectre.Console.Cli's `ITypeRegistrar`
5. Commands receive services via **constructor injection**

### Output Formatting

- Three formatters implement `IOutputFormatter`: `TableOutputFormatter` (Spectre.Console), `JsonOutputFormatter` and `CsvOutputFormatter`
- **Critical**: All logging goes to `stderr`, data output to `stdout` (enables clean JSON/CSV piping)
- `OutputFormatterFactory` selects the formatter at runtime from the `--json` / `--csv` flags

### JSON Serialization and Trimming

Source generators are used for JSON serialization to ensure trim-safety:
- `src/Workvivo.Shared/Serialization/AppJsonSerializerContext.cs` contains `[JsonSerializable]` attributes for all models
- All JSON operations use the pre-generated context instead of reflection
- Zero IL2026 warnings

**To add new types to serialization:**
```csharp
// In AppJsonSerializerContext.cs, add:
[JsonSerializable(typeof(YourNewType))]
[JsonSerializable(typeof(List<YourNewType>))]
```

### MCP Server

- Built on the `ModelContextProtocol` SDK with `Microsoft.Extensions.Hosting`; `Program.cs` registers `AddMcpServer().WithStdioServerTransport().WithToolsFromAssembly()`
- Tools are methods marked `[McpServerTool]` on classes marked `[McpServerToolType]` in `Tools/`, with `[Description]` attributes on the method and its parameters; `IWorkvivoApiClient` is constructor-injected
- **Critical**: the transport is STDIO, so stdout carries the protocol. All logging must go to `Console.Error`
- Uses the same configuration sources and `WORKVIVO_*` environment variables as the CLI
- See [`src/workvivo-mcp-server/README.md`](../src/workvivo-mcp-server/README.md) for client setup and the tool list

## CI/CD

### GitHub Actions Workflow

`.github/workflows/build-release.yml` handles:
- **Build matrix**: All 5 platforms (win-x64, osx-x64, osx-arm64, linux-x64, linux-arm64)
- **Artifacts**: Creates .zip (Windows) and .tar.gz (Unix) archives with SHA256 checksums
- **Releases**: Auto-creates GitHub Release on version tags (`v*`)
- **Trigger**: Runs on tag push or manual workflow dispatch

### Creating a Release

```bash
# 1. Update version in src/workvivo-cli/WorkvivoCli.csproj
# 2. Commit and tag
git add src/workvivo-cli/WorkvivoCli.csproj
git commit -m "Bump version to 0.2.0"
git tag v0.2.0
git push origin main --tags

# 3. GitHub Actions will build and create release automatically
# 4. Update winget manifests with new SHA256 hashes from release
```

### Winget Package

- Manifests in `winget/` directory
- Follow `winget/README.md` for submission process to microsoft/winget-pkgs
- Must update SHA256 hash in installer manifest after each GitHub Release
- Test locally before submitting: `winget install --manifest winget\YourCompany.WorkvivoCli.yaml`

## Contributing

Contributions are welcome! Please follow these guidelines:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Format your code (`dotnet format WorkvivoCli.slnx`)
5. Run the tests (`dotnet test`)
6. Push to the branch (`git push origin feature/amazing-feature`)
7. Open a Pull Request

## Dependencies

- **Spectre.Console** (0.49.1): Rich terminal formatting (tables, colors, panels)
- **Spectre.Console.Cli** (0.49.1): CLI framework (commands, settings, help generation)
- **Microsoft.Extensions.Configuration.\***: Configuration from appsettings.json, user-secrets, environment variables
- **Microsoft.Extensions.DependencyInjection**: DI container (bridged to Spectre.Console.Cli via `TypeRegistrar`)
- **Microsoft.Extensions.Http**: `IHttpClientFactory` for `WorkvivoApiClient`
- **ModelContextProtocol** (preview) and **Microsoft.Extensions.Hosting**: MCP server only
- **xUnit**, **FluentAssertions**, **NSubstitute**: Testing stack