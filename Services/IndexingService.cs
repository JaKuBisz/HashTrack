using System;
using HashTrack.DTOs;
using HashTrack.Enums;
using HashTrack.Helpers;

namespace HashTrack.Services
{
    /// <summary>
    /// This service serches for all occurences of "#somwWord" in all rtefacts and returns the list of all found hashtags and stATISCITCS OF THEIR USAGE. in the background.
    /// </summary>
    [RegisterService(typeof(IndexingService))]
    public class IndexingService
    {
        private readonly ArtifactSearchService _artifactSearchService;
        public IndexingService(ArtifactSearchService artifactSearchService)
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
                Tag = Constants.IndexingSearchTag,
                ExactMatch = false
            });
        }
        
    }
}