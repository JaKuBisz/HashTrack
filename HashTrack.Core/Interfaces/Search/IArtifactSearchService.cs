using HashTrack.Core.Models.Search;

namespace HashTrack.Core.Interfaces.Search
{
    public interface IArtifactSearchService
    {
        void SearchExactMatch(AdvancedSearchQueryOptions searchQuery);
    }
}