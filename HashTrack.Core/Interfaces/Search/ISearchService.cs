using HashTrack.Core.Models.Search;

namespace HashTrack.Core.Interfaces.Search
{
    public interface ISearchService
    {
        void SearchAllItemsForTag(HashTagModel hashTag);

        void SearchExactMatch(AdvancedSearchQueryOptions searchQuery);
    }
}