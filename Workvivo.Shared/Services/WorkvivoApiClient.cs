using System.Net.Http.Json;
using Workvivo.Shared.Configuration;
using Workvivo.Shared.Models;
using Workvivo.Shared.Serialization;

namespace Workvivo.Shared.Services;

public class WorkvivoApiClient : IWorkvivoApiClient
{
    private readonly HttpClient _httpClient;
    private readonly WorkvivoSettings _settings;

    public WorkvivoApiClient(HttpClient httpClient, WorkvivoSettings settings)
    {
        _httpClient = httpClient;
        _settings = settings;

        // Ensure BaseUrl ends with a slash for proper relative URL resolution
        var baseUrl = _settings.BaseUrl.TrimEnd('/') + "/";
        _httpClient.BaseAddress = new Uri(baseUrl);
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.ApiToken}");
        _httpClient.DefaultRequestHeaders.Add("Workvivo-Id", _settings.OrganizationId);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    public async Task<PagedResponse<User>> GetUsersAsync(
        int skip = 0,
        int take = 50,
        string? inSpaces = null,
        string? expand = null,
        CancellationToken cancellationToken = default)
    {
        var queryParams = new List<string>
        {
            $"skip={skip}",
            $"take={take}"
        };

        if (!string.IsNullOrWhiteSpace(inSpaces))
        {
            queryParams.Add($"in_spaces={inSpaces}");
        }

        if (!string.IsNullOrWhiteSpace(expand))
        {
            queryParams.Add($"expand={expand}");
        }

        var url = $"users?{string.Join("&", queryParams)}";

        try
        {
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync(AppJsonSerializerContext.Default.PagedResponseUser, cancellationToken);

            return result ?? new PagedResponse<User>();
        }
        catch (HttpRequestException ex)
        {
            throw new HttpRequestException($"Failed to fetch users: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error processing users response: {ex.Message}", ex);
        }
    }

    public async Task<List<User>> GetAllUsersAsync(
        string? inSpaces = null,
        string? expand = null,
        CancellationToken cancellationToken = default)
    {
        var allUsers = new List<User>();
        int skip = 0;
        const int take = 100;

        while (true)
        {
            var response = await GetUsersAsync(skip, take, inSpaces, expand, cancellationToken);
            allUsers.AddRange(response.Data);

            if (string.IsNullOrWhiteSpace(response.Meta?.Pagination?.NextPage))
            {
                break;
            }

            skip += take;
        }

        return allUsers;
    }

    public async Task<User> GetUserAsync(string userId, string? expand = null, CancellationToken cancellationToken = default)
    {
        var url = $"users/{userId}";
        if (!string.IsNullOrWhiteSpace(expand))
        {
            url += $"?expand={expand}";
        }

        try
        {
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync(
                AppJsonSerializerContext.Default.SingleItemResponseUser,
                cancellationToken);
            return result?.Data ?? throw new InvalidOperationException($"User with ID '{userId}' not found.");
        }
        catch (HttpRequestException ex)
        {
            throw new HttpRequestException($"Failed to fetch user {userId}: {ex.Message}", ex);
        }
    }

    public async Task<PagedResponse<Space>> GetSpacesAsync(
        int skip = 0,
        int take = 50,
        string? type = null,
        CancellationToken cancellationToken = default)
    {
        var queryParams = new List<string>
        {
            $"skip={skip}",
            $"take={take}"
        };

        if (!string.IsNullOrWhiteSpace(type))
        {
            queryParams.Add($"type={type}");
        }

        var url = $"spaces?{string.Join("&", queryParams)}";

        try
        {
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync(AppJsonSerializerContext.Default.PagedResponseSpace, cancellationToken);

            return result ?? new PagedResponse<Space>();
        }
        catch (HttpRequestException ex)
        {
            throw new HttpRequestException($"Failed to fetch spaces: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error processing spaces response: {ex.Message}", ex);
        }
    }

    public async Task<List<Space>> GetAllSpacesAsync(
        string? type = null,
        CancellationToken cancellationToken = default)
    {
        var allSpaces = new List<Space>();
        int skip = 0;
        const int take = 100;

        while (true)
        {
            var response = await GetSpacesAsync(skip, take, type, cancellationToken);
            allSpaces.AddRange(response.Data);

            if (string.IsNullOrWhiteSpace(response.Meta?.Pagination?.NextPage))
            {
                break;
            }

            skip += take;
        }

        return allSpaces;
    }

    public async Task<Space> GetSpaceAsync(string spaceId, CancellationToken cancellationToken = default)
    {
        var url = $"spaces/{spaceId}";

        try
        {
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync(
                AppJsonSerializerContext.Default.SingleItemResponseSpace,
                cancellationToken);
            return result?.Data ?? throw new InvalidOperationException($"Space with ID '{spaceId}' not found.");
        }
        catch (HttpRequestException ex)
        {
            throw new HttpRequestException($"Failed to fetch space {spaceId}: {ex.Message}", ex);
        }
    }

    public async Task<PagedResponse<User>> GetSpaceUsersAsync(
        string spaceId,
        int skip = 0,
        int take = 50,
        CancellationToken cancellationToken = default)
    {
        var queryParams = new List<string>
        {
            $"skip={skip}",
            $"take={take}"
        };

        var url = $"spaces/{spaceId}/users?{string.Join("&", queryParams)}";

        try
        {
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync(AppJsonSerializerContext.Default.PagedResponseUser, cancellationToken);

            return result ?? new PagedResponse<User>();
        }
        catch (HttpRequestException ex)
        {
            throw new HttpRequestException($"Failed to fetch users for space {spaceId}: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error processing space users response: {ex.Message}", ex);
        }
    }

    public async Task<List<User>> GetAllSpaceUsersAsync(
        string spaceId,
        CancellationToken cancellationToken = default)
    {
        var allUsers = new List<User>();
        int skip = 0;
        const int take = 100;

        while (true)
        {
            var response = await GetSpaceUsersAsync(spaceId, skip, take, cancellationToken);
            allUsers.AddRange(response.Data);

            if (string.IsNullOrWhiteSpace(response.Meta?.Pagination?.NextPage))
            {
                break;
            }

            skip += take;
        }

        return allUsers;
    }
}
