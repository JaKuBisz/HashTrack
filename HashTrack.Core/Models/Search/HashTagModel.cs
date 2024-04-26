using System;
using System.Collections.Generic;
using System.Linq;
using HashTrack.Core.Enums;
using Microsoft.Office.Interop.Outlook;

namespace HashTrack.Core.Models.Search
{
    public class HashTagModel : UniqueTag
    {//TODO Implement invalidating cache to not recalculate tge Totals each request occurrences 
        public int NumOfOccurrences { get; set; }
        public HashSet<ArtefactModel> SearchResults { get; set; } = new HashSet<ArtefactModel>();
        public HashSet<Guid> ArtifactsIds { get; set; } = new HashSet<Guid>();
        public DateTime LastUpdated { get; set; } 
        public HashSet<HashTagModel> MergedHashTags { get; set; } = new HashSet<HashTagModel>(); //TODO: is null
        public HashSet<HashTagModel> ExcludedHashTags { get; set; } = new HashSet<HashTagModel>();
        public bool IsMerged => MergedHashTags.Any();
        //Settings
        public bool CreateFolder { get; set; }
        public bool CreateCategory { get; set; }
        public string FolderName { get; set; }
        public string CategoryName { get; set; }
        public CategoryColor CategoryColor { get; set; } = CategoryColor.olCategoryColorNone;

        public HashTagModel()
        { }
  
        public HashTagModel(string tag,HashSet<ArtefactModel> searchResults, int numOfOccurrences = 1) : base(tag)
        {
            NumOfOccurrences = numOfOccurrences;
            SearchResults = searchResults; 
        }

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
        
        public HashSet<HashTagModel> TotalMergedHashTags()
        {
            return MergedHashTags.Concat(MergedHashTags.SelectMany(x => x.TotalMergedHashTags())).ToHashSet();
        }
        
        public HashSet<HashTagModel> TotalExcludedHashTags()
        {
            return ExcludedHashTags.Concat(ExcludedHashTags.SelectMany(x => x.TotalExcludedHashTags())).ToHashSet();
        }
    }
}