using Microsoft.Extensions.Caching.Memory;
using Workvivo.Shared.Models;
using Workvivo.Shared.Services;

namespace workvivo_webapp.Services;

public interface IWorkvivoDataService
{
    Task<List<User>> GetCachedUsersAsync(CancellationToken cancellationToken = default);
    Task<List<Space>> GetCachedSpacesAsync(CancellationToken cancellationToken = default);
    Task RefreshDataAsync(CancellationToken cancellationToken = default);
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
}
