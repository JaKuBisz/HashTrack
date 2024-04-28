using HashTrack.Core.Enums;
using HashTrack.Core.Models.Search;

namespace HashTrack.Extensions
{
    public static class AdvancedSearchQueryExtensions
    {
        public static bool Verify(this SearchTagsQueryOptions searchTagsQuery)
        {
            var result = true;
            result &= searchTagsQuery != null;
            result &= !string.IsNullOrWhiteSpace(searchTagsQuery.EventTag);
            result &= !string.IsNullOrWhiteSpace(searchTagsQuery.Tag) || searchTagsQuery.Tags != null;
            result &= searchTagsQuery.Artefacts != ArtifactTypes.None;

            return result;
        }
    }
}