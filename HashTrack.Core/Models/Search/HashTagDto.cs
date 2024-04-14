using System;
using System.Collections.Generic;
using System.Linq;

namespace HashTrack.Core.Models.Search
{
    public class HashTagDto : UniqueEntity
    {
        public int NumOfOccurences { get; set; }
        public HashSet<SearchResultViewItem> SearchResults { get; set; }
        public DateTime LastUpdated { get; set; } 

        public int TotalNumOfOccurences()
        {
            return NumOfOccurences + MergedHashTags.Sum(x => x.TotalNumOfOccurences());
        }

        public HashSet<SearchResultViewItem> TotalSearchResults()
        {
            return SearchResults.Concat(MergedHashTags.SelectMany(x => x.TotalSearchResults())).ToHashSet();
        }

        public HashSet<HashTagDto> MergedHashTags { get; set; }

        public HashSet<HashTagDto> ExcludedHashTags { get; set; } //TODO: is null
  
        public HashTagDto() : base(id)
        {
            NumOfOccurences = numOfOccurences;
            SearchResults = searchResults; 
        }
  
        public HashTagDto(string id,HashSet<SearchResultViewItem> searchResults, int numOfOccurences = 1) : base(id)
        {
            NumOfOccurences = numOfOccurences;
            SearchResults = searchResults; 
        }
    }
}