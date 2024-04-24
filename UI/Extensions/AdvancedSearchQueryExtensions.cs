using HashTrack.Core.Enums;
using HashTrack.Core.Models.Search;

namespace HashTrack.Extensions
{
    public static class AdvancedSearchQueryExtensions
    {
        public static bool Verify(this AdvancedSearchQueryOptions searchQuery)
        {
            var result = true;
            result &= searchQuery != null;
            result &= !string.IsNullOrWhiteSpace(searchQuery.Tag);
            result &= !string.IsNullOrWhiteSpace(searchQuery.Keyword);
            result &= searchQuery.Artefacts != ArtifactTypes.None;

            return result;
        }
    }
}