using System.Collections.Concurrent;
using HashTrack.Enums;
using HashTrack.Helpers;
using HashTrack.Persistance.Interfaces;

namespace HashTrack.Services
{
    [RegisterService(typeof(IStorage), LifeCycle.Singleton)]
    public class CachedPersistanceStorage : IStorage
    {
        private ConcurrentDictionary<string, object> _cache = new ConcurrentDictionary<string, object>();
        private IStorage _persistantStorage;

        public CachedPersistanceStorage(PersistantStorage persistantStorage)
        {
            _persistantStorage = persistantStorage;
        }
        // Generate a unique key for each object based on its type and an identifier
        private string GetKey<T>(string id) => $"{typeof(T).FullName}:{id}";

        // Add or update an object in the cache
        public void Set<T>(string id, T value)
        {
            var key = GetKey<T>(id);
            _cache[key] = value;
            
            _persistantStorage.Set(id, value);
        }

        // Try to get an object from the cache
        public bool TryGet<T>(string id, out T value)
        {
            var key = GetKey<T>(id);
            if (_cache.TryGetValue(key, out object objectValue) && objectValue is T typedValue)
            {
                value = typedValue;
                return true;
            }
            
            if (_persistantStorage.TryGet(id, out value))
            {
                Set(id, value);
                return true;
            }

            value = default;
            return false;
        }

        // Remove an object from the cache
        public void Remove<T>(string id)
        {
            var key = GetKey<T>(id);
            _cache.TryRemove(key, out _);
            _persistantStorage.Remove<T>(id);
        }
    }
}