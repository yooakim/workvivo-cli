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
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Workvivo.Shared.Configuration;
using Workvivo.Shared.Models;
using Workvivo.Shared.Serialization;

namespace Workvivo.Shared.Services;

/// <summary>
/// A file-backed caching decorator for <see cref="IWorkvivoApiClient"/> that persists
/// API responses as JSON files so that cache entries survive across process restarts.
/// Set <see cref="WorkvivoSettings.CacheTtlMinutes"/> to 0 to bypass caching entirely.
/// </summary>
public class FileBackedCachingWorkvivoApiClient : IWorkvivoApiClient
{
    private readonly IWorkvivoApiClient _inner;
    private readonly string _cacheDir;
    private readonly TimeSpan _ttl;

    public FileBackedCachingWorkvivoApiClient(
        IWorkvivoApiClient inner,
        WorkvivoSettings settings)
    {
        _inner = inner;
        _ttl = TimeSpan.FromMinutes(settings.CacheTtlMinutes);
        _cacheDir = settings.CacheDirectory;

        if (_ttl != TimeSpan.Zero)
            Directory.CreateDirectory(_cacheDir);
    }

    // ── Core helper ──────────────────────────────────────────────────────────

    private async Task<T> GetOrSetAsync<T>(
        string cacheKey,
        JsonTypeInfo<T> typeInfo,
        Func<CancellationToken, Task<T>> factory,
        CancellationToken ct)
    {
        var filePath = CacheFilePath(cacheKey);

        if (File.Exists(filePath) && DateTime.UtcNow - File.GetLastWriteTimeUtc(filePath) < _ttl)
        {
            try
            {
                await using var readStream = File.OpenRead(filePath);
                var cached = await JsonSerializer.DeserializeAsync(readStream, typeInfo, ct);
                if (cached is not null)
                    return cached;
            }
            catch (Exception ex) when (ex is JsonException or IOException)
            {
                TryDeleteFile(filePath);
            }
        }

        var result = await factory(ct);

        var tmpPath = filePath + ".tmp." + Path.GetRandomFileName();
        try
        {
            await using (var writeStream = File.Create(tmpPath))
                await JsonSerializer.SerializeAsync(writeStream, result, typeInfo, ct);
            File.Move(tmpPath, filePath, overwrite: true);
        }
        catch
        {
            TryDeleteFile(tmpPath);
        }

        return result;
    }

    private string CacheFilePath(string key)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(key));
        return Path.Combine(_cacheDir, Convert.ToHexString(hash).ToLowerInvariant() + ".json");
    }

    private static void TryDeleteFile(string path)
    {
        try { File.Delete(path); } catch { }
    }

    // ── Users ────────────────────────────────────────────────────────────────

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
        return GetOrSetAsync(key, AppJsonSerializerContext.Default.PagedResponseUser,
            ct => _inner.GetUsersAsync(skip, take, inSpaces, inTeams, expand, ct), cancellationToken);
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
        return GetOrSetAsync(key, AppJsonSerializerContext.Default.ListUser,
            ct => _inner.GetAllUsersAsync(inSpaces, inTeams, expand, ct), cancellationToken);
    }

    public Task<User> GetUserAsync(
        string userId,
        string? expand = null,
        CancellationToken cancellationToken = default)
    {
        if (_ttl == TimeSpan.Zero)
            return _inner.GetUserAsync(userId, expand, cancellationToken);

        var key = $"user|{userId}|expand={expand ?? ""}";
        return GetOrSetAsync(key, AppJsonSerializerContext.Default.User,
            ct => _inner.GetUserAsync(userId, expand, ct), cancellationToken);
    }

    // ── Spaces ───────────────────────────────────────────────────────────────

    public Task<PagedResponse<Space>> GetSpacesAsync(
        int skip = 0,
        int take = 50,
        string? type = null,
        CancellationToken cancellationToken = default)
    {
        if (_ttl == TimeSpan.Zero)
            return _inner.GetSpacesAsync(skip, take, type, cancellationToken);

        var key = $"spaces|skip={skip}|take={take}|type={type ?? ""}";
        return GetOrSetAsync(key, AppJsonSerializerContext.Default.PagedResponseSpace,
            ct => _inner.GetSpacesAsync(skip, take, type, ct), cancellationToken);
    }

    public Task<List<Space>> GetAllSpacesAsync(
        string? type = null,
        CancellationToken cancellationToken = default)
    {
        if (_ttl == TimeSpan.Zero)
            return _inner.GetAllSpacesAsync(type, cancellationToken);

        var key = $"spaces_all|type={type ?? ""}";
        return GetOrSetAsync(key, AppJsonSerializerContext.Default.ListSpace,
            ct => _inner.GetAllSpacesAsync(type, ct), cancellationToken);
    }

    public Task<Space> GetSpaceAsync(
        string spaceId,
        CancellationToken cancellationToken = default)
    {
        if (_ttl == TimeSpan.Zero)
            return _inner.GetSpaceAsync(spaceId, cancellationToken);

        var key = $"space|{spaceId}";
        return GetOrSetAsync(key, AppJsonSerializerContext.Default.Space,
            ct => _inner.GetSpaceAsync(spaceId, ct), cancellationToken);
    }

    // ── Space membership ─────────────────────────────────────────────────────

    public Task<PagedResponse<User>> GetSpaceUsersAsync(
        string spaceId,
        int skip = 0,
        int take = 50,
        CancellationToken cancellationToken = default)
    {
        if (_ttl == TimeSpan.Zero)
            return _inner.GetSpaceUsersAsync(spaceId, skip, take, cancellationToken);

        var key = $"space_users|{spaceId}|skip={skip}|take={take}";
        return GetOrSetAsync(key, AppJsonSerializerContext.Default.PagedResponseUser,
            ct => _inner.GetSpaceUsersAsync(spaceId, skip, take, ct), cancellationToken);
    }

    public Task<List<User>> GetAllSpaceUsersAsync(
        string spaceId,
        CancellationToken cancellationToken = default)
    {
        if (_ttl == TimeSpan.Zero)
            return _inner.GetAllSpaceUsersAsync(spaceId, cancellationToken);

        var key = $"space_users_all|{spaceId}";
        return GetOrSetAsync(key, AppJsonSerializerContext.Default.ListUser,
            ct => _inner.GetAllSpaceUsersAsync(spaceId, ct), cancellationToken);
    }
}
