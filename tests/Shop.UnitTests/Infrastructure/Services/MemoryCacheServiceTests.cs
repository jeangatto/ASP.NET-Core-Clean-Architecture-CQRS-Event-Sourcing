using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Shop.Core.AppSettings;
using Shop.Core.Extensions;
using Shop.Infrastructure.Data.Services;
using Xunit;
using Xunit.Categories;

namespace Shop.UnitTests.Infrastructure.Services;

[UnitTest]
public class MemoryCacheServiceTests
{
    [Fact]
    public async Task GetOrCreateAsync_ItemIsCached_FactoryRunsOnce()
    {
        using var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var cache = CreateService(memoryCache);
        var factoryCalls = 0;

        Task<string> Factory()
        {
            factoryCalls++;
            return Task.FromResult("cached");
        }

        var first = await cache.GetOrCreateAsync("item", Factory);
        var second = await cache.GetOrCreateAsync("item", Factory);

        first.Should().Be("cached");
        second.Should().Be("cached");
        factoryCalls.Should().Be(1);
    }

    [Fact]
    public async Task GetOrCreateAsync_NullItem_ReturnsNull()
    {
        using var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var cache = CreateService(memoryCache);

        var result = await cache.GetOrCreateAsync("item", () => Task.FromResult<string>(null));

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetOrCreateAsync_NonEmptyListIsCached()
    {
        using var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var cache = CreateService(memoryCache);
        var factoryCalls = 0;
        IReadOnlyList<string> items = ["first", "second"];

        Task<IReadOnlyList<string>> Factory()
        {
            factoryCalls++;
            return Task.FromResult(items);
        }

        var first = await cache.GetOrCreateAsync("items", Factory);
        var second = await cache.GetOrCreateAsync("items", Factory);

        first.Should().Equal("first", "second");
        second.Should().Equal("first", "second");
        factoryCalls.Should().Be(1);
    }

    [Fact]
    public async Task GetOrCreateAsync_EmptyList_ReturnsEmptyList()
    {
        using var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var cache = CreateService(memoryCache);

        var result = await cache.GetOrCreateAsync("items", () => Task.FromResult<IReadOnlyList<string>>([]));

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task RemoveAsync_KeysAndParamsOverloads_RemoveCachedItems()
    {
        using var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var cache = CreateService(memoryCache);
        memoryCache.Set("single", "value");
        memoryCache.Set("first", "value");
        memoryCache.Set("second", "value");

        await cache.RemoveAsync("single");
        await cache.RemoveAsync(new[] { "first", "second" });

        memoryCache.TryGetValue("single", out _).Should().BeFalse();
        memoryCache.TryGetValue("first", out _).Should().BeFalse();
        memoryCache.TryGetValue("second", out _).Should().BeFalse();
    }

    [Fact]
    public async Task RemoveAsync_NullKeyCollection_DoesNothing()
    {
        using var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var cache = CreateService(memoryCache);

        var act = () => cache.RemoveAsync((IEnumerable<string>)null);

        await act.Should().NotThrowAsync();
    }

    private static MemoryCacheService CreateService(IMemoryCache memoryCache)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["CacheOptions:AbsoluteExpirationInHours"] = "1",
                ["CacheOptions:SlidingExpirationInSeconds"] = "60"
            })
            .Build();

        return new MemoryCacheService(
            Substitute.For<ILogger<MemoryCacheService>>(),
            memoryCache,
            Options.Create(configuration.GetOptions<CacheOptions>()));
    }
}
