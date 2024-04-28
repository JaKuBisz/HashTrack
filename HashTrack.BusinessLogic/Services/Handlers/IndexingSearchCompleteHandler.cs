
using HashTrack.Core;
using HashTrack.Core.Attributes;
using HashTrack.Core.Interfaces.Handlers;
using HashTrack.Interfaces.Indexing;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace HashTrack.BusinessLogic.Services.Handlers
{
    [RegisterHandler(Events.IndexingSearchCompleted, typeof(ISearchCompleteHandler))]
    public class IndexingSearchCompleteHandler : ISearchCompleteHandler
    {
        private readonly IIndexingService _indexingService;

        public IndexingSearchCompleteHandler(IIndexingService indexingService)
        {
            _indexingService = indexingService;
        }

        public void HandleSearchComplete(Outlook.Search searchResult)
        {
            if (searchResult.Tag != Events.IndexingSearchCompleted)
            {
                return;
            }

            _indexingService.IndexSearchResults(searchResult);
        }
    }
}