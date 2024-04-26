using System.Collections.Generic;
using System.Linq;
using HashTrack.Core.Models;

namespace HashTrack.Core.Extensions
{
    public static class UniqueEntityExtensions
    {
        public static bool ContainsKey<T>(this IEnumerable<UniqueTag<T>> entities, T id)
        {
            return entities.Any(e => e.Id.Equals(id));
        }
        
        public static TEntity GetByKey<TEntity, T>(this IEnumerable<TEntity> entities, T id) where TEntity : UniqueTag<T>
        {
            return entities.FirstOrDefault(e => e.Id.Equals(id));
        }
    
        public static bool TryGetByKey<TEntity, T>(this IEnumerable<TEntity> entities, T id, out TEntity entity) where TEntity : UniqueTag<T>
        {
            entity = entities.FirstOrDefault(e => e.Id.Equals(id));
            return entity != null;
        }
    }
}