using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shop.Core.AppSettings;
using Shop.Core.Interfaces;

namespace Shop.Infrastructure.Data.Cache;

public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<MemoryCacheService> _logger;
    private readonly MemoryCacheEntryOptions _cacheOptions;

    public MemoryCacheService(
        ILogger<MemoryCacheService> logger,
        IMemoryCache memoryCache,
        IOptions<CacheOptions> cacheOptions)
    {
        _logger = logger;
        _memoryCache = memoryCache;
        _cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(cacheOptions.Value.AbsoluteExpirationInHours),
            SlidingExpiration = TimeSpan.FromSeconds(cacheOptions.Value.SlidingExpirationInSeconds)
        };
    }

    public async Task<TItem> GetOrCreateAsync<TItem>(string cacheKey, Func<Task<TItem>> factory)
    {
        return await _memoryCache.GetOrCreateAsync(cacheKey, async (cacheEntry) =>
        {
            var cacheValue = cacheEntry?.Value;
            if (cacheValue != null)
            {
                _logger.LogInformation("----- Fetched from MemoryCache: '{CacheKey}'", cacheKey);
                return (TItem)cacheValue;
            }

            var item = await factory();
            if (item != null)
            {
                _logger.LogInformation("----- Added to MemoryCache: '{CacheKey}'", cacheKey);
                _memoryCache.Set(cacheKey, item, _cacheOptions);
            }

            return item;
        });
    }

    public async Task<IEnumerable<TItem>> GetOrCreateAsync<TItem>(string cacheKey, Func<Task<IEnumerable<TItem>>> factory)
    {
        return await _memoryCache.GetOrCreateAsync(cacheKey, async (cacheEntry) =>
        {
            var cacheValues = cacheEntry?.Value;
            if (cacheValues != null)
            {
                _logger.LogInformation("----- Fetched from MemoryCache: '{CacheKey}'", cacheKey);
                return (IEnumerable<TItem>)cacheValues;
            }

            var items = await factory();
            if (items != null)
            {
                _logger.LogInformation("----- Added to MemoryCache: '{CacheKey}'", cacheKey);
                _memoryCache.Set(cacheKey, items, _cacheOptions);
            }

            return items;
        });
    }
}