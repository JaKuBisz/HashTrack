using System;
using HashTrack.Core.Models.Search;

namespace HashTrack.Core.Interfaces.Search
{
    public interface ISearchService
    {
        void SearchTags(SearchTagsQueryOptions searchTagsQuery);
        void PerformIndexing(DateTime? from = null, DateTime? to = null);
        void SearchAllItemsForTag(HashTagModel hashTag);
    }
}