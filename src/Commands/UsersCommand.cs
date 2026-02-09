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
using System.ComponentModel;
using Spectre.Console.Cli;
using WorkvivoCli.Output;
using Workvivo.Shared.Services;

using static WorkvivoCli.Output.OutputFormatterFactory;

namespace WorkvivoCli.Commands;

public class ListUsersCommand : AsyncCommand<ListUsersCommand.Settings>
{
    private readonly IWorkvivoApiClient _apiClient;

    public ListUsersCommand(IWorkvivoApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public sealed class Settings : GlobalSettings
    {
        [CommandOption("--skip")]
        [Description("Number of users to skip for pagination")]
        [DefaultValue(0)]
        public int Skip { get; init; }

        [CommandOption("--take")]
        [Description("Number of users to return")]
        [DefaultValue(50)]
        public int Take { get; init; }

        [CommandOption("--in-spaces")]
        [Description("Filter users by space IDs (pipe-separated, e.g., space1|space2)")]
        public string? InSpaces { get; init; }

        [CommandOption("--expand")]
        [Description("Expand related objects (e.g., teams)")]
        public string? Expand { get; init; }

        [CommandOption("--all")]
        [Description("Fetch all users (ignores skip/take)")]
        [DefaultValue(false)]
        public bool All { get; init; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            var formatter = Create(settings);
            var machineReadable = IsMachineReadable(settings);

            if (settings.All)
            {
                if (!machineReadable)
                {
                    Console.Error.WriteLine("Fetching all users...");
                }

                var allUsers = await _apiClient.GetAllUsersAsync(settings.InSpaces, settings.Expand);
                formatter.FormatUsers(allUsers);

                if (!machineReadable)
                {
                    Console.Error.WriteLine($"\nTotal: {allUsers.Count} users");
                }
            }
            else
            {
                var result = await _apiClient.GetUsersAsync(settings.Skip, settings.Take, settings.InSpaces, settings.Expand);
                formatter.FormatUsers(result.Data);

                if (!machineReadable && result.Total > 0)
                {
                    Console.Error.WriteLine($"\nShowing {result.Data.Count} of {result.Total} users (skip: {settings.Skip}, take: {settings.Take})");
                }
            }

            return 0;
        }
        catch (HttpRequestException ex)
        {
            Console.Error.WriteLine($"Error: API request failed - {ex.Message}");
            return 1;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }
}

public class GetUserCommand : AsyncCommand<GetUserCommand.Settings>
{
    private readonly IWorkvivoApiClient _apiClient;

    public GetUserCommand(IWorkvivoApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public sealed class Settings : GlobalSettings
    {
        [CommandArgument(0, "<user-id>")]
        [Description("The ID of the user to retrieve")]
        public string UserId { get; init; } = string.Empty;

        [CommandOption("--expand")]
        [Description("Expand related objects (e.g., teams)")]
        public string? Expand { get; init; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            var user = await _apiClient.GetUserAsync(settings.UserId, settings.Expand);

            var formatter = Create(settings);
            formatter.FormatUser(user);

            return 0;
        }
        catch (HttpRequestException ex)
        {
            Console.Error.WriteLine($"Error: API request failed - {ex.Message}");
            return 1;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }
}
