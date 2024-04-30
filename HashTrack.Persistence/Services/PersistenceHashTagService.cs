using System;
using System.Collections.Generic;
using System.Linq;
using HashTrack.Core.Attributes;
using HashTrack.Core.Enums;
using HashTrack.Core.Extensions;
using HashTrack.Core.Interfaces;
using HashTrack.Core.Models.Search;
using HashTrack.Persistence.Entities;
using HashTrack.Persistence.Interfaces;
using HashTrack.Persistence.Mappers;

namespace HashTrack.Persistence.Services
{
    [RegisterService(LifeCycle.Transient, typeof(IPersistenceHashTagService))]
    public class PersistenceHashTagService : IPersistenceHashTagService
    {
        private readonly IHashTagRepository _repository;

        public PersistenceHashTagService(IHashTagRepository repository)
        {
            _repository = repository;
        }

        public HashSet<HashTagModel> GetAllHashTags()
        {
            var hashTagEntities = _repository.GetAll();
            if (hashTagEntities == null) return new HashSet<HashTagModel>();

            return GetEnrichedModels(hashTagEntities);
        }

        public HashTagModel GetHashTag(string tag)
        {
            var hashtagEntity = _repository.GetByTag(tag);
            if (hashtagEntity == null) return null;

            return GetEnrichedModels(new[] { hashtagEntity }).First();
        }

        public void SaveHashTag(HashTagModel hashTag)
        {
            var entity = hashTag.MapToHashTagEntity();
            _repository.Upsert(entity, x => x.Tag == entity.Tag);
            _repository.Save();
        }

        public void SaveHashTags(HashSet<HashTagModel> hashTags)
        {
            foreach (var hashTag in hashTags)
            {
                var entity = hashTag.MapToHashTagEntity();
                _repository.Upsert(entity, x => x.Tag == entity.Tag);
            }

            _repository.Save();
        }

        public HashSet<HashTagModel> GetHashTagsByIds(IEnumerable<string> Ids)
        {
            var hashTagEntities = _repository.GetByTags(Ids);
            if (hashTagEntities == null) return new HashSet<HashTagModel>();

            return GetEnrichedModels(hashTagEntities);
        }

        private HashSet<HashTagModel> GetEnrichedModels(IEnumerable<HashTagEntity> entities)
        {
            var originalModels = entities.Select(x => x.MapToHashTagDto()).ToHashSet();
            var entitiesToEnrich = entities.Where(x => x.MergedHashTags.Any() || x.ExcludedHashTags.Any());
            if (!entitiesToEnrich.Any()) return originalModels.ToHashSet();

            var missingChildEntities = RetrieveMissingEntities();
            var enrichedModels = new HashSet<HashTagModel>(originalModels);
            if (missingChildEntities != null) enrichedModels = GetEnrichedModels(missingChildEntities);
            //models = models.Concat(enrichedChildModels);
            foreach (var entity in entitiesToEnrich)
                originalModels.AddOrReplace(GetEnrichedModelForEntity(entity), true);

            return originalModels.ToHashSet();

            HashTagModel GetEnrichedModelForEntity(HashTagEntity entity)
            {
                var model = originalModels.First(x => x.Id == entity.Tag);
                //Get missing entities that need to be retrieved from db

                model.MergedHashTags = enrichedModels
                    .Where(x => entity.MergedHashTags.Contains(x.Id, StringComparer.Ordinal)).ToHashSet();
                model.ExcludedHashTags = enrichedModels
                    .Where(x => entity.ExcludedHashTags.Contains(x.Id, StringComparer.Ordinal)).ToHashSet();

                return model;
            }

            IEnumerable<HashTagEntity> RetrieveMissingEntities()
            {
                var tagsToRetrieve = entities.SelectMany(x => x.MergedHashTags)
                    .Concat(entities.SelectMany(x => x.ExcludedHashTags))
                    .Except(entities.Select(x => x.Tag));
                if (tagsToRetrieve.Any()) return _repository.GetByTags(tagsToRetrieve);
                //entities = entities.Concat(missingEntities);
                return null;
            }
        }
    }
}