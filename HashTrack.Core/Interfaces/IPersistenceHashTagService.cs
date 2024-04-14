using System.Collections.Generic;
using HashTrack.Core.Models.Search;

namespace HashTrack.Core.Interfaces
{
    public interface IPersistenceHashTagService
    {
        HashSet<HashTagModel> GetAllHashTags();
        HashTagModel GetHashTag(string tag);
        void SaveHashTag(HashTagModel hashTag);
        void SaveHashTags(HashSet<HashTagModel> hashTags);
    }
}