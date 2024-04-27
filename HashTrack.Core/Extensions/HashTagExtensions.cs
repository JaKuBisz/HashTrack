using System;
using System.Collections.Generic;
using HashTrack.Core.Models;

namespace HashTrack.Core.Extensions
{
    public static class HashTagExtrensions
    {
        public static void AddOrReplace<T>(this HashSet<UniqueId<T>> hashset, UniqueId<T> value, bool replace = false)
        {
            if (hashset == null)
            {
                throw new ArgumentNullException(nameof(hashset));
            }

            if (replace || !hashset.Contains(value))
            {
                hashset.Add(value);
            }
        }
    }
}