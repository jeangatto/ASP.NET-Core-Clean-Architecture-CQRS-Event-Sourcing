using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shop.Core.Abstractions;

public interface ICacheService
{
    Task<TItem> GetOrCreateAsync<TItem>(string cacheKey, Func<Task<TItem>> factory);
    Task<IReadOnlyList<TItem>> GetOrCreateAsync<TItem>(string cacheKey, Func<Task<IReadOnlyList<TItem>>> factory);
    Task RemoveAsync(params string[] cacheKeys);
}