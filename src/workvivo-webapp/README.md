# Workvivo Web App

An experimental Blazor Server web application that displays Workvivo users and spaces using the QuickGrid component.

## Features

- **Tabbed Interface**: Switch between Users and Spaces views
- **QuickGrid Display**: Sortable columns with client-side filtering
- **Caching**: In-memory cache (5-minute expiration) for fast data access
- **Real-time Search**: Filter users by name/email, spaces by name/visibility
- **Refresh Button**: Manual data refresh with loading indicators
- **Responsive Design**: Works on desktop, tablet, and mobile devices

## Prerequisites

- .NET 10 SDK
- Workvivo API credentials (same as CLI)

## Configuration

The web app shares configuration with the CLI. You have three options:

### Option 1: User Secrets (Recommended for Development)

The webapp shares the same UserSecretsId as the CLI, so you can configure once:

```bash
# Set credentials (run from either CLI or webapp directory)
dotnet user-secrets set "ApiToken" "your-api-token" --project src/workvivo-webapp
dotnet user-secrets set "OrganizationId" "your-org-id" --project src/workvivo-webapp
dotnet user-secrets set "BaseUrl" "https://api.workvivo.com/v1" --project src/workvivo-webapp

# Or if already configured for CLI, the webapp will automatically use them
```

### Option 2: Environment Variables

```bash
export WORKVIVO_APITOKEN="your-api-token"
export WORKVIVO_ORGANIZATIONID="your-org-id"
export WORKVIVO_BASEURL="https://api.workvivo.com/v1"
```

### Option 3: appsettings.json (Not Recommended for Credentials)

Edit `src/workvivo-webapp/appsettings.json`:

```json
{
  "ApiToken": "your-api-token",
  "OrganizationId": "your-org-id",
  "BaseUrl": "https://api.workvivo.com/v1"
}
```

## Running the Application

### Development Mode

```bash
# Navigate to project directory
cd src/workvivo-webapp

# Run with hot reload
dotnet watch run

# Or run without hot reload
dotnet run
```

The application will start on:
- HTTP: http://localhost:5000
- HTTPS: https://localhost:5001

### Production Build

```bash
# Build release version
dotnet build -c Release

# Publish self-contained (optional)
dotnet publish -c Release
```

## Usage

1. **Start the app**: Run `dotnet run` or `dotnet watch run`
2. **Open browser**: Navigate to `https://localhost:5001`
3. **View data**: The app automatically loads all users and spaces on startup
4. **Search**: Use the search boxes to filter by name, email, or visibility
5. **Sort**: Click column headers to sort data
6. **Refresh**: Click "Refresh Data" button to reload from API

## Architecture

### Project Structure

```
src/workvivo-webapp/
├── Components/
│   ├── Layout/
│   │   ├── MainLayout.razor       # Main layout with sidebar
│   │   └── NavMenu.razor          # Navigation menu
│   └── Pages/
│       └── Home.razor             # Main page with QuickGrid
├── Services/
│   └── WorkvivoDataService.cs     # Caching layer for API calls
├── wwwroot/
│   └── app.css                    # Custom styles
├── Program.cs                     # DI configuration
├── appsettings.json               # Configuration
└── workvivo-webapp.csproj         # Project file
```

### Key Components

#### WorkvivoDataService
- **Purpose**: Caching layer between UI and API client
- **Cache Keys**: `users-all`, `spaces-all`
- **Expiration**: 5 minutes (configurable in appsettings.json)
- **Methods**:
  - `GetCachedUsersAsync()` - Fetch/cache all users
  - `GetCachedSpacesAsync()` - Fetch/cache all spaces
  - `RefreshDataAsync()` - Clear cache and reload

#### Home.razor
- **Features**:
  - Tabbed interface (Users/Spaces)
  - QuickGrid with sorting
  - Client-side filtering (search box)
  - Loading states and error handling
  - Refresh button

### Data Flow

1. **Startup**: `Home.razor` calls `DataService.GetCachedUsersAsync()` and `GetCachedSpacesAsync()`
2. **First Request**: Service fetches from API via `IWorkvivoApiClient`, caches result
3. **Subsequent Requests**: Service returns cached data if not expired
4. **Refresh**: User clicks button → Service clears cache → Fetches fresh data

## Configuration Options

### Cache Settings

Edit `appsettings.json`:

```json
{
  "CacheSettings": {
    "ExpirationMinutes": 5
  }
}
```

### Logging

Adjust log levels in `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "workvivo_webapp.Services": "Debug"
    }
  }
}
```

## Troubleshooting

### "Configuration Error" on Startup

**Cause**: Missing API credentials

**Solution**: Configure credentials using one of the three methods above

### No Data Displayed

**Cause**: API call failed or returned empty results

**Solution**: 
1. Check console logs for error messages
2. Verify credentials are correct
3. Test CLI commands to confirm API access

### Port Already in Use

**Solution**: Change port in `Properties/launchSettings.json` or specify at runtime:

```bash
dotnet run --urls "http://localhost:5002;https://localhost:5003"
```

## Limitations

- **Authentication**: No user authentication (assumes trusted network)
- **Authorization**: No role-based access control
- **Scalability**: In-memory cache (not suitable for multiple instances)
- **Data Size**: Fetches all data at once (may be slow for large datasets)

## Future Enhancements

- Pagination for large datasets
- Real-time updates via SignalR
- User/space detail pages
- Export to CSV/Excel
- Advanced filtering (date ranges, multi-select)
- Authentication and authorization
- Deployment configurations (Docker, Azure)

## Related Projects

- **CLI Tool**: `src/workvivo-cli` - Command-line interface
- **Shared Library**: `src/Workvivo.Shared` - API client and models (shared by both)

## License

Same as parent project (GPL-3.0-or-later)
