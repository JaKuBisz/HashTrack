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
    }
}