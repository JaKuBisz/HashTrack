using System.Collections.Generic;
using System.Linq;
using HashTrack.Core;
using HashTrack.Core.Attributes;
using HashTrack.Core.Interfaces.Handlers;
using HashTrack.Core.Models.Search;
using HashTrack.Helpers;
using HashTrack.Interfaces;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace HashTrack.BusinessLogic.Services.Handlers
{
    [RegisterHandler(Events.DefaultSearchCompleted, typeof(ISearchCompleteHandler))]
    public class AdvancedSearchCompleteHandler : ISearchCompleteHandler
    {//TODO: Use events instead of direct call to UI
        //private readonly SidePanelWpfControl _hashTrackSearchWpfControl;

        public AdvancedSearchCompleteHandler()//SidePanelWpfControl hashTrackSearchWpfControl)
        {
            //_hashTrackSearchWpfControl = hashTrackSearchWpfControl;
        }

        public void HandleSearchComplete(Outlook.Search searchResult)
        {
            if (searchResult.Tag != Events.DefaultSearchCompleted)
            {
                return;
            }
        
            Outlook.Results results = searchResult.Results;
            var transformedResults = TransformResultForView(results);
            
            transformedResults = transformedResults
                    .OrderBy(x=> x.Type)
                    .ThenByDescending(x => x.Date)
                    .ToList();

            //_hashTrackSearchWpfControl.SetSearchResults(transformedResults);
        }

        private List<ArtefactModel> TransformResultForView(Outlook.Results results)
        {
          
            var searchResults = new List<ArtefactModel>();
            for (int i = 1; i <= results.Count; i++)
            {
                searchResults.Add(ArtefactItemHelper.MapSearchResultViewItem(results[i]));
            }
            return searchResults;
        }
    }
}