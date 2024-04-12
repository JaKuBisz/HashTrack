using System.Collections.Generic;
using System.Linq;
using HashTrack.Core.Models;

namespace HashTrack.Core.Extensions
{
    public static class UniqueEntityExtensions
    {
        public static bool ContainsKey(this IEnumerable<UniqueEntity> entities, string id)
        {
            return entities.Any(e => e.Id == id);
        }
        
        public static TEntity GetByKey<TEntity>(this IEnumerable<TEntity> entities, string id) where TEntity : UniqueEntity
        {
            return entities.FirstOrDefault(e => e.Id == id);
        }
    
        public static bool TryGetByKey<TEntity>(this IEnumerable<TEntity> entities, string id, out TEntity entity) where TEntity : UniqueEntity
        {
            entity = entities.FirstOrDefault(e => e.Id == id);
            return entity != null;
        }
    }
}