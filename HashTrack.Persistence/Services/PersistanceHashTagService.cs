using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using HashTrack.Core.Models.Search;
using HashTrack.Persistence.Entities;
using HashTrack.Persistence.Interfaces;
using HashTrack.Persistence.Mappers;

namespace HashTrack.Persistence.Services
{
    public class PersistanceHashTagService : IPersistanceHashTagService
    {
        private IHashTagRepository _repository;
        private IDbSet<HashTagEntity> _dbSet;

        public PersistanceHashTagService(IHashTagRepository repository, DbContext context)
        {
            _repository = repository;
            _dbSet = context.Set<HashTagEntity>();
        }

        public HashSet<HashTagDto> GetAllHashTags()
        {
            var hashTagEntities = _repository.GetAll().ToHashSet();
            var result = hashTagEntities.Select(x => x.MapToHashTagDto(_dbSet));
            
            return result.ToHashSet();
        }
    }
}