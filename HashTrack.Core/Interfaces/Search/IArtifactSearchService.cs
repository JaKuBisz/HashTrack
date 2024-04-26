using HashTrack.Core.Models.Search;

namespace HashTrack.Core.Interfaces.Search
{
    public interface IArtifactSearchService
    {
        void SearchAllItemsForTag(HashTagModel hashTag);

        void SearchExactMatch(AdvancedSearchQueryOptions searchQuery);
    }
}