# Development Guide

This guide covers building, testing, and contributing to the Workvivo CLI.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later

## Build from Source

```bash
# Clone the repository
git clone https://github.com/yooakim/workvivo-cli.git
cd workvivo-cli

# Restore dependencies
dotnet restore src/WorkvivoCli.csproj

# Build the project
dotnet build src/WorkvivoCli.csproj -c Release

# Run the tool without installing
dotnet run --project src/WorkvivoCli.csproj -- users list --help
```

## Build Scripts

Build self-contained executables for all platforms:

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
dotnet publish src/WorkvivoCli.csproj -c Release -r win-x64 --self-contained
dotnet publish src/WorkvivoCli.csproj -c Release -r osx-x64 --self-contained
dotnet publish src/WorkvivoCli.csproj -c Release -r linux-x64 --self-contained
```

### Test the Built Executable

```bash
./nupkg/wv --help
```

## Project Structure

```
workvivo-cli/
├── src/
│   ├── Commands/          # Command implementations
│   │   ├── UsersCommand.cs
│   │   ├── SpacesCommand.cs
│   │   └── GlobalSettings.cs
│   ├── Configuration/     # Config models and setup
│   │   └── WorkvivoSettings.cs
│   ├── Infrastructure/    # DI bridge for Spectre.Console.Cli
│   │   ├── TypeRegistrar.cs
│   │   └── TypeResolver.cs
│   ├── Models/            # DTOs for API responses
│   │   ├── User.cs
│   │   ├── Space.cs
│   │   └── PagedResponse.cs
│   ├── Output/            # Formatters (table, JSON)
│   │   ├── IOutputFormatter.cs
│   │   ├── TableOutputFormatter.cs
│   │   └── JsonOutputFormatter.cs
│   ├── Serialization/     # JSON source generators (trim-safe)
│   │   └── AppJsonSerializerContext.cs
│   ├── Services/          # API client services
│   │   ├── IWorkvivoApiClient.cs
│   │   └── WorkvivoApiClient.cs
│   ├── Program.cs         # Entry point
│   ├── appsettings.json   # Configuration template
│   ├── .editorconfig      # Code formatting rules
│   └── WorkvivoCli.csproj # Project file
├── tests/                 # Unit tests
├── docs/                  # Documentation
├── winget/                # Windows Package Manager manifests
├── build-all.sh           # Build script (macOS/Linux)
├── build-all.ps1          # Build script (Windows PowerShell)
├── build-all.cmd          # Build script (Windows CMD)
└── WorkvivoCli.slnx       # Solution file
```

## Configuration for Development

### User Secrets (Recommended for Local Development)

User secrets keep credentials out of source control. They are stored securely on your machine and are tied to this project.

```bash
dotnet user-secrets set "ApiToken" "your-api-token-here" --project src/WorkvivoCli.csproj
dotnet user-secrets set "OrganizationId" "your-org-id-here" --project src/WorkvivoCli.csproj
dotnet user-secrets set "BaseUrl" "https://api.workvivo.com/v1" --project src/WorkvivoCli.csproj
```

**Manage secrets:**
```bash
# List all secrets
dotnet user-secrets list --project src/WorkvivoCli.csproj

# Remove a secret
dotnet user-secrets remove "ApiToken" --project src/WorkvivoCli.csproj

# Clear all secrets
dotnet user-secrets clear --project src/WorkvivoCli.csproj
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

# Verify formatting without changes (for CI)
dotnet format WorkvivoCli.slnx --verify-no-changes
```

## Running Tests

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test /p:CollectCoverage=true

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

### Command Structure

- **Spectre.Console.Cli** for CLI framework
- **`CommandApp`** → **Branches** (`users`, `spaces`) → **`AsyncCommand<TSettings>`** classes (`list`, `get`)
- Each command is a separate class inheriting from `AsyncCommand<TSettings>`
- Command options/arguments defined declaratively via `[CommandOption]` and `[CommandArgument]` attributes on nested `Settings` classes
- All commands inherit from `GlobalSettings` base class which provides shared options (e.g., `--json`)

### Dependency Injection Flow

1. `Program.cs` builds configuration via `ConfigurationBuilder` (appsettings.json, user-secrets, env vars)
2. `WorkvivoSettings` bound and validated before DI container is built (skipped for `--help`/`--version`)
3. `ServiceCollection` configured with settings singleton and `HttpClient` factory for `IWorkvivoApiClient`
4. `TypeRegistrar` (in `Infrastructure/`) bridges `IServiceCollection` with Spectre.Console.Cli's `ITypeRegistrar`
5. Commands receive services via **constructor injection**

### Output Formatting

- Two formatters implement `IOutputFormatter`: `TableOutputFormatter` (Spectre.Console) and `JsonOutputFormatter`
- **Critical**: All logging goes to `stderr`, data output to `stdout` (enables clean JSON piping)
- Formatter selected at runtime based on `--json` flag

### JSON Serialization and Trimming

Source generators are used for JSON serialization to ensure trim-safety:
- `Serialization/AppJsonSerializerContext.cs` contains `[JsonSerializable]` attributes for all models
- All JSON operations use the pre-generated context instead of reflection
- Zero IL2026 warnings

**To add new types to serialization:**
```csharp
// In AppJsonSerializerContext.cs, add:
[JsonSerializable(typeof(YourNewType))]
[JsonSerializable(typeof(List<YourNewType>))]
```

## CI/CD

### GitHub Actions Workflow

`.github/workflows/build-release.yml` handles:
- **Build matrix**: All 5 platforms (win-x64, osx-x64, osx-arm64, linux-x64, linux-arm64)
- **Artifacts**: Creates .zip (Windows) and .tar.gz (Unix) archives with SHA256 checksums
- **Releases**: Auto-creates GitHub Release on version tags (`v*`)
- **Trigger**: Runs on tag push or manual workflow dispatch

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
- **xUnit**, **FluentAssertions**, **NSubstitute**: Testing stack