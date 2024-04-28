using System;
using HashTrack.Core;
using HashTrack.Core.Attributes;
using HashTrack.Core.Enums;
using HashTrack.Core.Interfaces.Search;
using HashTrack.Core.Models.Search;
using HashTrack.Interfaces.Indexing;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HashTrack.BusinessLogic.Extensions;
using HashTrack.Core.Extensions;
using HashTrack.Core.Interfaces;
using HashTrack.Core.Interfaces.Clustering;
using HashTrack.Helpers;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace HashTrack.BusinessLogic.Services
{
    /// <summary>
    /// This service serches for all occurences of "#somwWord" in all rtefacts and returns the list of all found hashtags and stATISCITCS OF THEIR USAGE. in the background.
    /// </summary>
    [RegisterService(LifeCycle.Transient, typeof(IIndexingService))]
    public class IndexingService : IIndexingService
    {        
        private readonly IPersistenceHashTagService _storage;
        private readonly IEventAggregator _eventAggregator;
        private readonly ICache _cache;
        private readonly IClusteringClassifier _clusteringClassifier;
        private readonly ICategoryManagerService _categoryManager;
        private readonly IMessageService _messageService;

        public IndexingService(
            IPersistenceHashTagService storage, ICache cache, IEventAggregator eventAggregator,
            IClusteringClassifier clusteringClassifier, ICategoryManagerService categoryManager,
            IMessageService messageService)//SidePanelWpfControl hashTrackSearchWpfControl, IStorage storage)
        {   
            //_hashTrackSearchWpfControl = hashTrackSearchWpfControl;
            _storage = storage;
            _cache = cache;
            _eventAggregator = eventAggregator;
            _clusteringClassifier = clusteringClassifier;
            _categoryManager = categoryManager;
            _messageService = messageService;
        }
        
        public void IndexSearchResults(Outlook.Search searchResult)
        {
            //Offload from UI thread
            Task.Run(() => PerformIndexing(searchResult));
        }
        
        
        private void PerformIndexing(Outlook.Search searchResult)
        {
            try
            {
                var groupedResults =
                    ExtractTagsAndEnrichArtefacts(searchResult);
                //_storage.Set(Constants.Storage.IndexedHashTags, groupedResults);

                var clusteredResults = ClusterHashTags(groupedResults).ToHashSet();
                //var result = clusteredResults.Select(x =>new IndexingResultsViewItem(x.Id, x.NumOfOccurrences, x.SearchResults)).ToList();

                _storage.SaveHashTags(clusteredResults);
                _cache.SetHashTags(clusteredResults.ToList());
                _eventAggregator.FireEvent(Events.HashTagsUpdated);
            }
            catch (System.Exception e)
            {
                _messageService.ShowMessage(e, "Error occured during indexing");
            }
        }
        
        private List<HashTagModel> ExtractTagsAndEnrichArtefacts(Outlook.Search searchResult)
        {
            // Group and index the search results here
            //TODO: First load indexed hashtags from storage
            var indexedHashTags = _storage.GetAllHashTags().ToList();
            foreach (var item in searchResult.Results)
            {
                var textContent = ArtefactItemHelper.GetBody(item);
                if (textContent == null)
                {
                    continue;
                }
                
                HashSet<string> hashTags = ExtractHashtags(textContent);
                EnrichArtefact(item, hashTags);
                var searchResultViewItem = ArtefactItemHelper.MapSearchResultViewItem(item);
                if (searchResultViewItem is null)
                {
                    continue;
                }
                
                foreach (var hashTag in hashTags)
                {
                    if (!indexedHashTags.TryGetByKey(hashTag, out var hashTagModel))
                    {
                        hashTagModel = new HashTagModel(hashTag);
                        indexedHashTags.Add(hashTagModel);
                    }
                    
                    hashTagModel.AddNewSearchResult(searchResultViewItem);


                    if (hashTagModel.CreateCategory)
                    {
                        _categoryManager.AddItemToCategory(hashTagModel, item);
                    }
                }
            }

            return indexedHashTags;
        }

        private void EnrichArtefact(object item, HashSet<string> hashTags)
        {
            var properties = GetArtefactUserProperties(item);
            if (properties is null)
            {
                return;
            }
            
            EnrichArtefactWithHashTags(properties, hashTags.ToArray());
            EnrichArtefactWithId(properties);
            SaveItem(item);
        }
        
        private void EnrichArtefactWithHashTags(Outlook.UserProperties properties, string[] hashTags)
        {
            var tagsProperty = properties.Find(Constants.CustomProperties.Tags);
            if (tagsProperty != null)
            {
                //Tag already indexed skip
                //Should not be skipped for editable items
                //return;
            }

            if (tagsProperty is null)
            {

                tagsProperty = properties.Add(
                    Constants.CustomProperties.Tags,
                    Outlook.OlUserPropertyType.olKeywords,
                    true,
                    1);
            }

            tagsProperty.Value = hashTags;
        }

        private Guid EnrichArtefactWithId(Outlook.UserProperties properties)
        {
            var property = properties.Find(Constants.CustomProperties.artefactID);

            if (property is null)
            {

                property = properties.Add(
                    Constants.CustomProperties.artefactID,
                    Outlook.OlUserPropertyType.olText,
                    true,
                    1);
                property.Value = Guid.NewGuid().ToString();
            }

            return Guid.TryParse(property.Value.ToString(), out var id) ? id : Guid.Empty;
        }

        private Outlook.UserProperties GetArtefactUserProperties(object item)
        {
            switch (item)
            {
                case Outlook._MailItem mailItem:
                    return mailItem.UserProperties;
                    break;
                case Outlook._ContactItem contactItem:
                    return contactItem.UserProperties;
                    break;
                case Outlook._AppointmentItem appointmentItem:
                    return appointmentItem.UserProperties;
                    break;
                case Outlook._TaskItem taskItem:
                    return taskItem.UserProperties;
                    break;
                default:
                    return null;
            }
        }

        private void SaveItem(object item)
        {
            switch (item)
            {
                case Outlook._MailItem mailItem:
                    mailItem.Save();
                    break;
                case Outlook._ContactItem contactItem:
                    contactItem.Save();
                    break;
                case Outlook._AppointmentItem appointmentItem:
                    appointmentItem.Save();
                    break;
                case Outlook._TaskItem taskItem:
                    taskItem.Save();
                    break;
                default:
                    return;
            }
        }

        private List<HashTagModel> ClusterHashTags(List<HashTagModel> hashtags)
        {
            //We order hastags so we dont have to check which one has more occurrences and automatically know its the main hashtag
            var orderedHashtags = hashtags.OrderByDescending(x => x.NumOfOccurrences).ToList();
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
                        || (!secondaryHashtag.MergedTagsContain(primaryHashtag)) // Prevent merge if in secondary merge list
                            && _clusteringClassifier.Classify(primaryHashtag, secondaryHashtag)) // Classify
                    {
                        primaryHashtag.MergeHashTag(secondaryHashtag);
                    }
                }
            }

            return hashtags;
        }

        private HashTagModel CombineHashTags(HashTagModel primary, HashTagModel secondary)
        {
            primary.NumOfOccurrences += secondary.NumOfOccurrences;
            foreach (var result in secondary.SearchResults)
            {
                primary.SearchResults.Add(result);
            }

            return primary;
        }

        private HashSet<string> ExtractHashtags(string text)
        {
            var hashtags = new HashSet<string>();
            var matches = Regex.Matches(text, @"(?<!\w)#\w+"); //Matches hashtags that are not preceded by a word
            foreach (Match match in matches)
            {
                string hashtag = match.Value;
                hashtags.Add(hashtag);
            }

            return hashtags;
        }
    }
}