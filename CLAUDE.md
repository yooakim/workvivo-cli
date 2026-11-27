# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Workvivo CLI (`wv`) is a .NET 10 self-contained CLI tool for managing Workvivo spaces and users via the Workvivo API. It uses Spectre.Console.Cli for command parsing and outputs data in table, JSON, or CSV format. The repository also contains a read-only MCP server exposing the same API to AI assistants. Licensed under GPL-3.0-or-later.

## Repository Layout

| Path | Project | Purpose |
|------|---------|---------|
| `src/Workvivo.Shared/` | `Workvivo.Shared.csproj` (library) | Code shared by both executables: `Configuration/`, `Models/`, `Serialization/`, `Services/`. Namespace `Workvivo.Shared.*` |
| `src/workvivo-cli/` | `WorkvivoCli.csproj` (exe, assembly `wv`) | The CLI: `Commands/`, `Infrastructure/`, `Output/`, `Program.cs`. Namespace `WorkvivoCli.*` |
| `src/workvivo-mcp-server/` | `WorkvivoMcpServer.csproj` (exe) | MCP server over STDIO: `Tools/`, `Program.cs`. Has its own `README.md` |
| `tests/WorkvivoCli.Tests/` | `WorkvivoCli.Tests.csproj` | xUnit tests for the CLI and shared library |

`WorkvivoCli.slnx` includes all four projects. Both executables reference `Workvivo.Shared`; they do not reference each other.

## Build & Development Commands

```bash
# Build everything (shared library, CLI, MCP server, tests)
dotnet build WorkvivoCli.slnx

# Build only the CLI
dotnet build src/workvivo-cli/WorkvivoCli.csproj -c Release

# Run the CLI (pass CLI args after --)
dotnet run --project src/workvivo-cli/WorkvivoCli.csproj -- users list --help

# Run the MCP server (STDIO; needs credentials configured)
dotnet run --project src/workvivo-mcp-server/WorkvivoMcpServer.csproj

# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run a single test by name
dotnet test --filter "DisplayName~Validate_WithValidSettings"

# Format check (verify only, writes nothing; not enforced by any workflow)
dotnet format WorkvivoCli.slnx --verify-no-changes

# Format fix
dotnet format WorkvivoCli.slnx

# Publish self-contained CLI executables to dist/
./build-all.ps1   # Windows
./build-all.sh    # macOS/Linux
```

## Architecture

### Shared Library (`Workvivo.Shared`)

Holds everything that is not specific to one front end: `WorkvivoSettings` (configuration + validation), the API models, `AppJsonSerializerContext`, and `IWorkvivoApiClient`/`WorkvivoApiClient`. New API endpoints and models go here so both the CLI and the MCP server can use them.

### Command Structure (Spectre.Console.Cli)

`src/workvivo-cli/Program.cs` configures the `CommandApp` with two command branches:

- `users` → `list` (ListUsersCommand), `get` (GetUserCommand)
- `spaces` → `list` (ListSpacesCommand), `get` (GetSpaceCommand), `users` (SpaceUsersCommand)

All commands inherit from `AsyncCommand<T>` where `T` extends `GlobalSettings`, which provides `--json` and `--csv` output flags.

### Dependency Injection Bridge

The CLI's `Program.cs` builds configuration (appsettings.json → user-secrets → env vars), validates `WorkvivoSettings`, then creates an `IServiceCollection`. `TypeRegistrar`/`TypeResolver` in `src/workvivo-cli/Infrastructure/` bridge .NET DI to Spectre.Console.Cli's `ITypeRegistrar` interface, enabling constructor injection in commands.

### Output Pipeline (stderr vs stdout)

All progress/log messages go to **stderr**; all data output goes to **stdout**. This enables clean piping of `--json` or `--csv` output to files. `OutputFormatterFactory` (in `src/workvivo-cli/Output/`) resolves the correct `IOutputFormatter` implementation (Table, Json, or Csv) based on command flags.

### JSON Serialization & Trimming

`AppJsonSerializerContext` (in `src/Workvivo.Shared/Serialization/`) is a source-generated `JsonSerializerContext` that registers all model types. This eliminates reflection for trim-safe, AOT-compatible serialization. All new model types must be registered here.

### API Client & Pagination

`WorkvivoApiClient` (in `src/Workvivo.Shared/Services/`) uses `IHttpClientFactory` and supports paginated endpoints via `skip`/`take` parameters. `GetAll*` methods auto-page in batches of 100. Responses use `PagedResponse<T>` (generic wrapper with pagination metadata).

### MCP Server (`workvivo-mcp-server`)

`Program.cs` loads and validates the same `WorkvivoSettings`, then hosts an MCP server with `AddMcpServer().WithStdioServerTransport().WithToolsFromAssembly()` (ModelContextProtocol SDK). Tools live in `Tools/UserTools.cs` and `Tools/SpaceTools.cs`: classes marked `[McpServerToolType]`, methods marked `[McpServerTool]` with `[Description]` on the method and each parameter, taking `IWorkvivoApiClient` by constructor injection. All tools are read-only.

Because the transport is STDIO, **stdout belongs to the protocol**: anything the server logs must go to `Console.Error`.

`mcp.json.template` (repo root) is the client config template; copy it to `.vscode/mcp.json`, which is gitignored because it holds credentials.

## Key Conventions

- **Namespaces**: `Workvivo.Shared.*` for shared code, `WorkvivoCli.*` for the CLI, `WorkvivoMcpServer.*` for the MCP server
- **Models** use `[JsonPropertyName("snake_case")]` attributes matching the Workvivo API
- **CSV output** follows RFC 4180; each resource type has a curated column subset (no avatar URLs)
- **Test naming**: `MethodName_Scenario_ExpectedOutcome` with xUnit `[Theory]`/`[InlineData]` for parameterized tests
- **Assertions**: FluentAssertions fluent style; mocking with NSubstitute
- **Configuration precedence**: environment variables > user secrets > appsettings.json
- **Environment variables**: `WORKVIVO_APITOKEN`, `WORKVIVO_ORGANIZATIONID`, `WORKVIVO_BASEURL`
- **Code style**: Defined in `.editorconfig` — 4-space indent for C#, braces on new lines, `var` preferred when type is apparent

## Configuration for Development

The CLI and the MCP server have separate user-secrets stores (different `UserSecretsId`), so set secrets for each project you run. Environment variables apply to both.

```bash
dotnet user-secrets set "ApiToken" "your-token" --project src/workvivo-cli/WorkvivoCli.csproj
dotnet user-secrets set "OrganizationId" "your-org-id" --project src/workvivo-cli/WorkvivoCli.csproj
dotnet user-secrets set "BaseUrl" "https://api.workvivo.com/v1" --project src/workvivo-cli/WorkvivoCli.csproj

# Same keys for the MCP server
dotnet user-secrets set "ApiToken" "your-token" --project src/workvivo-mcp-server/WorkvivoMcpServer.csproj
```
