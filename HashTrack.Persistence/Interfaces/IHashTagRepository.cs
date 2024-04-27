using System.Collections.Generic;
using HashTrack.Persistence.Entities;
using HashTrack.Persistence.Repositories;

namespace HashTrack.Persistence.Interfaces
{
    public interface IHashTagRepository : IRepository<HashTagEntity>
    {
        HashTagEntity GetByTag(string tag);
        HashSet<HashTagEntity> GetByTags(IEnumerable<string> tags);
    }
}