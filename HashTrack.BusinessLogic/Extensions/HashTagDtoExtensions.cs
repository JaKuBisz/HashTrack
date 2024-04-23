using System.Linq;
using HashTrack.Core.Models.Search;

namespace HashTrack.BusinessLogic.Extensions
{
    public static class HashTagDtoExtensions
    {
        public static void AddNewSearchResult(this HashTagModel hashTag, ArtefactModel searchResult)
        {
            if (!hashTag.SearchResults.Add(searchResult)) return;
            hashTag.NumOfOccurrences++;
        }

        public static void MergeHashTag(this HashTagModel primary, HashTagModel secondary)
        {
            primary.MergedHashTags.Add(secondary);
      /*      
            primary.NumOfOccurrences += secondary.NumOfOccurrences;
            primary.MergedHashTags.Add(secondary.Id);
            foreach (var result in secondary.SearchResults)
            {
                primary.SearchResults.Add(result);
            }

            foreach (var result in secondary.MergedHashTags)
            {
                primary.MergedHashTags.Add(result);
            }

            foreach (var result in secondary.ExcludedHashTags)
            {
                primary.ExcludedHashTags.Add(result);
            }*/
        }

        public static void UnMergeHashTag(this HashTagModel primary, HashTagModel secondary)
        {
            primary.MergedHashTags.Remove(secondary);
            /*
            primary.NumOfOccurrences -= secondary.NumOfOccurrences;
            foreach (var result in secondary.SearchResults)
            {
                primary.SearchResults.Remove(result);
            }

            foreach (var result in secondary.MergedHashTags)
            {
                primary.MergedHashTags.Remove(result);
            }

            foreach (var result in secondary.ExcludedHashTags)
            {
                primary.ExcludedHashTags.Remove(result);
            }*/
        }

        public static bool MergedTagsContain(this HashTagModel hashTag, HashTagModel secondaryHashTag)
        {
            return hashTag.TotalMergedHashTags().Contains(secondaryHashTag);
        }

        public static bool ExcludedTagsContain(this HashTagModel hashTag, HashTagModel secondaryHashTag)
        {
            return hashTag.TotalExcludedHashTags().Contains(secondaryHashTag);
        }
    }

}