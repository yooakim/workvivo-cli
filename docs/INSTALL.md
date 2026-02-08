# Installation Guide

This guide covers installing and configuring the Workvivo CLI.

## Prerequisites

**None** - Self-contained executables include the .NET runtime. No installation required.

## Installation

### Windows (via winget)

```powershell
# Install using Windows Package Manager (recommended)
winget install wv
```

### macOS

1. Download the latest `wv-macos-x64.tar.gz` from [GitHub Releases](https://github.com/yooakim/workvivo-cli/releases)
2. Extract the archive:
   ```bash
   tar -xzf wv-macos-x64.tar.gz
   ```
3. (Optional) Move to PATH:
   ```bash
   sudo mv wv /usr/local/bin/
   ```
4. Run the CLI:
   ```bash
   wv --help
   ```

### macOS (Apple Silicon)

1. Download the latest `wv-macos-arm64.tar.gz` from [GitHub Releases](https://github.com/yooakim/workvivo-cli/releases)
2. Extract the archive:
   ```bash
   tar -xzf wv-macos-arm64.tar.gz
   ```
3. (Optional) Move to PATH:
   ```bash
   sudo mv wv /usr/local/bin/
   ```
4. Run the CLI:
   ```bash
   wv --help
   ```

### Linux

1. Download the latest `wv-linux-x64.tar.gz` from [GitHub Releases](https://github.com/yooakim/workvivo-cli/releases)
2. Extract the archive:
   ```bash
   tar -xzf wv-linux-x64.tar.gz
   ```
3. (Optional) Move to PATH:
   ```bash
   sudo mv wv /usr/local/bin/
   ```
4. Run the CLI:
   ```bash
   wv --help
   ```

### Linux (ARM64)

1. Download the latest `wv-linux-arm64.tar.gz` from [GitHub Releases](https://github.com/yooakim/workvivo-cli/releases)
2. Extract the archive:
   ```bash
   tar -xzf wv-linux-arm64.tar.gz
   ```
3. (Optional) Move to PATH:
   ```bash
   sudo mv wv /usr/local/bin/
   ```
4. Run the CLI:
   ```bash
   wv --help
   ```

### Verify Installation

After installing, verify that the CLI is working:

```bash
wv --version
```

## Configuration

The CLI requires authentication credentials to access the Workvivo API. You can configure these in several ways.

### Option 1: appsettings.json

Create or edit `appsettings.json` in the same directory as the executable:

```json
{
  "BaseUrl": "https://api.workvivo.com/v1",
  "ApiToken": "your-api-token-here",
  "OrganizationId": "your-org-id-here"
}
```

### Option 2: Environment Variables (Recommended for production)

Set the following environment variables:

**macOS/Linux (Bash/Zsh):**
```bash
export WORKVIVO_APITOKEN="your-api-token-here"
export WORKVIVO_ORGANIZATIONID="your-org-id-here"
export WORKVIVO_BASEURL="https://api.workvivo.com/v1"  # Optional
```

**Windows (PowerShell):**
```powershell
$env:WORKVIVO_APITOKEN="your-api-token-here"
$env:WORKVIVO_ORGANIZATIONID="your-org-id-here"
$env:WORKVIVO_BASEURL="https://api.workvivo.com/v1"  # Optional
```

**Windows (CMD):**
```cmd
set WORKVIVO_APITOKEN=your-api-token-here
set WORKVIVO_ORGANIZATIONID=your-org-id-here
set WORKVIVO_BASEURL=https://api.workvivo.com/v1
```

To set environment variables permanently on Windows, use **System Properties → Environment Variables**, or in PowerShell:

```powershell
[Environment]::SetEnvironmentVariable("WORKVIVO_APITOKEN", "your-api-token-here", "User")
[Environment]::SetEnvironmentVariable("WORKVIVO_ORGANIZATIONID", "your-org-id-here", "User")
[Environment]::SetEnvironmentVariable("WORKVIVO_BASEURL", "https://api.workvivo.com/v1", "User")
```

### Configuration Key Reference

All configuration methods map to the same `WorkvivoSettings` properties. Use this table to find the correct key name for each method:

| Setting | `appsettings.json` | User Secrets | Environment Variable |
|---|---|---|---|
| API Token | `"ApiToken"` | `ApiToken` | `WORKVIVO_APITOKEN` |
| Organization ID | `"OrganizationId"` | `OrganizationId` | `WORKVIVO_ORGANIZATIONID` |
| Base URL | `"BaseUrl"` | `BaseUrl` | `WORKVIVO_BASEURL` |

### Configuration Precedence

**Highest to lowest:** Environment variables → User secrets → appsettings.json

> **Tip:** For local development, [User Secrets](DEVELOPMENT.md#user-secrets-recommended-for-local-development) are the recommended way to store credentials without risking them being committed to source control.

## Obtaining API Credentials

The Workvivo API requires the following for authentication:

- **API Token** — a Bearer token for the `Authorization` header
- **Organization ID** — sent via the `Workvivo-Id` header

To obtain your API credentials:

1. Log in to your Workvivo admin panel
2. Navigate to the API section
3. Generate a new Access Token
4. Note your Organization ID

For more information, visit the [Workvivo Developer Portal](https://developer.workvivo.com).

## Uninstalling

### Windows (winget)

```powershell
winget uninstall wv
```

### macOS / Linux

Remove the executable from where you placed it:

```bash
sudo rm /usr/local/bin/wv
```

If you extracted it to a different location, remove it from there instead.

## Troubleshooting

### "Missing Credentials" error

The CLI could not find your API token or Organization ID. Make sure you have configured at least one of the following:
- `appsettings.json` in the same directory as the executable
- Environment variables (`WORKVIVO_APITOKEN`, `WORKVIVO_ORGANIZATIONID`)
- User secrets (development only — see [Development Guide](DEVELOPMENT.md#user-secrets-recommended-for-local-development))

### "Permission denied" on macOS/Linux

If you get a permission error when running the CLI after extracting:

```bash
chmod +x wv
```

### macOS Gatekeeper warning

If macOS blocks the executable because it's from an unidentified developer:

```bash
# Remove the quarantine attribute
xattr -d com.apple.quarantine wv
```

### Checking your configuration

Run any command to verify connectivity:

```bash
wv users list --take 1
```

If the configuration is correct, you should see a table with one user. If not, the CLI will display a detailed error message explaining what's missing.
