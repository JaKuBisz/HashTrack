using System.Collections.Generic;

namespace HashTrack.DTOs
{
    public class HashTagDto
    {
        public string Id { get; set; }
        public int NumOfOccurences { get; set; }
        public HashSet<SearchResultViewItem> SearchResults { get; set; }
            
        public HashTagDto(int numOfOccurences, HashSet<SearchResultViewItem> searchResults)
        {
            NumOfOccurences = numOfOccurences;
            SearchResults = searchResults; 
        }
    }
}