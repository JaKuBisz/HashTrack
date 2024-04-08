using System.Collections.Generic;

namespace HashTrack.DTOs
{
    public class IndexingResultsViewItem
    {
        public string HashTag { get; set; }
        public int NumOfOccurences { get; set; }
        public HashSet<SearchResultViewItem> SearchResults { get; set; }
            
        public IndexingResultsViewItem(string hashTag, int numOfOccurences, HashSet<SearchResultViewItem> searchResults)
        {
            HashTag = hashTag;
            NumOfOccurences = numOfOccurences;
            SearchResults = searchResults;
        }
    }
}