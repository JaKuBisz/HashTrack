using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using HashTrack.Core.Enums;
using HashTrack.Core.Models.Search;
using HashTrack.Persistence.Entities;

namespace HashTrack.Persistence.Mappers
{
    public static class HashTagEntityToHashTagDtoMapper
    {
        public static HashTagModel MapToHashTagDto(this HashTagEntity entity)
        { 
            return new HashTagModel
            {
                Tag = entity.Tag,
                NumOfOccurrences = entity.NumOfOccurrences,
                LastUpdated = entity.LastUpdated,
                MergedHashTags = entity.MergedHashTags.Select(x => x.MapToHashTagDto()).ToHashSet(),
                ExcludedHashTags = entity.ExcludedHashTags.Select(x => x.MapToHashTagDto()).ToHashSet(),
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