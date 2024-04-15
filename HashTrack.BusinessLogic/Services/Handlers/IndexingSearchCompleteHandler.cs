using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HashTrack.BusinessLogic.Extensions;
using HashTrack.Clustering.Services;
using HashTrack.Core;
using HashTrack.Core.Attributes;
using HashTrack.Core.Extensions;
using HashTrack.Core.Interfaces;
using HashTrack.Core.Interfaces.Handlers;
using HashTrack.Core.Interfaces.Persistence;
using HashTrack.Core.Models.Search;
using HashTrack.DTOs;
using HashTrack.Helpers;
using HashTrack.Interfaces;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace HashTrack.BusinessLogic.Services.Handlers
{
    [RegisterHandler(Events.IndexingSearchCompleted, typeof(ISearchCompleteHandler))]
    public class IndexingSearchCompleteHandler : ISearchCompleteHandler
    {//TODO: Use events instead of direct call to UI
        //private readonly SidePanelWpfControl _hashTrackSearchWpfControl;
        private readonly IPersistenceHashTagService _storage;
        private readonly IEventPublisher _eventPublisher;

        public IndexingSearchCompleteHandler(IPersistenceHashTagService storage, IEventPublisher eventPublisher)//SidePanelWpfControl hashTrackSearchWpfControl, IStorage storage)
        {   
            //_hashTrackSearchWpfControl = hashTrackSearchWpfControl;
            _storage = storage;
            _eventPublisher = eventPublisher;
        }
        public void HandleSearchComplete(Outlook.Search searchResult)
        {
            if (searchResult.Tag != Events.IndexingSearchCompleted)
            {
                return;
            }
            //TODO: Clustering saving issue how to preserve tags before they are merged
            var groupedResults = 
                GroupAndIndexResults(searchResult);
            //_storage.Set(Constants.Storage.IndexedHashTags, groupedResults);

            //var clusteredResults = ClusterResults(groupedResults).ToHashSet();
            //var result = clusteredResults.Select(x =>new IndexingResultsViewItem(x.Id, x.NumOfOccurences, x.SearchResults)).ToList();
            
            _storage.SaveHashTags(groupedResults.ToHashSet());
            _eventPublisher.FireEvent(Events.IndexingSearchProcessed);
            //_hashTrackSearchWpfControl.SetIndexingResult(result);
        }
        
        private List<HashTagModel> GroupAndIndexResults(Outlook.Search searchResult)
        {
            // Group and index the search results here
            //TODO: First load indexed hashtags from storage
            //_storage.TryGet(Constants.Storage.IndexedHashTags, out List<HashTagModel> indexedHashTags);
            var indexedHashTags = new List<HashTagModel>(); //indexedHashTags ?? 

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
                        HashTagDtoExtensions.AddNewSearchResult(resultItem, searchResultViewItem);
                        //resultItem.AddNewSearchResult(searchResultViewItem);
                    }
                    else
                    {
                        indexedHashTags.Add(new HashTagModel(hashTag, new HashSet<ArtefactModel> { searchResultViewItem }));
                    }
                }
            }

            return indexedHashTags;
        }

        private List<HashTagModel> ClusterResults(List<HashTagModel> hashtags)
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
                    
                    if (primaryHashtag.ExcludedTagsContain(secondaryHashtag) || secondaryHashtag.ExcludedTagsContain(primaryHashtag))
                    {// Ensure mutual exclusion from exception tags;
                        continue;
                    }
                    
                    //Prevents tag with less usages to be absorbed by tag with more usages automatically
                    if (primaryHashtag.MergedTagsContain(secondaryHashtag) // Merge if in merge list
                        || (!secondaryHashtag.MergedTagsContain(primaryHashtag))) // Prevent merge if in secondary merge list
                            //&& ClusteringClassifier.Classify(primaryHashtag, secondaryHashtag))) // Classify
                    {//TODO: Implement ClusteringClassifier
                        primaryHashtag.MergeHashTag(secondaryHashtag);
                    }
                }
            }

            return hashtags;
        }

        private HashTagModel CombineHashTags(HashTagModel primary, HashTagModel secondary)
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