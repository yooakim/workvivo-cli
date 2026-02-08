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
using WorkvivoCli.Services;

using static WorkvivoCli.Output.OutputFormatterFactory;

namespace WorkvivoCli.Commands;

public class ListSpacesCommand : AsyncCommand<ListSpacesCommand.Settings>
{
    private readonly IWorkvivoApiClient _apiClient;

    public ListSpacesCommand(IWorkvivoApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public sealed class Settings : GlobalSettings
    {
        [CommandOption("--skip")]
        [Description("Number of spaces to skip for pagination")]
        [DefaultValue(0)]
        public int Skip { get; init; }

        [CommandOption("--take")]
        [Description("Number of spaces to return")]
        [DefaultValue(50)]
        public int Take { get; init; }

        [CommandOption("--type")]
        [Description("Filter spaces by type (e.g., Corporate, Community)")]
        public string? Type { get; init; }

        [CommandOption("--all")]
        [Description("Fetch all spaces (ignores skip/take)")]
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
                    Console.Error.WriteLine("Fetching all spaces...");
                }

                var allSpaces = await _apiClient.GetAllSpacesAsync(settings.Type);
                formatter.FormatSpaces(allSpaces);

                if (!machineReadable)
                {
                    Console.Error.WriteLine($"\nTotal: {allSpaces.Count} spaces");
                }
            }
            else
            {
                var result = await _apiClient.GetSpacesAsync(settings.Skip, settings.Take, settings.Type);
                formatter.FormatSpaces(result.Data);

                if (!machineReadable && result.Total > 0)
                {
                    Console.Error.WriteLine($"\nShowing {result.Data.Count} of {result.Total} spaces (skip: {settings.Skip}, take: {settings.Take})");
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

public class GetSpaceCommand : AsyncCommand<GetSpaceCommand.Settings>
{
    private readonly IWorkvivoApiClient _apiClient;

    public GetSpaceCommand(IWorkvivoApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public sealed class Settings : GlobalSettings
    {
        [CommandArgument(0, "<space-id>")]
        [Description("The ID of the space to retrieve")]
        public string SpaceId { get; init; } = string.Empty;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            var space = await _apiClient.GetSpaceAsync(settings.SpaceId);

            var formatter = Create(settings);
            formatter.FormatSpace(space);

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

public class SpaceUsersCommand : AsyncCommand<SpaceUsersCommand.Settings>
{
    private readonly IWorkvivoApiClient _apiClient;

    public SpaceUsersCommand(IWorkvivoApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public sealed class Settings : GlobalSettings
    {
        [CommandArgument(0, "<space-id>")]
        [Description("The ID of the space")]
        public string SpaceId { get; init; } = string.Empty;

        [CommandOption("--skip")]
        [Description("Number of users to skip for pagination")]
        [DefaultValue(0)]
        public int Skip { get; init; }

        [CommandOption("--take")]
        [Description("Number of users to return")]
        [DefaultValue(50)]
        public int Take { get; init; }

        [CommandOption("--all")]
        [Description("Fetch all users in the space (ignores skip/take)")]
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
                    Console.Error.WriteLine($"Fetching all users in space {settings.SpaceId}...");
                }

                var allUsers = await _apiClient.GetAllSpaceUsersAsync(settings.SpaceId);
                formatter.FormatSpaceUsers(allUsers);

                if (!machineReadable)
                {
                    Console.Error.WriteLine($"\nTotal: {allUsers.Count} users in space {settings.SpaceId}");
                }
            }
            else
            {
                var result = await _apiClient.GetSpaceUsersAsync(settings.SpaceId, settings.Skip, settings.Take);
                formatter.FormatSpaceUsers(result.Data);

                if (!machineReadable && result.Total > 0)
                {
                    Console.Error.WriteLine($"\nShowing {result.Data.Count} of {result.Total} users in space {settings.SpaceId} (skip: {settings.Skip}, take: {settings.Take})");
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
