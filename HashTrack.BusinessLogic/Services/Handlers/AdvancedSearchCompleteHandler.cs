using System.Collections.Generic;
using System.Linq;
using HashTrack.Core;
using HashTrack.Core.Attributes;
using HashTrack.Core.Interfaces;
using HashTrack.Core.Interfaces.Handlers;
using HashTrack.Core.Models.Search;
using HashTrack.Helpers;
using HashTrack.Interfaces;
using HashTrack.Interfaces.Indexing;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace HashTrack.BusinessLogic.Services.Handlers
{
    [RegisterHandler(Events.DefaultSearchCompleted, typeof(ISearchCompleteHandler))]
    public class AdvancedSearchCompleteHandler : ISearchCompleteHandler
    {//TODO: Use events instead of direct call to UI
        //private readonly SidePanelWpfControl _hashTrackSearchWpfControl;
        private readonly IEventAggregator _eventAggregator;
        private readonly ICache _cache;
        private readonly IIndexingService _indexingService;
        


        public AdvancedSearchCompleteHandler(IEventAggregator eventAggregator, ICache cache, IIndexingService indexingService)//SidePanelWpfControl hashTrackSearchWpfControl)
        {
            _eventAggregator = eventAggregator;
            _cache = cache;
            _indexingService = indexingService;
        }

        public void HandleSearchComplete(Outlook.Search searchResult)
        {
            if (searchResult.Tag != Events.DefaultSearchCompleted)
            {
                return;
            }
        
            //Automatically index the deep search results
            _indexingService.IndexSearchResults(searchResult);
            
            Outlook.Results results = searchResult.Results;
            var transformedResults = TransformResultForView(results);
            
            transformedResults = transformedResults
                    .OrderBy(x=> x.Type)
                    .ThenByDescending(x => x.Date)
                    .ToList();

            //_hashTrackSearchWpfControl.SetSearchResults(transformedResults);
            _cache.Set(Constants.Storage.Artefacts, transformedResults);
            _eventAggregator.FireEvent(Events.DefaultSearchProcessed);
        }

        private List<ArtefactModel> TransformResultForView(Outlook.Results results)
        {
          
            var searchResults = new List<ArtefactModel>();
            foreach (var item in results)
            {
                var searchResultViewItem = ArtefactItemHelper.MapSearchResultViewItem(item);
                if (searchResultViewItem != null)
                {
                    searchResults.Add(searchResultViewItem);
                }
            }
            return searchResults;
        }
    }
}