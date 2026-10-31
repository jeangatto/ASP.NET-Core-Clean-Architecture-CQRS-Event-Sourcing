using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
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
public class DistributedCacheServiceTests
{
    [Fact]
    public async Task GetOrCreateAsync_ItemIsCached_SetsSerializedValue()
    {
        var distributedCache = Substitute.For<IDistributedCache>();
        distributedCache.GetAsync("item", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<byte[]>(null));
        var cache = CreateService(distributedCache);

        var result = await cache.GetOrCreateAsync("item", () => Task.FromResult("value"));

        result.Should().Be("value");
        await distributedCache.Received(1).SetAsync(
            "item",
            Arg.Is<byte[]>(value => Encoding.UTF8.GetString(value) == "\"value\""),
            Arg.Any<DistributedCacheEntryOptions>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetOrCreateAsync_ItemExists_ReturnsCachedValue()
    {
        var distributedCache = Substitute.For<IDistributedCache>();
        distributedCache.GetAsync("item", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Encoding.UTF8.GetBytes("\"cached\"")));
        var cache = CreateService(distributedCache);
        var factoryCalled = false;

        var result = await cache.GetOrCreateAsync("item", () =>
        {
            factoryCalled = true;
            return Task.FromResult("new");
        });

        result.Should().Be("cached");
        factoryCalled.Should().BeFalse();
        await distributedCache.DidNotReceive().SetAsync(
            Arg.Any<string>(),
            Arg.Any<byte[]>(),
            Arg.Any<DistributedCacheEntryOptions>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetOrCreateAsync_NonEmptyListIsCached()
    {
        var distributedCache = Substitute.For<IDistributedCache>();
        distributedCache.GetAsync("items", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<byte[]>(null));
        var cache = CreateService(distributedCache);
        IReadOnlyList<string> items = ["first", "second"];

        var result = await cache.GetOrCreateAsync("items", () => Task.FromResult(items));

        result.Should().Equal("first", "second");
        await distributedCache.Received(1).SetAsync(
            "items",
            Arg.Is<byte[]>(value => Encoding.UTF8.GetString(value) == "[\"first\",\"second\"]"),
            Arg.Any<DistributedCacheEntryOptions>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetOrCreateAsync_EmptyListIsNotCached()
    {
        var distributedCache = Substitute.For<IDistributedCache>();
        distributedCache.GetAsync("items", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<byte[]>(null));
        var cache = CreateService(distributedCache);

        await cache.GetOrCreateAsync("items", () => Task.FromResult<IReadOnlyList<string>>([]));

        await distributedCache.DidNotReceive().SetAsync(
            Arg.Any<string>(),
            Arg.Any<byte[]>(),
            Arg.Any<DistributedCacheEntryOptions>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RemoveAsync_KeysAndParamsOverloads_RemoveEachKey()
    {
        var distributedCache = Substitute.For<IDistributedCache>();
        var cache = CreateService(distributedCache);

        await cache.RemoveAsync("single");
        await cache.RemoveAsync("first", "second");

        await distributedCache.Received(1).RemoveAsync("single", Arg.Any<CancellationToken>());
        await distributedCache.Received(1).RemoveAsync("first", Arg.Any<CancellationToken>());
        await distributedCache.Received(1).RemoveAsync("second", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RemoveAsync_NullKeyCollection_DoesNothing()
    {
        var distributedCache = Substitute.For<IDistributedCache>();
        var cache = CreateService(distributedCache);

        await cache.RemoveAsync((IEnumerable<string>)null);

        await distributedCache.DidNotReceive().RemoveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    private static DistributedCacheService CreateService(IDistributedCache distributedCache)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["CacheOptions:AbsoluteExpirationInHours"] = "1",
                ["CacheOptions:SlidingExpirationInSeconds"] = "60"
            })
            .Build();

        return new DistributedCacheService(
            distributedCache,
            Substitute.For<ILogger<DistributedCacheService>>(),
            Options.Create(configuration.GetOptions<CacheOptions>()));
    }
}
