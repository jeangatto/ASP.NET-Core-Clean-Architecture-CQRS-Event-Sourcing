using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shop.Core.Interfaces;

public interface ICacheService
{
    Task<TItem> GetOrCreateAsync<TItem>(string cacheKey, Func<Task<TItem>> factory);
    Task<IEnumerable<TItem>> GetOrCreateAsync<TItem>(string cacheKey, Func<Task<IEnumerable<TItem>>> factory);
}