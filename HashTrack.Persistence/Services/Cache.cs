using System;
using HashTrack.Core.Attributes;
using HashTrack.Core.Enums;
using HashTrack.Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace HashTrack.Persistence.Services
{
    [RegisterService(LifeCycle.Singleton, typeof(ICache))]
    public class Cache : ICache
    {
        private readonly IMemoryCache _memoryCache;

        private readonly MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromHours(3)
        };

        public Cache(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public bool TryGet<T>(string key, out T item)
        {
            return _memoryCache.TryGetValue(key, out item);
        }

        public void Set<T>(string key, T item)
        {
            _memoryCache.Set(key, item, cacheEntryOptions);
        }

        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }
    }
}