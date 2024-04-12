using System.Collections.Concurrent;
using HashTrack.Core.Attributes;
using HashTrack.Core.Enums;
using HashTrack.Core.Interfaces.Persistence;

namespace HashTrack.Persistence.Services
{
    [RegisterService(LifeCycle.Singleton, typeof(IStorage))]
    public class CachedPersistenceStorage : IStorage
    {
        private ConcurrentDictionary<string, object> _cache = new ConcurrentDictionary<string, object>();
        private IStorage _persistentStorage;

        public CachedPersistenceStorage(IStorage persistentStorage)
        {
            _persistentStorage = persistentStorage;
        }
        // Generate a unique key for each object based on its type and an identifier
        private string GetKey<T>(string id) => $"{typeof(T).FullName}:{id}";

        // Add or update an object in the cache
        public void Set<T>(string id, T value)
        {
            var key = GetKey<T>(id);
            _cache[key] = value;
            
            _persistentStorage.Set(id, value);
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
            
            if (_persistentStorage.TryGet(id, out value))
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
            _persistentStorage.Remove<T>(id);
        }
    }
}