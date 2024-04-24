using System.Collections.Generic;
using HashTrack.Core;
using HashTrack.Core.Interfaces;
using HashTrack.Core.Models.Search;

namespace HashTrack.Extensions
{
    public static class CacheExtensions
    {
        public static List<ArtefactModel> GetArtefacts(this ICache cache)
        {
            return cache.Get<List<ArtefactModel>>(Constants.Storage.Artefacts);
        }
        public static List<HashTagModel> GetHashTags(this ICache cache)
        {
           return cache.Get<List<HashTagModel>>(Constants.Storage.IndexedHashTags);
        }
    }
}