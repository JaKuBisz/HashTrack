using System.Collections.Generic;

namespace HashTrack.DTOs
{
    public class IndexingResultDto
    {
        public int NumOfOccurences { get; set; }
        public HashSet<SearchResultViewItem> SearchResults { get; set; }
            
        public IndexingResultDto(int numOfOccurences, HashSet<SearchResultViewItem> searchResults)
        {
            NumOfOccurences = numOfOccurences;
            SearchResults = searchResults;
        }
    }
}