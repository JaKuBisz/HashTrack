using System;
using System.Collections.Generic;
using HashTrack.Core.Models;
using HashTrack.Core.Models.Search;

namespace HashTrack.Core.Extensions
{
    public static class HashTagExtensions
    {
        public static void AddOrReplace(this HashSet<HashTagModel> hashset, HashTagModel value, bool replace = false)
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