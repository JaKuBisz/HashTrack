using System;
using System.Collections.Generic;
using System.Linq;
using HashTrack.Core.Enums;
using Microsoft.Office.Interop.Outlook;

namespace HashTrack.Core.Models.Search
{
    public class HashIdModel : UniqueId<string>
    {//TODO Implement invalidating cache to not recalculate tge Totals each request occurrences 
        public int NumOfOccurrences { get; set; }
        public HashSet<ArtefactModel> SearchResults { get; set; } = new HashSet<ArtefactModel>();
        public HashSet<Guid> ArtifactsIds { get; set; } = new HashSet<Guid>();
        public DateTime LastUpdated { get; set; } 
        public HashSet<HashIdModel> MergedHashTags { get; set; } = new HashSet<HashIdModel>(); //TODO: is null
        public HashSet<HashIdModel> ExcludedHashTags { get; set; } = new HashSet<HashIdModel>();
        public bool HasMergedTags => MergedHashTags.Any();
        public bool HasExcludedTags => ExcludedHashTags.Any();
        //Settings
        public bool CreateFolder { get; set; }
        public bool CreateCategory { get; set; }
        public string FolderName { get; set; }
        public string CategoryName { get; set; }
        public CategoryColor CategoryColor { get; set; } = CategoryColor.olCategoryColorNone;

        public HashIdModel()
        { }
  
        public HashIdModel(string id) : base(id)
        { }

        public int TotalNumOfOccurences()
        {
            return NumOfOccurrences + MergedHashTags.Sum(x => x.TotalNumOfOccurences());
        }

        public HashSet<Guid> TotalArtifactsIds()
        {
            return ArtifactsIds.Concat(MergedHashTags.SelectMany(x => x.TotalArtifactsIds())).ToHashSet();
        }

        public HashSet<ArtefactModel> TotalSearchResults()
        {
            return SearchResults.Concat(MergedHashTags.SelectMany(x => x.TotalSearchResults())).ToHashSet();
        }
        
        public HashSet<HashIdModel> TotalMergedHashTags()
        {
            return MergedHashTags.Concat(MergedHashTags.SelectMany(x => x.TotalMergedHashTags())).ToHashSet();
        }
        
        public HashSet<HashIdModel> TotalExcludedHashTags()
        {
            return ExcludedHashTags.Concat(ExcludedHashTags.SelectMany(x => x.TotalExcludedHashTags())).ToHashSet();
        }
    }
}