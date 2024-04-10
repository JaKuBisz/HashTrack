using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HashTrack.Attributes;
using HashTrack.Clustering.DTOs;
using HashTrack.Clustering.Services;
using HashTrack.Core.Extension;
using HashTrack.DTOs;
using HashTrack.Helpers;
using HashTrack.Interfaces;
using HashTrack.Persistance.Interfaces;
using HashTrack.Search.Extensions;
using Microsoft.Office.Interop.Outlook;

namespace HashTrack.Handlers
{
    [RegisterHandler(typeof(ISearchCompleteHandler), Constants.IndexingSearchTag)]
    public class IndexingSearchCompleteHandler : ISearchCompleteHandler
    {
        private readonly SidePanelWpfControl _hashTrackSearchWpfControl;
        private readonly IStorage _storage;

        public IndexingSearchCompleteHandler(SidePanelWpfControl hashTrackSearchWpfControl, IStorage storage)
        {   
            _hashTrackSearchWpfControl = hashTrackSearchWpfControl;
            _storage = storage;
        }
        public void HandleSearchComplete(Search searchResult)
        {
            if (searchResult.Tag != Constants.IndexingSearchTag)
            {
                return;
            }
            //TODO: Clustering saving issue how to preserve tags before they are merged
            var groupedResults = 
                GroupAndIndexResults(searchResult);
            _storage.Set(Constants.Storage.IndexedHashTags, groupedResults);

            var clusteredResults = ClusterResults(groupedResults);
            var result = clusteredResults.Select(x =>
                    new IndexingResultsViewItem(x.Key, x.Value.NumOfOccurences, x.Value.SearchResults)).ToList();
            
            
            _hashTrackSearchWpfControl.SetIndexingResult(result);
        }
        
        private List<HashTagDto> GroupAndIndexResults(Search searchResult)
        {
            // Group and index the search results here
            //TODO: First load indexed hashtags from storage
            _storage.TryGet(Constants.Storage.IndexedHashTags, out List<HashTagDto> indexedHashTags);
            indexedHashTags = indexedHashTags ?? new List<HashTagDto>();

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
                    if (indexedHashTags.TryGetByKey(hashTag, out var resultItem))
                    {
                        resultItem.AddNewSearchResult(searchResultViewItem);
                    }
                    else
                    {
                        indexedHashTags.Add(new HashTagDto(hashTag, new HashSet<SearchResultViewItem> { searchResultViewItem }));
                    }
                }
            }

            return indexedHashTags;
        }

        private List<HashTagDto> ClusterResults(List<HashTagDto> hashtags)
        {
            //We order hastags so we dont have to check which one has more occurrences and automatically know its the main hashtag
            var orderedHashtags = hashtags.OrderByDescending(x => x.NumOfOccurences).ToList();
            //TODO: Improve O(n^2) complexity, see notes
            foreach (var primaryHashtag in orderedHashtags)
            {
                foreach (var secondaryHashtag in orderedHashtags)
                {
                    if (primaryHashtag.Equals(secondaryHashtag))
                    {
                        continue;
                    }
                    
                    if (primaryHashtag.ExcludedTagsContain(secondaryHashtag.Id) || secondaryHashtag.ExcludedTagsContain(primaryHashtag.Id))
                    {// Ensure mutual exclusion from exception tags;
                        continue;
                    }
                    
                    //Prevents tag with less usages to be absorbed by tag with more usages automatically
                    if (primaryHashtag.MergedTagsContain(secondaryHashtag.Id) // Merge if in merge list
                        || (!secondaryHashtag.MergedTagsContain(primaryHashtag.Id) // Prevent merge if in secondary merge list
                            && ClusteringClassifier.Classify(primaryHashtag, secondaryHashtag))) // Classify
                    {
                        primaryHashtag.MergeHashTag(secondaryHashtag);
                    }
                }
            }

            return hashtags;
        }

        private HashTagDto CombineHashTags(HashTagDto primary, HashTagDto secondary)
        {
            primary.NumOfOccurences += secondary.NumOfOccurences;
            foreach (var result in secondary.SearchResults)
            {
                primary.SearchResults.Add(result);
            }

            return primary;
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
    }
}