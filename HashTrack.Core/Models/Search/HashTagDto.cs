using System.Collections.Generic;
using HashTrack.DTOs;

namespace HashTrack.Core.Models.Search
{
    public class HashTagDto : UniqueEntity
    {
        public int NumOfOccurences { get; set; }
        public HashSet<SearchResultViewItem> SearchResults { get; set; }
        
        public HashSet<string> MergedHashTags { get; set; }
        
        public HashSet<string> ExcludedHashTags { get; set; }
            
        public HashTagDto(string id,HashSet<SearchResultViewItem> searchResults, int numOfOccurences = 1) : base(id)
        {
            NumOfOccurences = numOfOccurences;
            SearchResults = searchResults; 
        }
    }
}