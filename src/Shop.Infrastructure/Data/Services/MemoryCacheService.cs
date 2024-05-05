using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shop.Core.AppSettings;
using Shop.Core.Extensions;
using Shop.Core.SharedKernel;

namespace Shop.Infrastructure.Data.Services;

internal class MemoryCacheService(
    ILogger<MemoryCacheService> logger,
    IMemoryCache memoryCache,
    IOptions<CacheOptions> cacheOptions) : ICacheService
{
    private const string CacheServiceName = nameof(MemoryCacheService);
    private readonly MemoryCacheEntryOptions _cacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(cacheOptions.Value.AbsoluteExpirationInHours),
        SlidingExpiration = TimeSpan.FromSeconds(cacheOptions.Value.SlidingExpirationInSeconds)
    };

    public async Task<TItem> GetOrCreateAsync<TItem>(string cacheKey, Func<Task<TItem>> factory)
    {
        return await memoryCache.GetOrCreateAsync(cacheKey, async cacheEntry =>
        {
            var cacheValue = cacheEntry?.Value;
            if (cacheValue != null)
            {
                logger.LogInformation(
                    "----- Fetched from {CacheServiceName} '{CacheKey}'", CacheServiceName, cacheKey);

                return (TItem)cacheValue;
            }

            var item = await factory();
            if (!item.IsDefault()) // SonarQube Bug: item != nulll
            {
                logger.LogInformation("----- Added to {CacheServiceName}: '{CacheKey}'", CacheServiceName, cacheKey);
                memoryCache.Set(cacheKey, item, _cacheOptions);
            }

            return item;
        });
    }

    public async Task<IReadOnlyList<TItem>> GetOrCreateAsync<TItem>(
        string cacheKey,
        Func<Task<IReadOnlyList<TItem>>> factory)
    {
        return await memoryCache.GetOrCreateAsync(cacheKey, async cacheEntry =>
        {
            var cacheValues = cacheEntry?.Value;
            if (cacheValues != null)
            {
                logger.LogInformation(
                    "----- Fetched from {CacheServiceName}: '{CacheKey}'", CacheServiceName, cacheKey);

                return (IReadOnlyList<TItem>)cacheValues;
            }

            var items = await factory();
            if (items?.Any() == true)
            {
                logger.LogInformation("----- Added to {CacheServiceName}: '{CacheKey}'", CacheServiceName, cacheKey);
                memoryCache.Set(cacheKey, items, _cacheOptions);
            }

            return items;
        });
    }

    public Task RemoveAsync(params string[] cacheKeys)
    {
        foreach (var cacheKey in cacheKeys)
        {
            logger.LogInformation("----- Removed from {CacheServiceName}: '{CacheKey}'", CacheServiceName, cacheKey);
            memoryCache.Remove(cacheKey);
        }

        return Task.CompletedTask;
    }
}