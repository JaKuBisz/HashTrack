using System;
using HashTrack.Core;
using HashTrack.Core.Attributes;
using HashTrack.Core.Enums;
using HashTrack.Core.Interfaces.Search;
using HashTrack.Core.Models.Search;
using HashTrack.Interfaces.Indexing;

namespace HashTrack.BusinessLogic.Services
{
    /// <summary>
    /// This service serches for all occurences of "#somwWord" in all rtefacts and returns the list of all found hashtags and stATISCITCS OF THEIR USAGE. in the background.
    /// </summary>
    [RegisterService(LifeCycle.Transient, typeof(IIndexingService))]
    public class IndexingService : IIndexingService
    {
        private readonly IArtifactSearchService _artifactSearchService;
        public IndexingService(IArtifactSearchService artifactSearchService)
        {
            _artifactSearchService = artifactSearchService;
        }
        
        public void IndexAllArtifacts()
        {
            // Index all artifacts in last 30 days
            var from = DateTime.Now.AddDays(-30);
            _artifactSearchService.SearchExactMatch(new AdvancedSearchQueryOptions
            {
                Keyword = "#",
                Artefacts = ArtifactTypes.All,
                From = from,
                To = null,
                Tag = Events.IndexingSearchCompleted,
                ExactMatch = false
            });
        }
        
    }
}