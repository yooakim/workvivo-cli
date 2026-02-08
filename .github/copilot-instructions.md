# Copilot Instructions for Workvivo CLI

## Project Overview

A .NET 10 CLI tool for managing Workvivo spaces and users via the Workvivo REST API. Distributed as self-contained executables (no .NET runtime required for end users). Built with Spectre.Console.Cli for CLI parsing and Spectre.Console for table formatting. Fully cross-platform (Windows, macOS, Linux).

## Build, Test, and Lint

### Build
```bash
# Build the project (development)
dotnet build src/WorkvivoCli.csproj -c Release

# Build self-contained executables for all platforms
./build-all.sh          # macOS/Linux
.\build-all.ps1         # Windows PowerShell
build-all.cmd           # Windows CMD

# Build for a specific platform
dotnet publish src/WorkvivoCli.csproj -c Release -r win-x64 --self-contained
dotnet publish src/WorkvivoCli.csproj -c Release -r osx-x64 --self-contained
dotnet publish src/WorkvivoCli.csproj -c Release -r linux-x64 --self-contained
```

### Test
```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test /p:CollectCoverage=true

# Run a single test by filter
dotnet test --filter "FullyQualifiedName~WorkvivoSettingsTests.Validate_WithValidSettings_ShouldNotThrow"
dotnet test --filter "DisplayName~Validate_WithValidSettings"
```

### Lint/Format
```bash
# Format code (auto-fix)
dotnet format WorkvivoCli.slnx

# Verify formatting without changes (for CI)
dotnet format WorkvivoCli.slnx --verify-no-changes
```

### Local Installation (for testing)
```bash
# Run directly without installing
dotnet run --project src/WorkvivoCli.csproj -- users list --help

# Or test the built executable
./dist/windows-x64/workvivo-cli.exe --version   # Windows
./dist/macos-x64/workvivo-cli --version         # macOS Intel
./dist/linux-x64/workvivo-cli --version         # Linux
```

## Architecture

### Distribution Strategy
- **Self-contained executables**: No .NET runtime required on target machines
- **Platform-specific builds**: win-x64, osx-x64, osx-arm64, linux-x64, linux-arm64
- **Single-file deployment**: All dependencies bundled into one executable (~13MB per platform)
- **Trimmed for size**: Uses IL trimming to reduce file size while preserving functionality
- **Package managers**: Primary distribution via winget (Windows), GitHub Releases (all platforms)

### Trimming Warnings
**Solution implemented**: Source Generators for JSON serialization (trim-safe)
- Created `Serialization/AppJsonSerializerContext.cs` with `[JsonSerializable]` attributes for all models
- All JSON operations use the pre-generated context instead of reflection
- Zero IL2026 warnings (JSON serialization is now trim-safe)
- Third-party library warnings (IL2104) suppressed as they're tested and working

**Why this matters**: Reflection-based JSON serialization can be problematic with trimming because the trimmer can't analyze what types will be needed at runtime. Source generators create the serialization code at compile time, making it trim-safe and eliminating the warnings.

To add new types to serialization:
```csharp
// In AppJsonSerializerContext.cs, add:
[JsonSerializable(typeof(YourNewType))]
[JsonSerializable(typeof(List<YourNewType>))]
```

### Command Structure
- **Spectre.Console.Cli** for CLI framework (stable, released — replaced System.CommandLine beta)
- **`CommandApp`** → **Branches** (`users`, `spaces`) → **`AsyncCommand<TSettings>`** classes (`list`, `get`)
- Each command is a separate class inheriting from `AsyncCommand<TSettings>` (e.g., `ListUsersCommand`, `GetUserCommand`)
- Command options/arguments defined declaratively via `[CommandOption]` and `[CommandArgument]` attributes on nested `Settings` classes
- All commands inherit from `GlobalSettings` base class which provides shared options (e.g., `--json`)

### Dependency Injection Flow
1. `Program.cs` builds configuration via `ConfigurationBuilder` (appsettings.json, user-secrets, env vars)
2. `WorkvivoSettings` bound and validated before DI container is built (skipped for `--help`/`--version`)
3. `ServiceCollection` configured with settings singleton and `HttpClient` factory for `IWorkvivoApiClient`
4. `TypeRegistrar` (in `Infrastructure/`) bridges `IServiceCollection` with Spectre.Console.Cli's `ITypeRegistrar`
5. Commands receive services via **constructor injection** (e.g., `ListUsersCommand(IWorkvivoApiClient apiClient)`)

### Output Formatting
- Two formatters implement `IOutputFormatter`: `TableOutputFormatter` (Spectre.Console) and `JsonOutputFormatter`
- **Critical**: All logging goes to `stderr`, data output to `stdout` (enables clean JSON piping)
- Formatter selected at runtime based on `--json` flag
- Table formatter uses Spectre.Console for rich terminal tables

### API Client Pattern
- `WorkvivoApiClient` implements `IWorkvivoApiClient`
- Configured with base URL and authentication headers in constructor
- Uses `HttpClient` with factory pattern for dependency injection
- All API methods return `PagedResponse<T>` for consistency
- Query parameters built dynamically based on optional arguments

### Configuration Precedence
1. Environment variables (`WORKVIVO_APITOKEN`, `WORKVIVO_ORGANIZATIONID`, `WORKVIVO_BASEURL`)
2. User secrets (via `dotnet user-secrets` - recommended for local development)
3. `appsettings.json` file
4. Validation happens in `Program.cs` before DI container is built

### User Secrets Setup (Local Development)
```bash
# Set credentials securely (not committed to source control)
dotnet user-secrets set "ApiToken" "your-token" --project src/WorkvivoCli.csproj
dotnet user-secrets set "OrganizationId" "your-org-id" --project src/WorkvivoCli.csproj
dotnet user-secrets set "BaseUrl" "https://api.workvivo.com/v1" --project src/WorkvivoCli.csproj

# List/manage secrets
dotnet user-secrets list --project src/WorkvivoCli.csproj
dotnet user-secrets remove "ApiToken" --project src/WorkvivoCli.csproj
dotnet user-secrets clear --project src/WorkvivoCli.csproj
```

User secrets location:
- **Windows**: `%APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json`
- **macOS/Linux**: `~/.microsoft/usersecrets/<user_secrets_id>/secrets.json`

### Configuration Key Reference

All configuration sources map to the same `WorkvivoSettings` properties. The key names differ by source:

| Setting | `appsettings.json` | User Secrets | Environment Variable |
|---|---|---|---|
| API Token | `"ApiToken"` | `ApiToken` | `WORKVIVO_APITOKEN` |
| Organization ID | `"OrganizationId"` | `OrganizationId` | `WORKVIVO_ORGANIZATIONID` |
| Base URL | `"BaseUrl"` | `BaseUrl` | `WORKVIVO_BASEURL` |

Environment variables use the `WORKVIVO_` prefix which is stripped by `AddEnvironmentVariables(prefix: "WORKVIVO_")`, then bound case-insensitively to the property names. User secrets and appsettings.json keys match the property names directly.

## Key Conventions

### File Organization
- **Commands/**: Command classes grouped by resource (e.g., `UsersCommand.cs` contains `ListUsersCommand`, `GetUserCommand`)
  - `GlobalSettings.cs`: Base `CommandSettings` class with shared options (`--json`)
  - Each command class contains a nested `Settings` class inheriting from `GlobalSettings`
- **Infrastructure/**: DI bridge for Spectre.Console.Cli (`TypeRegistrar.cs`, `TypeResolver.cs`)
- **Services/**: API clients with interface + implementation (e.g., `IWorkvivoApiClient`, `WorkvivoApiClient`)
- **Models/**: DTOs with `[JsonPropertyName]` attributes matching API snake_case (e.g., `external_id`, `display_name`)
- **Output/**: Formatters implementing `IOutputFormatter` interface
- **Configuration/**: Settings classes (e.g., `WorkvivoSettings`)

### Naming Patterns
- **Interfaces**: Prefix with `I` (e.g., `IWorkvivoApiClient`, `IOutputFormatter`)
- **Commands**: Suffix with `Command` (e.g., `ListUsersCommand`, `GetSpaceCommand`)
- **Models**: Match API resource names (e.g., `User`, `Space`, `PagedResponse<T>`)
- **Settings**: Suffix with `Settings` (e.g., `WorkvivoSettings`)

### Code Style (from .editorconfig)
- **Indentation**: 4 spaces for C#, 2 spaces for JSON/YAML/XML
- **var**: Use for built-in types and when type is apparent
- **Braces**: Always use braces, new line before open brace (`csharp_new_line_before_open_brace = all`)
- **Usings**: Outside namespace, System directives first, no grouping
- **Null checks**: Prefer null coalescing (`??`) and null propagation (`?.`)
- **Expression bodies**: Use for properties/indexers/lambdas, not for methods/constructors

### Testing Patterns
- **Framework**: xUnit with FluentAssertions for assertions, NSubstitute for mocking
- **Test structure**: Arrange-Act-Assert with comments
- **Naming**: `MethodName_Scenario_ExpectedOutcome` (e.g., `Validate_WithValidSettings_ShouldNotThrow`)
- **Theory tests**: Use `[Theory]` with `[InlineData]` for parameterized tests
- **Assertions**: FluentAssertions style (`.Should().NotThrow()`, `.Should().Throw<T>().WithMessage()`)

### JSON Serialization
- Use `System.Text.Json` (not Newtonsoft.Json)
- Configure `PropertyNameCaseInsensitive = true` in serializer options
- Models use `[JsonPropertyName("snake_case")]` attributes to match Workvivo API

### Logging
- **All status/progress messages to stderr**: Commands write informational messages via `Console.Error.WriteLine`
- **Data output to stdout**: Formatters write data (tables, JSON) to `Console.WriteLine` / `AnsiConsole.Write`
- **Purpose**: Keeps stdout clean for JSON output redirection (e.g., `workvivo-cli users list --json > users.json`)

### Error Handling
- Configuration errors: Display rich formatted errors using Spectre.Console panels and tables
- Error messages use `AnsiConsoleSettings` with `Out = new AnsiConsoleOutput(Console.Error)` to ensure stderr output
- API errors: Catch in command handlers, write to `Console.Error`, return exit code 1
- HTTP errors: `response.EnsureSuccessStatusCode()` throws on non-2xx status codes

### Spectre.Console Usage
- **Tables**: All data output formatters use `Spectre.Console.Table` for rich terminal display
- **Error messages**: Configuration errors displayed with colored panels, tables, and markup
- **stderr routing**: Error console created with `AnsiConsoleSettings { Out = new AnsiConsoleOutput(Console.Error) }`
- **Markup syntax**: Use `[red]`, `[green]`, `[yellow]`, `[bold]`, `[dim]` etc. for colored output

### Pagination Strategy
- API uses `skip` and `take` parameters (defaults: skip=0, take=50)
- `--all` flag fetches all pages automatically (loops until `next_url` is null)
- Commands should support both manual pagination and `--all` mode

## Cross-Platform Notes

- **Build scripts**: Provide `build-all.sh`, `build-all.ps1`, and `build-all.cmd` for all platforms
- **Path handling**: Use .NET APIs (e.g., `AppContext.BaseDirectory`), never hardcode separators
- **Line endings**: 
  - `.editorconfig` enforces LF for code files (works everywhere in .NET)
  - `.gitattributes` normalizes line endings in Git (LF for most files, CRLF for .cmd/.bat)
  - Shell scripts (.sh, .ps1) must have LF to work on Unix
  - Windows batch files (.cmd, .bat) must have CRLF
- **Environment variables**: Prefix with `WORKVIVO_` for automatic binding via `AddEnvironmentVariables(prefix: "WORKVIVO_")`. The prefix is stripped and keys map directly to `WorkvivoSettings` properties (e.g., `WORKVIVO_APITOKEN` → `ApiToken`)
- **Terminal output**: Spectre.Console handles Unicode and terminal capabilities automatically
- **Git configuration**: See `.github/GIT_CONFIGURATION.md` for details on .gitignore, .gitattributes, .editorconfig

## CI/CD and Releases

### GitHub Actions Workflow
`.github/workflows/build-release.yml` handles:
- **Build matrix**: All 5 platforms (win-x64, osx-x64, osx-arm64, linux-x64, linux-arm64)
- **Artifacts**: Creates .zip (Windows) and .tar.gz (Unix) archives with SHA256 checksums
- **Releases**: Auto-creates GitHub Release on version tags (v*)
- **Trigger**: Runs on tag push (`git tag v0.1.0 && git push --tags`) or manual workflow dispatch

### Creating a Release
```bash
# 1. Update version in src/WorkvivoCli.csproj
# 2. Commit and tag
git add src/WorkvivoCli.csproj
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

## MCP Server Recommendations

### GitHub MCP
When the GitHub MCP server is enabled, Copilot can:
- List and search issues/PRs across the repository
- Read PR diffs and review comments
- Check GitHub Actions workflow status
- Browse repository files and commit history
- Search code across the repository

Useful for:
- Reviewing PRs before merging
- Checking CI/CD pipeline failures
- Finding related issues or past discussions
- Code archaeology (finding when/why changes were made)

### Microsoft/Azure MCP
When the Microsoft/Azure MCP server is enabled, Copilot can access official .NET documentation:
- .NET API reference (e.g., `System.CommandLine`, `System.Text.Json`, `HttpClient`)
- .NET CLI command documentation
- Best practices and migration guides
- NuGet package documentation

Especially useful for:
- Looking up Spectre.Console.Cli patterns and APIs
- Understanding .NET 10 features and APIs
- Finding recommended practices for HttpClient factory
- Researching configuration and dependency injection patterns

## Dependencies

- **Spectre.Console** (0.49.1): Rich terminal formatting (tables, colors, panels)
- **Spectre.Console.Cli** (0.49.1): CLI framework (commands, settings, help generation)
- **Microsoft.Extensions.Configuration.\***: Configuration from appsettings.json, user-secrets, environment variables
- **Microsoft.Extensions.DependencyInjection**: DI container (bridged to Spectre.Console.Cli via `TypeRegistrar`)
- **Microsoft.Extensions.Http**: `IHttpClientFactory` for `WorkvivoApiClient`
- **xUnit**, **FluentAssertions**, **NSubstitute**: Testing stack

### Adding a New Command
1. Create a class inheriting `AsyncCommand<TSettings>` with a nested `Settings : GlobalSettings` class
2. Define options/arguments with `[CommandOption]`/`[CommandArgument]` attributes
3. Inject services via the constructor (e.g., `IWorkvivoApiClient`)
4. Register the command in `Program.cs` via `config.AddCommand<T>("name")` inside the appropriate branch
