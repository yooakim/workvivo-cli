/*
 * Workvivo CLI - CLI tool for managing Workvivo spaces and users
 * Copyright (C) 2026  [Joakim Westin/Joakim Westin AB]
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using Spectre.Console.Cli;
using WorkvivoCli.Commands;
using WorkvivoCli.Configuration;
using WorkvivoCli.Infrastructure;
using WorkvivoCli.Services;

// Build configuration from appsettings.json, user-secrets, and environment variables
var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true)
    .AddUserSecrets<Program>(optional: true)
    .AddEnvironmentVariables(prefix: "WORKVIVO_")
    .Build();

// Bind settings from configuration
// With AddEnvironmentVariables(prefix: "WORKVIVO_"), env vars map directly to properties:
//   WORKVIVO_APITOKEN → ApiToken, WORKVIVO_ORGANIZATIONID → OrganizationId, WORKVIVO_BASEURL → BaseUrl
var settings = new WorkvivoSettings();
configuration.Bind(settings);

// Only validate credentials when actually running a command (not for --help or --version)
var isHelpOrVersion = args.Length == 0
    || args.Any(a => a is "--help" or "-h" or "-?" or "--version");

if (!isHelpOrVersion)
{
    try
    {
        settings.Validate();
    }
    catch (InvalidOperationException ex)
    {
        ShowConfigurationError(ex.Message);
        return 1;
    }
}

// Configure dependency injection
var services = new ServiceCollection();
services.AddSingleton(settings);
services.AddHttpClient<IWorkvivoApiClient, WorkvivoApiClient>();

// Create the Spectre.Console.Cli app with DI support
var registrar = new TypeRegistrar(services);
var app = new CommandApp(registrar);

app.Configure(config =>
{
    config.SetApplicationName("wv");
    config.SetApplicationVersion("0.1.0");

    config.AddBranch("users", users =>
    {
        users.SetDescription("Manage Workvivo users");
        users.AddCommand<ListUsersCommand>("list")
            .WithDescription("List all users");
        users.AddCommand<GetUserCommand>("get")
            .WithDescription("Get a specific user by ID");
    });

    config.AddBranch("spaces", spaces =>
    {
        spaces.SetDescription("Manage Workvivo spaces");
        spaces.AddCommand<ListSpacesCommand>("list")
            .WithDescription("List all spaces");
        spaces.AddCommand<GetSpaceCommand>("get")
            .WithDescription("Get a specific space by ID");
        spaces.AddCommand<SpaceUsersCommand>("users")
            .WithDescription("List users in a specific space");
    });
});

return app.Run(args);

/// <summary>
/// Displays a rich configuration error message to stderr with setup instructions.
/// </summary>
static void ShowConfigurationError(string message)
{
    var errorConsole = AnsiConsole.Create(new AnsiConsoleSettings
    {
        Ansi = AnsiSupport.Detect,
        ColorSystem = ColorSystemSupport.Detect,
        Out = new AnsiConsoleOutput(Console.Error)
    });

    errorConsole.Write(
        new Panel(new Markup($"[red bold]Configuration Error:[/] {message}"))
            .Border(BoxBorder.Rounded)
            .BorderColor(Color.Red)
            .Header("[red][[Error]][/]")
    );

    errorConsole.WriteLine();
    errorConsole.MarkupLine("[yellow]Please configure your credentials using one of these methods:[/]");
    errorConsole.WriteLine();

    var table = new Table()
        .Border(TableBorder.Rounded)
        .BorderColor(Color.Grey)
        .AddColumn(new TableColumn("[cyan]Method[/]"))
        .AddColumn(new TableColumn("[cyan]Description[/]"))
        .AddColumn(new TableColumn("[cyan]Example[/]"));

    table.AddRow(
        "[green]1. Environment Variables[/]",
        "Recommended for production",
        "[dim]WORKVIVO_APITOKEN=your-token\nWORKVIVO_ORGANIZATIONID=your-org-id[/]"
    );

    table.AddRow(
        "[green]2. User Secrets[/]",
        "Recommended for local development",
        "[dim]dotnet user-secrets set \"ApiToken\" \"your-token\"[/]"
    );

    table.AddRow(
        "[green]3. appsettings.json[/]",
        "Fallback (avoid committing credentials)",
        "[dim]{ \"ApiToken\": \"...\", \"OrganizationId\": \"...\" }[/]"
    );

    errorConsole.Write(table);
    errorConsole.WriteLine();
}
