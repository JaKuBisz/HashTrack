using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using HashTrack.Core.Attributes;
using HashTrack.Core.Enums;
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
            if (hashTagEntities == null)
            {
                return new HashSet<HashTagModel>();
            }
            return GetHashTagsModels(hashTagEntities);
        }

        public HashTagModel GetHashTag(string tag)
        {
            var hashtagEntity = _repository.GetByTag(tag);
            if (hashtagEntity == null)
            {
                return null;
            }
            return hashtagEntity.MapToHashTagDto();
        }

        public void SaveHashTag(HashTagModel hashTag)
        {
            var entity = hashTag.MapToHashTagEntity();
            _repository.Update(entity, x => x.Tag == entity.Tag);
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

        private HashSet<HashTagModel> GetHashTagsModels(IEnumerable<HashTagEntity> hashTagEntities)
        {
            return hashTagEntities.Select(x => x.MapToHashTagDto()).ToHashSet();
            //TODO: Fix references on HashTags
            /*
            //Disabled referencing of entities
            // HashTagModels need to ahve references to each other so we need to map each object and then enrich with the references
            var hashTags = hashTagEntities
                .Select(hashTagEntity => (model: hashTagEntity.MapToHashTagDto(), entity: hashTagEntity));

            //Disabled referencing of entities
           foreach (var (model, entity) in hashTags)
            {
                var tag = entity.Tag;

                if (entity.MergedHashTags.Any())
                {
                    var mergedTags = hashTags
                        .Where(x => entity.MergedHashTags
                            .Contains(x.entity.Tag, StringComparer.OrdinalIgnoreCase))
                        .Select(x => x.model)
                        .ToHashSet();
                    model.MergedHashTags = mergedTags;
                }

                if (entity.ExcludedHashTags.Any())
                {
                    var excludedTags = hashTags
                        .Where(x => entity.ExcludedHashTags
                            .Contains(x.entity.Tag, StringComparer.OrdinalIgnoreCase))
                        .Select(x => x.model)
                        .ToHashSet();
                    model.ExcludedHashTags = excludedTags;
                }
            }

            return hashTags.Select(x => x.model).ToHashSet();*/
        }
/*
        private HashSet<HashTagModel> GetHashTagsModel(HashTagModel model, IEnumerable<HashTagEntity> mergedHashTagEntities, IEnumerable<HashTagEntity> excludedHashTagEntities)
        {
            var mergedHashTags = mergedHashTagEntities.Select(x => x.MapToHashTagDto()).ToHashSet();
            var excludedHashTags = excludedHashTagEntities.Select(x => x.MapToHashTagDto()).ToHashSet();

            model.MergedHashTags = mergedHashTags;
            model.ExcludedHashTags = excludedHashTags;

            return model;
        }
        {
            // HashTagModels need to ahve references to each other so we need to map each object and then enrich with the references
            var hashTags = hashTagEntities
                .Select(hashTagEntity => (model: hashTagEntity.MapToHashTagDto(), entity: hashTagEntity));

            foreach (var (model, entity) in hashTags)
            {
                var tag = entity.Tag;

                if (entity.MergedHashTags.Any())
                {
                    var mergedTags = hashTags
                        .Where(x => entity.MergedHashTags
                            .Contains(x.entity.Tag, StringComparer.OrdinalIgnoreCase))
                        .Select(x => x.model)
                        .ToHashSet();
                    model.MergedHashTags = mergedTags;
                }

                if (entity.ExcludedHashTags.Any())
                {
                    var excludedTags = hashTags
                        .Where(x => entity.ExcludedHashTags
                            .Contains(x.entity.Tag, StringComparer.OrdinalIgnoreCase))
                        .Select(x => x.model)
                        .ToHashSet();
                    model.ExcludedHashTags = excludedTags;
                }
            }

            return hashTags.Select(x => x.model).ToHashSet();
        }
        
        private             foreach (var (model, entity) in hashTags)
        {
            var tag = entity.Tag;

            if (entity.MergedHashTags.Any())
            {
                var mergedTags = hashTags
                    .Where(x => entity.MergedHashTags
                        .Contains(x.entity.Tag, StringComparer.OrdinalIgnoreCase))
                    .Select(x => x.model)
                    .ToHashSet();
                model.MergedHashTags = mergedTags;
            }

            if (entity.ExcludedHashTags.Any())
            {
                var excludedTags = hashTags
                    .Where(x => entity.ExcludedHashTags
                        .Contains(x.entity.Tag, StringComparer.OrdinalIgnoreCase))
                    .Select(x => x.model)
                    .ToHashSet();
                model.ExcludedHashTags = excludedTags;
            }
        }*/
    }
}