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
            _repository.Update(entity, x => x.Tag == entity.Tag);
            _repository.Save();
        }

        public void SaveHashTags(HashSet<HashTagModel> hashTags)
        {
            foreach (var hashTag in hashTags)
            {
                var entity = hashTag.MapToHashTagEntity();
                _repository.Update(entity, x => x.Tag == entity.Tag);
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
        /*
        private IEnumerable<HashTagModel> EnrichModelsWithChildrenV1(IEnumerable<HashTagEntity> entities)
        {
            var models = entities.Select(x => x.MapToHashTagDto()));
            var entitiesToEnrich = entities.Where(x => x.MergedHashTags.Any() || x.ExcludedHashTags.Any());
            if (!entitiesToEnrich.Any())
            {
                return models;
            }

            foreach (var entity in entitiesToEnrich)
            {
                var model = models.First(x => x.Id == entity.Tag);
                //Get missing entities that need to be retrieved from db

                var tagsToRetrieve = entity.MergedHashTags.Except(entities.Select(x => x.Tag)
                    .Concat(entity.ExcludedHashTags.Except(entities.Select(x => x.Tag))));
                if (tagsToRetrieve.Any())
                {
                    var missingEntities = _repository.GetByTags(tagsToRetrieve);
                    entities = entities.Concat(missingEntities);
                }

                model.MergedHashTags = models
                    .Where(x => entity.MergedHashTags.Contains(x.Id, StringComparer.Ordinal)).ToHashSet();
                model.ExcludedHashTags = models
                    .Where(x => entity.ExcludedHashTags.Contains(x.Id, StringComparer.Ordinal)).ToHashSet();

                if (model.MergedHashTags.Count != entity.MergedHashTags.Count || model.ExcludedHashTags.Count != entity.ExcludedHashTags.Count)
                {
                    entitiesToEnrich.Where(x => entity.)
                    var tagsToRetrieve = entity.MergedHashTags.Except(model.MergedHashTags.Select(x => x.Id))
                        .Concat(entity.ExcludedHashTags.Except(model.ExcludedHashTags.Select(x => x.Id)));
                }
            }

            return models.ToHashSet();
        }

        private HashTagModel EnrichModelWithChildren(HashTagEntity entity)
        {
            var model = entity.MapToHashTagDto();
            if (!entity.MergedHashTags.Any() && !entity.ExcludedHashTags.Any())
            {
                return model;
            }

            var childEntities = _repository.GetByTags(entity.MergedHashTags.Concat(entity.ExcludedHashTags));
                //.Select(x => x.MapToHashTagDto());
            foreach (var childModel in childEntities)
            {
                EnrichModelWithChildren(childModel);
            }

            model.MergedHashTags = childEntities
                .Where(x => entity.MergedHashTags.Contains(x.Id, StringComparer.Ordinal)).ToHashSet();
            model.ExcludedHashTags = childEntities
                .Where(x => entity.ExcludedHashTags.Contains(x.Id, StringComparer.Ordinal)).ToHashSet();

            return model;
        }*/

/*
        private HashSet<HashTagModel> GetHashTagsModels(IEnumerable<HashTagEntity> hashTagEntities)
        {
            return hashTagEntities.Select(x => x.MapToHashTagDto()).ToHashSet();
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
        }
    }*/
}