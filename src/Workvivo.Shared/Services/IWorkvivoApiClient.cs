using Workvivo.Shared.Models;

namespace Workvivo.Shared.Services;

/// <summary>
/// Interface for the Workvivo API client.
/// </summary>
public interface IWorkvivoApiClient
{
    /// <summary>
    /// Gets a list of users with optional filtering and pagination.
    /// </summary>
    Task<PagedResponse<User>> GetUsersAsync(int skip = 0, int take = 50, string? inSpaces = null, string? expand = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all users by fetching all pages.
    /// </summary>
    Task<List<User>> GetAllUsersAsync(string? inSpaces = null, string? expand = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific user by ID.
    /// </summary>
    Task<User> GetUserAsync(string userId, string? expand = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a list of spaces with optional filtering and pagination.
    /// </summary>
    Task<PagedResponse<Space>> GetSpacesAsync(int skip = 0, int take = 50, string? type = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all spaces by fetching all pages.
    /// </summary>
    Task<List<Space>> GetAllSpacesAsync(string? type = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific space by ID.
    /// </summary>
    Task<Space> GetSpaceAsync(string spaceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets users in a specific space with optional pagination.
    /// </summary>
    Task<PagedResponse<User>> GetSpaceUsersAsync(string spaceId, int skip = 0, int take = 50, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all users in a specific space by fetching all pages.
    /// </summary>
    Task<List<User>> GetAllSpaceUsersAsync(string spaceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a user to a space.
    /// </summary>
    /// <param name="spaceId">The ID of the space.</param>
    /// <param name="userId">The ID of the user to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    Task AddUserToSpaceAsync(string spaceId, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a user from a space.
    /// </summary>
    /// <param name="spaceId">The ID of the space.</param>
    /// <param name="userId">The ID of the user to remove.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    Task RemoveUserFromSpaceAsync(string spaceId, string userId, CancellationToken cancellationToken = default);
}
