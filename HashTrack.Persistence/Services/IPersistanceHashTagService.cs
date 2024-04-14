using System.Collections.Generic;
using HashTrack.Core.Models.Search;

namespace HashTrack.Persistence.Services
{
    public interface IPersistanceHashTagService
    {
        HashSet<HashTagDto> GetAllHashTags();
    }
}