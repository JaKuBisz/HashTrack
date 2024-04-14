using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using HashTrack.Core.Models.Search;
using HashTrack.Persistence.Entities;
using HashTrack.Persistence.Interfaces;

namespace HashTrack.Persistence.Mappers
{
    public static class HashTagEntityToHashTagDtoMapper
    {
        public static HashTagDto MapToHashTagDto(this HashTagEntity entity, IDbSet<HashTagEntity> dbSet)
        {
            var mergedHashTagEntities =
                dbSet.Where(x => entity.MergedHashTags.Contains(x.Tag, StringComparer.OrdinalIgnoreCase));
            
            var excludedHashTagEntities =
                dbSet.Where(x => entity.ExcludedHashTags.Contains(x.Tag, StringComparer.OrdinalIgnoreCase));
            
            var mergedHashTags = mergedHashTagEntities.Select(x => x.MapToHashTagDto(dbSet)).ToHashSet();
            var excludedHashTags = excludedHashTagEntities.Select(x => x.MapToHashTagDto(dbSet)).ToHashSet();

            return new HashTagDto
            {
                Id = entity.Tag,
                NumOfOccurences = entity.NumOfOccurrences,
                LastUpdated = entity.LastUpdated,
                MergedHashTags = mergedHashTags,
                ExcludedHashTags = excludedHashTags
            };
        }

        public static HashTagDto MapToHashTagDto(this HashTagEntity entity, HashSet<HashTagDto> hashTagDtos)
        {
            var mergedHashTags = 
                hashTagDtos
                    .Where(x => entity.MergedHashTags
                        .Contains(x.Id, StringComparer.OrdinalIgnoreCase))
                    .ToHashSet();
            var excludedHashTags = 
                hashTagDtos
                    .Where(x => entity.ExcludedHashTags
                        .Contains(x.Id, StringComparer.OrdinalIgnoreCase))
                    .ToHashSet();

            return new HashTagDto
            {
                Id = entity.Tag,
                NumOfOccurences = entity.NumOfOccurrences,
                LastUpdated = entity.LastUpdated,
                MergedHashTags = mergedHashTags,
                ExcludedHashTags = excludedHashTags
            };
        }
    }
}