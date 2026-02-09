# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Workvivo CLI (`wv`) is a .NET 10 self-contained CLI tool for managing Workvivo spaces and users via the Workvivo API. It uses Spectre.Console.Cli for command parsing and outputs data in table, JSON, or CSV format. Licensed under GPL-3.0-or-later.

## Build & Development Commands

```bash
# Build
dotnet build src/WorkvivoCli.csproj -c Release

# Run (pass CLI args after --)
dotnet run --project src/WorkvivoCli.csproj -- users list --help

# Run all tests
dotnet test

# Run tests with coverage
dotnet test /p:CollectCoverage=true

# Run a single test by name
dotnet test --filter "DisplayName~Validate_WithValidSettings"

# Format check (CI)
dotnet format WorkvivoCli.slnx --verify-no-changes

# Format fix
dotnet format WorkvivoCli.slnx

# Publish self-contained executables to dist/
./build-all.ps1   # Windows
./build-all.sh    # macOS/Linux
```

## Architecture

### Command Structure (Spectre.Console.Cli)

`Program.cs` configures the `CommandApp` with two command branches:

- `users` → `list` (ListUsersCommand), `get` (GetUserCommand)
- `spaces` → `list` (ListSpacesCommand), `get` (GetSpaceCommand), `users` (SpaceUsersCommand)

All commands inherit from `AsyncCommand<T>` where `T` extends `GlobalSettings`, which provides `--json` and `--csv` output flags.

### Dependency Injection Bridge

`Program.cs` builds configuration (appsettings.json → user-secrets → env vars), validates `WorkvivoSettings`, then creates an `IServiceCollection`. `TypeRegistrar`/`TypeResolver` in `Infrastructure/` bridge .NET DI to Spectre.Console.Cli's `ITypeRegistrar` interface, enabling constructor injection in commands.

### Output Pipeline (stderr vs stdout)

All progress/log messages go to **stderr**; all data output goes to **stdout**. This enables clean piping of `--json` or `--csv` output to files. `OutputFormatterFactory` resolves the correct `IOutputFormatter` implementation (Table, Json, or Csv) based on command flags.

### JSON Serialization & Trimming

`AppJsonSerializerContext` (in `Serialization/`) is a source-generated `JsonSerializerContext` that registers all model types. This eliminates reflection for trim-safe, AOT-compatible serialization. All new model types must be registered here.

### API Client & Pagination

`WorkvivoApiClient` uses `IHttpClientFactory` and supports paginated endpoints via `skip`/`take` parameters. `GetAll*` methods auto-page in batches of 100. Responses use `PagedResponse<T>` (generic wrapper with pagination metadata).

## Key Conventions

- **Models** use `[JsonPropertyName("snake_case")]` attributes matching the Workvivo API
- **CSV output** follows RFC 4180; each resource type has a curated column subset (no avatar URLs)
- **Test naming**: `MethodName_Scenario_ExpectedOutcome` with xUnit `[Theory]`/`[InlineData]` for parameterized tests
- **Assertions**: FluentAssertions fluent style; mocking with NSubstitute
- **Configuration precedence**: environment variables > user secrets > appsettings.json
- **Environment variables**: `WORKVIVO_APITOKEN`, `WORKVIVO_ORGANIZATIONID`, `WORKVIVO_BASEURL`
- **Code style**: Defined in `.editorconfig` — 4-space indent for C#, braces on new lines, `var` preferred when type is apparent

## Configuration for Development

```bash
dotnet user-secrets set "ApiToken" "your-token" --project src/WorkvivoCli.csproj
dotnet user-secrets set "OrganizationId" "your-org-id" --project src/WorkvivoCli.csproj
dotnet user-secrets set "BaseUrl" "https://api.workvivo.com/v1" --project src/WorkvivoCli.csproj
```
