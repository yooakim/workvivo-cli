using System.ComponentModel;
using ModelContextProtocol.Server;
using Workvivo.Shared.Models;
using Workvivo.Shared.Services;

namespace WorkvivoMcpServer.Tools;

[McpServerToolType]
public class UserTools
{
    private readonly IWorkvivoApiClient _apiClient;

    public UserTools(IWorkvivoApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    [McpServerTool]
    [Description("List users from Workvivo with optional pagination and filtering")]
    public async Task<List<User>> ListUsers(
        [Description("Number of users to skip for pagination (default: 0)")] int skip = 0,
        [Description("Number of users to return (default: 50, max: 100)")] int take = 50,
        [Description("Filter users by space IDs (pipe-separated, e.g., 'space1|space2')")] string? inSpaces = null,
        [Description("Expand related objects (e.g., 'teams')")] string? expand = null,
        CancellationToken cancellationToken = default)
    {
        take = Math.Clamp(take, 1, 100);
        try
        {
            var response = await _apiClient.GetUsersAsync(skip: skip, take: take, inSpaces: inSpaces, expand: expand, cancellationToken: cancellationToken);
            return response.Data;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error listing users: {ex.Message}");
            throw;
        }
    }

    [McpServerTool]
    [Description("Get a specific user by ID from Workvivo")]
    public async Task<User> GetUser(
        [Description("The ID of the user to retrieve")] string userId,
        [Description("Expand related objects (e.g., 'teams')")] string? expand = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _apiClient.GetUserAsync(userId, expand, cancellationToken);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error getting user {userId}: {ex.Message}");
            throw;
        }
    }

    [McpServerTool]
    [Description("Search for users by name (case-insensitive fuzzy matching)")]
    public async Task<List<User>> SearchUsersByName(
        [Description("Name to search for (partial matches supported)")] string nameQuery,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var allUsers = await _apiClient.GetAllUsersAsync(cancellationToken: cancellationToken);
            
            var matches = allUsers
                .Where(u => 
                    u.DisplayName?.Contains(nameQuery, StringComparison.OrdinalIgnoreCase) == true ||
                    u.Name?.Contains(nameQuery, StringComparison.OrdinalIgnoreCase) == true ||
                    u.FirstName?.Contains(nameQuery, StringComparison.OrdinalIgnoreCase) == true ||
                    u.LastName?.Contains(nameQuery, StringComparison.OrdinalIgnoreCase) == true)
                .ToList();
            
            return matches;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error searching users by name: {ex.Message}");
            throw;
        }
    }

    [McpServerTool]
    [Description("Search for users by email address (partial matches supported)")]
    public async Task<List<User>> SearchUsersByEmail(
        [Description("Email to search for (partial matches supported)")] string emailQuery,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var allUsers = await _apiClient.GetAllUsersAsync(cancellationToken: cancellationToken);
            
            var matches = allUsers
                .Where(u => u.Email?.Contains(emailQuery, StringComparison.OrdinalIgnoreCase) == true)
                .ToList();
            
            return matches;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error searching users by email: {ex.Message}");
            throw;
        }
    }

    [McpServerTool]
    [Description("Get all users (fetches all pages automatically - may take time for large organizations)")]
    public async Task<List<User>> GetAllUsers(
        [Description("Filter users by space IDs (pipe-separated)")] string? inSpaces = null,
        [Description("Expand related objects (e.g., 'teams')")] string? expand = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _apiClient.GetAllUsersAsync(inSpaces: inSpaces, expand: expand, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error getting all users: {ex.Message}");
            throw;
        }
    }
}
