using System.Collections.Generic;
using System.Linq;
using HashTrack.Core.Interfaces;
using HashTrack.Core.Models.Search;

namespace HashTrack.Core.Extensions
{
    public static class CacheExtensions
    {
        public static List<ArtefactModel> GetArtefacts(this ICache cache)
        {
            return cache.TryGet<List<ArtefactModel>>(Constants.Storage.Artefacts, out var artefacts)
                ? artefacts : new List<ArtefactModel>();
        }
        
        public static List<HashTagModel> GetHashTags(this ICache cache)
        {
           return cache.TryGet<List<HashTagModel>>(Constants.Storage.IndexedHashTags, out var hashTags)
                ? hashTags : new List<HashTagModel>();
        }
        
        public static void SetArtefacts(this ICache cache, List<ArtefactModel> artefacts)
        {
            cache.Set(Constants.Storage.Artefacts, artefacts);
        }
        
        public static void SetHashTags(this ICache cache, List<HashTagModel> hashTags)
        {
            cache.Set(Constants.Storage.IndexedHashTags, hashTags);
        }
        
        public static void AddArtefacts(this ICache cache, IEnumerable<ArtefactModel> artefacts)
        {
            var existingArtefacts = cache.GetArtefacts();
            existingArtefacts.AddRange(artefacts);
            cache.SetArtefacts(existingArtefacts.ToHashSet().ToList());
        }
        
        public static void AddHashTags(this ICache cache, IEnumerable<HashTagModel> hashTags)
        {//TODO: Add override option for saving From indexing - overding existing tags with updated one? no i need to merge them somehow here OMFG
            var existingHashTags = cache.GetHashTags();
            existingHashTags.AddRange(hashTags);
            cache.SetHashTags(existingHashTags.ToHashSet().ToList());
        }
    }
}