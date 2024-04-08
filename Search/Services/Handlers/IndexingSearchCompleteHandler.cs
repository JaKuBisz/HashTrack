using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HashTrack.Attributes;
using HashTrack.DTOs;
using HashTrack.Helpers;
using HashTrack.Interfaces;
using Microsoft.Office.Interop.Outlook;

namespace HashTrack.Handlers
{
    [RegisterHandler(typeof(ISearchCompleteHandler), Constants.IndexingSearchTag)]
    public class IndexingSearchCompleteHandler : ISearchCompleteHandler
    {
        private readonly SidePanelWpfControl _hashTrackSearchWpfControl;

        public IndexingSearchCompleteHandler(SidePanelWpfControl hashTrackSearchWpfControl)
        {   
            _hashTrackSearchWpfControl = hashTrackSearchWpfControl;
        }
        public void HandleSearchComplete(Search searchResult)
        {
            if (searchResult.Tag != Constants.IndexingSearchTag)
            {
                return;
            }
            
            var hashtags = 
                GroupAndIndexResults(searchResult).Select(x =>
                    new IndexingResultsViewItem(x.Key, x.Value.NumOfOccurences, x.Value.SearchResults)).ToList();
            _hashTrackSearchWpfControl.SetIndexingResult(hashtags);
        }
        
        private Dictionary<string, IndexingResultDto> GroupAndIndexResults(Search searchResult)
        {
            // Group and index the search results here
            var result = new Dictionary<string, IndexingResultDto>();

            for (int i = 1; i <= searchResult.Results.Count; i++)
            {
                var item = searchResult.Results[i];
                var textContent = ArtefactItemHelper.GetBody(item);
                if (textContent == null)
                {
                    continue;
                }
                
                HashSet<string> hashTags = ExtractHashtags(textContent);
                foreach (var hashTag in hashTags)
                {
                    var searchResultViewItem = ArtefactItemHelper.MapSearchResultViewItem(item);
                    if (result.ContainsKey(hashTag))
                    {
                        var resultItem = result[hashTag];
                        resultItem.NumOfOccurences += 1;
                        resultItem.SearchResults.Add(searchResultViewItem);
                    }
                    else
                    {
                        result.Add(hashTag, new IndexingResultDto(1, new HashSet<SearchResultViewItem> { searchResultViewItem }));
                    }
                }
            }

            return result;
        }

        private HashSet<string> ExtractHashtags(string text)
        {
            var hashtags = new HashSet<string>();
            var matches = Regex.Matches(text, @"#\w+");
            foreach (Match match in matches)
            {
                string hashtag = match.Value.ToLower(); // Normalize to lowercase for consistent counting
                hashtags.Add(hashtag);
            }

            return hashtags;
        }
        
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
}