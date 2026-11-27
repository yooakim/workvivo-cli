using System.ComponentModel;
using ModelContextProtocol.Server;
using Workvivo.Shared.Models;
using Workvivo.Shared.Services;

namespace WorkvivoMcpServer.Tools;

[McpServerToolType]
public class SpaceTools
{
    private readonly IWorkvivoApiClient _apiClient;

    public SpaceTools(IWorkvivoApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    [McpServerTool]
    [Description("List spaces from Workvivo with optional pagination and filtering")]
    public async Task<List<Space>> ListSpaces(
        [Description("Number of spaces to skip for pagination (default: 0)")] int skip = 0,
        [Description("Number of spaces to return (default: 50, max: 100)")] int take = 50,
        [Description("Filter spaces by type (e.g., 'Corporate', 'Community')")] string? type = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _apiClient.GetSpacesAsync(skip, take, type, cancellationToken);
            return response.Data;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error listing spaces: {ex.Message}");
            throw;
        }
    }

    [McpServerTool]
    [Description("Get a specific space by ID from Workvivo")]
    public async Task<Space> GetSpace(
        [Description("The ID of the space to retrieve")] string spaceId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _apiClient.GetSpaceAsync(spaceId, cancellationToken);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error getting space {spaceId}: {ex.Message}");
            throw;
        }
    }

    [McpServerTool]
    [Description("List users in a specific space with their roles")]
    public async Task<List<User>> ListSpaceUsers(
        [Description("The ID of the space")] string spaceId,
        [Description("Number of users to skip for pagination (default: 0)")] int skip = 0,
        [Description("Number of users to return (default: 50, max: 100)")] int take = 50,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _apiClient.GetSpaceUsersAsync(spaceId, skip, take, cancellationToken);
            return response.Data;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error listing users in space {spaceId}: {ex.Message}");
            throw;
        }
    }

    [McpServerTool]
    [Description("Get all users in a specific space (fetches all pages automatically)")]
    public async Task<List<User>> GetAllSpaceUsers(
        [Description("The ID of the space")] string spaceId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _apiClient.GetAllSpaceUsersAsync(spaceId, cancellationToken);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error getting all users in space {spaceId}: {ex.Message}");
            throw;
        }
    }

    [McpServerTool]
    [Description("Get all spaces (fetches all pages automatically)")]
    public async Task<List<Space>> GetAllSpaces(
        [Description("Filter spaces by type (e.g., 'Corporate', 'Community')")] string? type = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _apiClient.GetAllSpacesAsync(type, cancellationToken);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error getting all spaces: {ex.Message}");
            throw;
        }
    }

    [McpServerTool]
    [Description("Search for spaces by name (case-insensitive partial matching)")]
    public async Task<List<Space>> SearchSpacesByName(
        [Description("Name to search for (partial matches supported)")] string nameQuery,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var allSpaces = await _apiClient.GetAllSpacesAsync(cancellationToken: cancellationToken);
            
            var nameQueryLower = nameQuery.ToLower();
            var matches = allSpaces
                .Where(s => s.Name?.ToLower().Contains(nameQueryLower) == true)
                .ToList();
            
            return matches;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error searching spaces by name: {ex.Message}");
            throw;
        }
    }
}
