using System;
using HashTrack.Core.Attributes;
using HashTrack.Core.Enums;
using HashTrack.Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace HashTrack.Persistence.Services
{
    [RegisterService(LifeCycle.Singleton, typeof(ICache<>), true)]
    public class BaseCache<T> : ICache<T> where T : class
    {
        private readonly IMemoryCache _memoryCache;
        private readonly MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
        {
            SlidingExpiration = TimeSpan.FromHours(3)
        };

        public BaseCache(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }


        public T Get(string key)
        {
            _memoryCache.TryGetValue(key, out T item);
            return item;
        }

        public void Set(string key, T item)
        {
            _memoryCache.Set(key, item, cacheEntryOptions);
        }

        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }
    }
}