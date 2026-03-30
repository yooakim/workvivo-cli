using FluentAssertions;
using NSubstitute;
using Workvivo.Shared.Configuration;
using Workvivo.Shared.Models;
using Workvivo.Shared.Services;

namespace WorkvivoCli.Tests.Services;

public class FileBackedCachingWorkvivoApiClientTests : IDisposable
{
    private readonly string _cacheDir;
    private readonly IWorkvivoApiClient _inner;

    public FileBackedCachingWorkvivoApiClientTests()
    {
        _cacheDir = Path.Combine(Path.GetTempPath(), "wv-cache-tests-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_cacheDir);
        _inner = Substitute.For<IWorkvivoApiClient>();
    }

    public void Dispose()
    {
        if (Directory.Exists(_cacheDir))
            Directory.Delete(_cacheDir, recursive: true);
    }

    private FileBackedCachingWorkvivoApiClient CreateSut(int ttlMinutes = 5) =>
        new(
            _inner,
            new WorkvivoSettings
            {
                ApiToken = "test-token",
                OrganizationId = "test-org",
                CacheTtlMinutes = ttlMinutes,
                CacheDirectory = _cacheDir
            });

    // ── TTL = 0 bypasses cache ────────────────────────────────────────────────

    [Fact]
    public async Task GetAllUsersAsync_WhenTtlIsZero_BypassesCacheAndCallsInnerEveryTime()
    {
        _inner.GetAllUsersAsync(default, default, default, default)
              .ReturnsForAnyArgs(new List<User>());
        var sut = CreateSut(ttlMinutes: 0);

        await sut.GetAllUsersAsync();
        await sut.GetAllUsersAsync();

        await _inner.ReceivedWithAnyArgs(2).GetAllUsersAsync();
        Directory.GetFiles(_cacheDir).Should().BeEmpty();
    }

    // ── Cache miss ────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllUsersAsync_OnCacheMiss_CallsInnerAndWritesCacheFile()
    {
        var expected = new List<User> { new() { Id = 42, Email = "x@y.com" } };
        _inner.GetAllUsersAsync(default, default, default, default)
              .ReturnsForAnyArgs(expected);
        var sut = CreateSut();

        var result = await sut.GetAllUsersAsync();

        result.Should().BeEquivalentTo(expected);
        await _inner.ReceivedWithAnyArgs(1).GetAllUsersAsync();
        Directory.GetFiles(_cacheDir, "*.json").Should().HaveCount(1);
    }

    // ── Cache hit ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllUsersAsync_OnCacheHit_ReturnsCachedValueWithoutCallingInner()
    {
        var expected = new List<User> { new() { Id = 7, Email = "cached@example.com" } };
        _inner.GetAllUsersAsync(default, default, default, default)
              .ReturnsForAnyArgs(expected);
        var sut = CreateSut();

        await sut.GetAllUsersAsync();
        _inner.ClearReceivedCalls();

        var result = await sut.GetAllUsersAsync();

        result.Should().BeEquivalentTo(expected);
        await _inner.DidNotReceiveWithAnyArgs().GetAllUsersAsync();
    }

    // ── TTL expiry ────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllUsersAsync_AfterTtlExpiry_CallsInnerAgain()
    {
        _inner.GetAllUsersAsync(default, default, default, default)
              .ReturnsForAnyArgs(new List<User>());
        var sut = CreateSut(ttlMinutes: 1);

        await sut.GetAllUsersAsync();

        var cacheFile = Directory.GetFiles(_cacheDir, "*.json").Single();
        File.SetLastWriteTimeUtc(cacheFile, DateTime.UtcNow.AddMinutes(-2));

        _inner.ClearReceivedCalls();

        await sut.GetAllUsersAsync();

        await _inner.ReceivedWithAnyArgs(1).GetAllUsersAsync();
    }

    // ── Corrupt cache file ────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllUsersAsync_WithCorruptCacheFile_DeletesFileAndCallsInner()
    {
        var expected = new List<User> { new() { Id = 5, Email = "fallback@example.com" } };
        _inner.GetAllUsersAsync(default, default, default, default)
              .ReturnsForAnyArgs(expected);
        var sut = CreateSut();

        await sut.GetAllUsersAsync();
        var cacheFile = Directory.GetFiles(_cacheDir, "*.json").Single();
        File.WriteAllText(cacheFile, "NOT VALID JSON {{{{");

        _inner.ClearReceivedCalls();

        var result = await sut.GetAllUsersAsync();

        result.Should().BeEquivalentTo(expected);
        await _inner.ReceivedWithAnyArgs(1).GetAllUsersAsync();
    }

    // ── Parameter key separation ──────────────────────────────────────────────

    [Fact]
    public async Task GetUsersAsync_DifferentParameters_ProduceSeparateCacheFiles()
    {
        _inner.GetUsersAsync(default, default, default, default, default, default)
              .ReturnsForAnyArgs(new PagedResponse<User>());
        var sut = CreateSut();

        await sut.GetUsersAsync(skip: 0, take: 50);
        await sut.GetUsersAsync(skip: 50, take: 50);

        Directory.GetFiles(_cacheDir, "*.json").Should().HaveCount(2);
    }
}
