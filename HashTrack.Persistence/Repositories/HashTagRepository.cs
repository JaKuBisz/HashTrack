using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using HashTrack.Core.Attributes;
using HashTrack.Core.Enums;
using HashTrack.Persistence.Entities;
using HashTrack.Persistence.Interfaces;

namespace HashTrack.Persistence.Repositories
{
    [RegisterService(LifeCycle.Transient, typeof(IHashTagRepository))]
    public class HashTagRepository : Repository<HashTagEntity>, IHashTagRepository
    {
        public HashTagRepository(DbContext context) : base(context)
        {
        }

        public HashTagEntity GetByTag(string tag)
        {
            return _dbSet.FirstOrDefault(x => x.Tag == tag);
        }

        public HashSet<HashTagEntity> GetByTags(IEnumerable<string> tags)
        {
            return _dbSet.Where(x => tags.Contains(x.Tag)).ToHashSet();
        }
    }
}