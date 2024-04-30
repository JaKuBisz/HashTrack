using System;
using System.Collections.Generic;
using HashTrack.Core.Models.Search;

namespace HashTrack.Core.Extensions
{
    public static class HashTagExtensions
    {
        public static void AddNewSearchResult(this HashTagModel hashTag, ArtefactModel searchResult)
        {
            if (!hashTag.SearchResults.Add(searchResult) || !hashTag.ArtifactsIds.Add(searchResult.Id)) return;
            hashTag.NumOfOccurrences++;
        }

        public static void MergeHashTag(this HashTagModel primary, HashTagModel secondary)
        {
            primary.ExcludedHashTags.Remove(secondary);
            primary.MergedHashTags.Add(secondary);
        }

        public static void UnMergeHashTag(this HashTagModel primary, HashTagModel secondary)
        {
            primary.MergedHashTags.Remove(secondary);
            primary.ExcludedHashTags.Add(secondary);
        }

        public static void RemoveExcluded(this HashTagModel primary, HashTagModel secondary)
        {
            primary.ExcludedHashTags.Remove(secondary);
        }

        public static bool MergedTagsContain(this HashTagModel hashTag, HashTagModel secondaryHashTag)
        {
            return hashTag.TotalMergedHashTags().Contains(secondaryHashTag);
        }

        public static bool ExcludedTagsContain(this HashTagModel hashTag, HashTagModel secondaryHashTag)
        {
            return hashTag.TotalExcludedHashTags().Contains(secondaryHashTag);
        }


        public static void AddOrReplace(this HashSet<HashTagModel> hashset, HashTagModel value, bool replace = false)
        {
            if (hashset == null) throw new ArgumentNullException(nameof(hashset));

            if (replace || !hashset.Contains(value)) hashset.Add(value);
        }
    }
}