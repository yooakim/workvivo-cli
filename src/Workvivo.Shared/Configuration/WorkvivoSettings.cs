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
namespace Workvivo.Shared.Configuration;

/// <summary>
/// Configuration settings for the Workvivo API client.
/// </summary>
public class WorkvivoSettings
{
    /// <summary>
    /// Gets or sets the Workvivo API base URL.
    /// Default: https://api.workvivo.com/v1
    /// </summary>
    public string BaseUrl { get; set; } = "https://api.workvivo.com/v1";

    /// <summary>
    /// Gets or sets the API bearer token for authentication.
    /// Can be overridden by WORKVIVO_APITOKEN environment variable.
    /// </summary>
    public string? ApiToken { get; set; }

    /// <summary>
    /// Gets or sets the Workvivo Organization ID.
    /// Can be overridden by WORKVIVO_ORGANIZATIONID environment variable.
    /// </summary>
    public string? OrganizationId { get; set; }

    /// <summary>
    /// Gets or sets the cache TTL in minutes for API responses.
    /// Default is 5 minutes. Set to 0 to disable caching.
    /// </summary>
    public int CacheTtlMinutes { get; set; } = 5;

    /// <summary>
    /// Gets or sets the directory used for file-backed cache storage.
    /// Default: ~/.workvivo/cache (cross-platform).
    /// Can be overridden by WORKVIVO_CACHEDIRECTORY environment variable.
    /// </summary>
    public string CacheDirectory { get; set; } =
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".workvivo",
            "cache");

    /// <summary>
    /// Validates that all required settings are configured.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when required settings are missing.</exception>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(ApiToken))
        {
            throw new InvalidOperationException(
                "API Token is required. Set it in appsettings.json or via WORKVIVO_APITOKEN environment variable.");
        }

        if (string.IsNullOrWhiteSpace(OrganizationId))
        {
            throw new InvalidOperationException(
                "Organization ID is required. Set it in appsettings.json or via WORKVIVO_ORGANIZATIONID environment variable.");
        }

        if (string.IsNullOrWhiteSpace(BaseUrl))
        {
            throw new InvalidOperationException("Base URL cannot be empty.");
        }
    }
}
