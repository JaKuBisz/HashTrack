using System.Linq;
using HashTrack.Core.Models.Search;
using HashTrack.Persistence.Entities;

namespace HashTrack.Persistence.Mappers
{
    public static class HashTagEntityMappers
    {
        public static HashTagEntity MapToHashTagEntity(this HashTagModel model)
        {
            return new HashTagEntity
            {
                Tag = model.Id,
                NumOfOccurrences = model.NumOfOccurrences,
                LastUpdated = model.LastUpdated,
                MergedHashTags = model.MergedHashTags.Select(x => x.Id).ToHashSet(),
                ExcludedHashTags = model.ExcludedHashTags.Select(x => x.Id).ToHashSet(),
                ArtefactIds = model.ArtifactsIds,
                CreateFolder = model.CreateFolder,
                CreateCategory = model.CreateCategory,
                FolderName = model.FolderName,
                CategoryName = model.CategoryName,
                CategoryColor = (int)model.CategoryColor
            };
        }
    }
}