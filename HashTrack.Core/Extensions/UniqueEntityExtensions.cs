using System.Collections.Generic;
using System.Linq;
using HashTrack.Core.Models;

namespace HashTrack.Core.Extensions
{
    public static class UniqueEntityExtensions
    {
        public static bool ContainsKey(this IEnumerable<UniqueTag> entities, string id)
        {
            return entities.Any(e => e.Tag == id);
        }
        
        public static TEntity GetByKey<TEntity>(this IEnumerable<TEntity> entities, string id) where TEntity : UniqueTag
        {
            return entities.FirstOrDefault(e => e.Tag == id);
        }
    
        public static bool TryGetByKey<TEntity>(this IEnumerable<TEntity> entities, string id, out TEntity entity) where TEntity : UniqueTag
        {
            entity = entities.FirstOrDefault(e => e.Tag == id);
            return entity != null;
        }
    }
}