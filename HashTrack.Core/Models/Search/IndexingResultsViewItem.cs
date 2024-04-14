using System.Collections.Generic;
using HashTrack.Core.Models.Search;

namespace HashTrack.DTOs
{
    public class IndexingResultsViewItem
    {
        public string HashTag { get; set; }
        public int NumOfOccurences { get; set; }
        public HashSet<ArtefactModel> SearchResults { get; set; }
            
        public IndexingResultsViewItem(string hashTag, int numOfOccurences, HashSet<ArtefactModel> searchResults)
        {
            HashTag = hashTag;
            NumOfOccurences = numOfOccurences;
            SearchResults = searchResults;
        }
    }
}