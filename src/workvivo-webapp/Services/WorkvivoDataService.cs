using Microsoft.Extensions.Caching.Memory;
using Workvivo.Shared.Models;
using Workvivo.Shared.Services;

namespace workvivo_webapp.Services;

public interface IWorkvivoDataService
{
    Task<List<User>> GetCachedUsersAsync(CancellationToken cancellationToken = default);
    Task<List<Space>> GetCachedSpacesAsync(CancellationToken cancellationToken = default);
    Task<List<User>> GetSpaceMembersAsync(string spaceId, CancellationToken cancellationToken = default);
    Task RefreshDataAsync(CancellationToken cancellationToken = default);
    Task AddUserToSpaceAsync(string spaceId, string userId, CancellationToken cancellationToken = default);
    Task RemoveUserFromSpaceAsync(string spaceId, string userId, CancellationToken cancellationToken = default);
    DateTime? LastRefreshTime { get; }
}

public class WorkvivoDataService : IWorkvivoDataService
{
    private readonly IWorkvivoApiClient _apiClient;
    private readonly IMemoryCache _cache;
    private readonly IConfiguration _configuration;
    private readonly ILogger<WorkvivoDataService> _logger;
    private const string UsersCacheKey = "users-all";
    private const string SpacesCacheKey = "spaces-all";
    private const string LastRefreshCacheKey = "last-refresh-time";
    private const string SpaceMembersCacheKeyPrefix = "space-members-";

    public DateTime? LastRefreshTime => _cache.Get<DateTime?>(LastRefreshCacheKey);

    public WorkvivoDataService(
        IWorkvivoApiClient apiClient,
        IMemoryCache cache,
        IConfiguration configuration,
        ILogger<WorkvivoDataService> logger)
    {
        _apiClient = apiClient;
        _cache = cache;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<List<User>> GetCachedUsersAsync(CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue<List<User>>(UsersCacheKey, out var cachedUsers) && cachedUsers != null)
        {
            _logger.LogDebug("Returning cached users (count: {Count})", cachedUsers.Count);
            return cachedUsers;
        }

        _logger.LogInformation("Fetching all users from API...");
        var users = await _apiClient.GetAllUsersAsync(cancellationToken: cancellationToken);

        var cacheExpiration = _configuration.GetValue<int>("CacheSettings:ExpirationMinutes", 5);
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(cacheExpiration));

        _cache.Set(UsersCacheKey, users, cacheOptions);
        _cache.Set(LastRefreshCacheKey, DateTime.UtcNow, cacheOptions);

        _logger.LogInformation("Cached {Count} users", users.Count);
        return users;
    }

    public async Task<List<Space>> GetCachedSpacesAsync(CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue<List<Space>>(SpacesCacheKey, out var cachedSpaces) && cachedSpaces != null)
        {
            _logger.LogDebug("Returning cached spaces (count: {Count})", cachedSpaces.Count);
            return cachedSpaces;
        }

        _logger.LogInformation("Fetching all spaces from API...");
        var spaces = await _apiClient.GetAllSpacesAsync(cancellationToken: cancellationToken);

        var cacheExpiration = _configuration.GetValue<int>("CacheSettings:ExpirationMinutes", 5);
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(cacheExpiration));

        _cache.Set(SpacesCacheKey, spaces, cacheOptions);
        _cache.Set(LastRefreshCacheKey, DateTime.UtcNow, cacheOptions);

        _logger.LogInformation("Cached {Count} spaces", spaces.Count);
        return spaces;
    }

    public async Task RefreshDataAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Clearing cache and refreshing data...");
        _cache.Remove(UsersCacheKey);
        _cache.Remove(SpacesCacheKey);
        _cache.Remove(LastRefreshCacheKey);

        // Pre-fetch both to warm up cache
        await Task.WhenAll(
            GetCachedUsersAsync(cancellationToken),
            GetCachedSpacesAsync(cancellationToken)
        );

        _logger.LogInformation("Data refresh complete");
    }

    public async Task<List<User>> GetSpaceMembersAsync(string spaceId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{SpaceMembersCacheKeyPrefix}{spaceId}";

        if (_cache.TryGetValue<List<User>>(cacheKey, out var cachedMembers) && cachedMembers != null)
        {
            _logger.LogDebug("Returning cached members for space {SpaceId} (count: {Count})", spaceId, cachedMembers.Count);
            return cachedMembers;
        }

        _logger.LogInformation("Fetching members for space {SpaceId} from API...", spaceId);
        var members = await _apiClient.GetAllSpaceUsersAsync(spaceId, cancellationToken);

        var cacheExpiration = _configuration.GetValue<int>("CacheSettings:ExpirationMinutes", 5);
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(cacheExpiration));

        _cache.Set(cacheKey, members, cacheOptions);

        _logger.LogInformation("Cached {Count} members for space {SpaceId}", members.Count, spaceId);
        return members;
    }

    public async Task AddUserToSpaceAsync(string spaceId, string userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding user {UserId} to space {SpaceId}...", userId, spaceId);

        await _apiClient.AddUserToSpaceAsync(spaceId, userId, cancellationToken);

        // Invalidate affected caches
        _cache.Remove($"{SpaceMembersCacheKeyPrefix}{spaceId}");
        _logger.LogInformation("Successfully added user {UserId} to space {SpaceId}", userId, spaceId);
    }

    public async Task RemoveUserFromSpaceAsync(string spaceId, string userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Removing user {UserId} from space {SpaceId}...", userId, spaceId);

        await _apiClient.RemoveUserFromSpaceAsync(spaceId, userId, cancellationToken);

        // Invalidate affected caches
        _cache.Remove($"{SpaceMembersCacheKeyPrefix}{spaceId}");
        _logger.LogInformation("Successfully removed user {UserId} from space {SpaceId}", userId, spaceId);
    }
}
