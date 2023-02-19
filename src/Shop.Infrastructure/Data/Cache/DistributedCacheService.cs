using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shop.Core.AppSettings;
using Shop.Core.Extensions;
using Shop.Core.Interfaces;

namespace Shop.Infrastructure.Data.Cache;

public class DistributedCacheService : ICacheService
{
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<DistributedCacheService> _logger;
    private readonly DistributedCacheEntryOptions _cacheOptions;

    public DistributedCacheService(
        IDistributedCache distributedCache,
        ILogger<DistributedCacheService> logger,
        IOptions<CacheOptions> cacheOptions)
    {
        _distributedCache = distributedCache;
        _logger = logger;
        _cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(cacheOptions.Value.AbsoluteExpirationInHours),
            SlidingExpiration = TimeSpan.FromSeconds(cacheOptions.Value.SlidingExpirationInSeconds)
        };
    }

    public async Task<TItem> GetOrCreateAsync<TItem>(string cacheKey, Func<Task<TItem>> factory)
    {
        var result = await _distributedCache.GetAsync(cacheKey);
        if (result?.Length > 0)
        {
            _logger.LogInformation("----- Fetched from DistributedCache: '{CacheKey}'", cacheKey);

            var value = Encoding.UTF8.GetString(result);
            return value.FromJson<TItem>();
        }
        else
        {
            var item = await factory();
            if (item != null)
            {
                _logger.LogInformation("----- Added to DistributedCache: '{CacheKey}'", cacheKey);

                var value = item.ToJson();
                var cacheValue = Encoding.UTF8.GetBytes(value);
                await _distributedCache.SetAsync(cacheKey, cacheValue, _cacheOptions);
            }

            return item;
        }
    }
}