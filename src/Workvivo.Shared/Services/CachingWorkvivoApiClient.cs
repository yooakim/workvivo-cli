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
using Microsoft.Extensions.Caching.Memory;
using Workvivo.Shared.Configuration;
using Workvivo.Shared.Models;

namespace Workvivo.Shared.Services;

/// <summary>
/// A caching decorator for <see cref="IWorkvivoApiClient"/> that stores API responses
/// in memory for a configurable TTL. Set <see cref="WorkvivoSettings.CacheTtlMinutes"/>
/// to 0 to bypass caching entirely.
/// </summary>
public class CachingWorkvivoApiClient : IWorkvivoApiClient
{
    private readonly IWorkvivoApiClient _inner;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _ttl;

    public CachingWorkvivoApiClient(
        IWorkvivoApiClient inner,
        IMemoryCache cache,
        WorkvivoSettings settings)
    {
        _inner = inner;
        _cache = cache;
        _ttl = TimeSpan.FromMinutes(settings.CacheTtlMinutes);
    }

    // -------------------------------------------------------------------------
    // Users
    // -------------------------------------------------------------------------

    public Task<PagedResponse<User>> GetUsersAsync(
        int skip = 0,
        int take = 50,
        string? inSpaces = null,
        string? inTeams = null,
        string? expand = null,
        CancellationToken cancellationToken = default)
    {
        if (_ttl == TimeSpan.Zero)
            return _inner.GetUsersAsync(skip, take, inSpaces, inTeams, expand, cancellationToken);

        var key = $"users|skip={skip}|take={take}|inSpaces={inSpaces ?? ""}|inTeams={inTeams ?? ""}|expand={expand ?? ""}";
        return _cache.GetOrCreateAsync(key, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = _ttl;
            return _inner.GetUsersAsync(skip, take, inSpaces, inTeams, expand, cancellationToken);
        })!;
    }

    public Task<List<User>> GetAllUsersAsync(
        string? inSpaces = null,
        string? inTeams = null,
        string? expand = null,
        CancellationToken cancellationToken = default)
    {
        if (_ttl == TimeSpan.Zero)
            return _inner.GetAllUsersAsync(inSpaces, inTeams, expand, cancellationToken);

        var key = $"users_all|inSpaces={inSpaces ?? ""}|inTeams={inTeams ?? ""}|expand={expand ?? ""}";
        return _cache.GetOrCreateAsync(key, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = _ttl;
            return _inner.GetAllUsersAsync(inSpaces, inTeams, expand, cancellationToken);
        })!;
    }

    public Task<User> GetUserAsync(
        string userId,
        string? expand = null,
        CancellationToken cancellationToken = default)
    {
        if (_ttl == TimeSpan.Zero)
            return _inner.GetUserAsync(userId, expand, cancellationToken);

        var key = $"user|{userId}|expand={expand ?? ""}";
        return _cache.GetOrCreateAsync(key, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = _ttl;
            return _inner.GetUserAsync(userId, expand, cancellationToken);
        })!;
    }

    // -------------------------------------------------------------------------
    // Spaces
    // -------------------------------------------------------------------------

    public Task<PagedResponse<Space>> GetSpacesAsync(
        int skip = 0,
        int take = 50,
        string? type = null,
        CancellationToken cancellationToken = default)
    {
        if (_ttl == TimeSpan.Zero)
            return _inner.GetSpacesAsync(skip, take, type, cancellationToken);

        var key = $"spaces|skip={skip}|take={take}|type={type ?? ""}";
        return _cache.GetOrCreateAsync(key, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = _ttl;
            return _inner.GetSpacesAsync(skip, take, type, cancellationToken);
        })!;
    }

    public Task<List<Space>> GetAllSpacesAsync(
        string? type = null,
        CancellationToken cancellationToken = default)
    {
        if (_ttl == TimeSpan.Zero)
            return _inner.GetAllSpacesAsync(type, cancellationToken);

        var key = $"spaces_all|type={type ?? ""}";
        return _cache.GetOrCreateAsync(key, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = _ttl;
            return _inner.GetAllSpacesAsync(type, cancellationToken);
        })!;
    }

    public Task<Space> GetSpaceAsync(
        string spaceId,
        CancellationToken cancellationToken = default)
    {
        if (_ttl == TimeSpan.Zero)
            return _inner.GetSpaceAsync(spaceId, cancellationToken);

        var key = $"space|{spaceId}";
        return _cache.GetOrCreateAsync(key, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = _ttl;
            return _inner.GetSpaceAsync(spaceId, cancellationToken);
        })!;
    }

    // -------------------------------------------------------------------------
    // Space membership
    // -------------------------------------------------------------------------

    public Task<PagedResponse<User>> GetSpaceUsersAsync(
        string spaceId,
        int skip = 0,
        int take = 50,
        CancellationToken cancellationToken = default)
    {
        if (_ttl == TimeSpan.Zero)
            return _inner.GetSpaceUsersAsync(spaceId, skip, take, cancellationToken);

        var key = $"space_users|{spaceId}|skip={skip}|take={take}";
        return _cache.GetOrCreateAsync(key, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = _ttl;
            return _inner.GetSpaceUsersAsync(spaceId, skip, take, cancellationToken);
        })!;
    }

    public Task<List<User>> GetAllSpaceUsersAsync(
        string spaceId,
        CancellationToken cancellationToken = default)
    {
        if (_ttl == TimeSpan.Zero)
            return _inner.GetAllSpaceUsersAsync(spaceId, cancellationToken);

        var key = $"space_users_all|{spaceId}";
        return _cache.GetOrCreateAsync(key, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = _ttl;
            return _inner.GetAllSpaceUsersAsync(spaceId, cancellationToken);
        })!;
    }
}
