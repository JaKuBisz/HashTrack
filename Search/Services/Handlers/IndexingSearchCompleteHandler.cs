using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HashTrack.Attributes;
using HashTrack.Clustering.DTOs;
using HashTrack.Clustering.Services;
using HashTrack.DTOs;
using HashTrack.Helpers;
using HashTrack.Interfaces;
using HashTrack.Persistance.Interfaces;
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
            
            var groupedResults = 
                GroupAndIndexResults(searchResult);
            _storage.Set(Constants.Storage.IndexedHashTags, groupedResults);

            var clusteredResults = ClusterResults(groupedResults);
            var result = clusteredResults.Select(x =>
                    new IndexingResultsViewItem(x.Key, x.Value.NumOfOccurences, x.Value.SearchResults)).ToList();
            
            
            _hashTrackSearchWpfControl.SetIndexingResult(result);
        }
        
        private Dictionary<string, HashTagDto> GroupAndIndexResults(Search searchResult)
        {
            // Group and index the search results here
            var result = new Dictionary<string, HashTagDto>();

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
                        result.Add(hashTag, new HashTagDto(1, new HashSet<SearchResultViewItem> { searchResultViewItem }));
                    }
                }
            }

            return result;
        }

        private Dictionary<string, HashTagDto> ClusterResults(Dictionary<string, HashTagDto> hashtags)
        {
            Dictionary<string, ClusteringSettingDto> clusteringSettings;
            _storage.TryGet(Constants.Storage.HashtagClusteringSettings, out clusteringSettings);
            clusteringSettings = clusteringSettings ?? new Dictionary<string, ClusteringSettingDto>();

            foreach (var clusterSetting in clusteringSettings)
            {
                var mainKey = clusterSetting.Key;
                if(!hashtags.ContainsKey(mainKey)) continue;
                var mainTag = hashtags[mainKey];
                
                foreach (var key in clusterSetting.Value.MergedTags)
                {
                    if(!hashtags.ContainsKey(key)) continue;
                    mainTag = CombineHashTags(mainTag, hashtags[key]);
                    hashtags.Remove(key);
                }
            }

            //We order hastags so we dont have to check which one has more occurences and automatically know its the mam hashtag
            var orderedHashtags = hashtags.OrderByDescending(x => x.Value.NumOfOccurences);
            foreach (var mainHashtag in orderedHashtags)
            {
                foreach (var secondaryHashtag in orderedHashtags)
                {
                    if (mainHashtag.Equals(secondaryHashtag))
                    {
                        continue;
                    }

                    var primaryKey = mainHashtag.Key;
                    var secondaryKey = secondaryHashtag.Key;

                    var mainExceptionTags =
                        clusteringSettings.TryGetValue(primaryKey, out var setting) ? setting.ExceptionTags : new List<string>();

                    var secondaryExceptionTags =
                        clusteringSettings.TryGetValue(secondaryKey, out setting) ? setting.ExceptionTags : new List<string>();
                    
                    if (mainExceptionTags.Contains(secondaryKey) || secondaryExceptionTags.Contains(primaryKey))
                    {// Ensure mutal exlusion from exception tags;
                        continue;
                    }
                    
                    //Prevents tag with less usages to be absorbed by tag with more usages automatically
                    if (ClusteringClassifier.Classify(mainHashtag, secondaryHashtag))
                    {
                        hashtags[primaryKey] = CombineHashTags(mainHashtag.Value, secondaryHashtag.Value);
                        if (clusteringSettings.ContainsKey(primaryKey))
                        {
                            clusteringSettings[primaryKey].MergedTags.Add(secondaryKey);
                        }
                        else
                        {
                            clusteringSettings[primaryKey] = new ClusteringSettingDto()
                            {
                                MergedTags = new List<string>() { secondaryKey },
                                ExceptionTags = new List<string>()
                            };
                        }
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