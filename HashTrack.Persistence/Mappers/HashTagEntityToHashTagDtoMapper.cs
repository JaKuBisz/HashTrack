using System.Collections.Generic;
using HashTrack.Core.Enums;
using HashTrack.Core.Models.Search;
using HashTrack.Persistence.Entities;

namespace HashTrack.Persistence.Mappers
{
    public static class HashTagEntityToHashTagDtoMapper
    {
        public static HashTagModel MapToHashTagDto(this HashTagEntity entity)
        {
            var mergedHashTagModels = new HashSet<HashTagModel>();
            var excludedHashTagModels = new HashSet<HashTagModel>();
            return new HashTagModel
            {
                Id = entity.Tag,
                NumOfOccurrences = entity.NumOfOccurrences,
                LastUpdated = entity.LastUpdated,
                MergedHashTags = mergedHashTagModels,
                ExcludedHashTags = excludedHashTagModels,
                ArtifactsIds = entity.ArtefactIds,
                SearchResults = new HashSet<ArtefactModel>(),
                CreateFolder = entity.CreateFolder,
                CreateCategory = entity.CreateCategory,
                FolderName = entity.FolderName,
                CategoryName = entity.CategoryName,
                CategoryColor = (CategoryColor)entity.CategoryColor
            };
        }
        /*
        public static HashTagModel MapToHashTagDto(this HashTagEntity entity, IDbSet<HashTagEntity> dbSet)
        {
            var mergedHashTagEntities =
                dbSet.Where(x => entity.MergedHashTags.Contains(x.Tag, StringComparer.OrdinalIgnoreCase));

            var excludedHashTagEntities =
                dbSet.Where(x => entity.ExcludedHashTags.Contains(x.Tag, StringComparer.OrdinalIgnoreCase));

            var mergedHashTags = mergedHashTagEntities.Select(x => x.MapToHashTagDto(dbSet)).ToHashSet();
            var excludedHashTags = excludedHashTagEntities.Select(x => x.MapToHashTagDto(dbSet)).ToHashSet();

            return new HashTagModel
            {
                Id = entity.Tag,
                NumOfOccurrences = entity.NumOfOccurrences,
                LastUpdated = entity.LastUpdated,
                MergedHashTags = mergedHashTags,
                ExcludedHashTags = excludedHashTags
            };
        }

        public static HashTagModel MapToHashTagDto(this HashTagEntity entity, HashSet<HashTagEntity> hashTagEntities)
        {
            var mergedHashTagEntities = hashTagEntities
                .Where(x => entity.MergedHashTags.Contains(x.Tag, StringComparer.OrdinalIgnoreCase));
            var excludedHashTagEntities = hashTagEntities
                .Where(x => entity.ExcludedHashTags.Contains(x.Tag, StringComparer.OrdinalIgnoreCase));

            var mergedHashTags = mergedHashTagEntities
                .Select(x => x.MapToHashTagDto(hashTagEntities)).ToHashSet();
            var excludedHashTags = excludedHashTagEntities
                .Select(x => x.MapToHashTagDto(hashTagEntities)).ToHashSet();

            return new HashTagModel
            {
                Id = entity.Tag,
                NumOfOccurrences = entity.NumOfOccurrences,
                LastUpdated = entity.LastUpdated,
                MergedHashTags = mergedHashTags,
                ExcludedHashTags = excludedHashTags
            };
        }*/
    }
}