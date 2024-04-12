using HashTrack.Core.Models.Search;

namespace HashTrack.BusinessLogic.Extensions
{
    public static class HashTagDtoExtensions
    {
        public static void AddNewSearchResult(this HashTagDto hashTag, SearchResultViewItem searchResult)
        {
            if (!hashTag.SearchResults.Add(searchResult)) return;
            hashTag.NumOfOccurences++;
        }

        public static void MergeHashTag(this HashTagDto primary, HashTagDto secondary)
        {
            primary.NumOfOccurences += secondary.NumOfOccurences;
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
            }
        }

        public static void UnMergeHashTag(this HashTagDto primary, HashTagDto secondary)
        {
            primary.NumOfOccurences -= secondary.NumOfOccurences;
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
            }
        }

        public static bool MergedTagsContain(this HashTagDto hashTag, string tag)
        {
            return hashTag.MergedHashTags.Contains(tag);
        }

        public static bool ExcludedTagsContain(this HashTagDto hashTag, string tag)
        {
            return hashTag.ExcludedHashTags.Contains(tag);
        }
    }

}