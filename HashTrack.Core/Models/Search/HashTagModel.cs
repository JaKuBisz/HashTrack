using System;
using System.Collections.Generic;
using System.Linq;

namespace HashTrack.Core.Models.Search
{
    public class HashTagModel : UniqueTag
    {//TODO Implement invalidating cache to not recalculate tge Totals each request
        public int NumOfOccurences { get; set; }
        public HashSet<ArtefactModel> SearchResults { get; set; }
        public DateTime LastUpdated { get; set; } 
        public HashSet<HashTagModel> MergedHashTags { get; set; } = new HashSet<HashTagModel>(); //TODO: is null
        public HashSet<HashTagModel> ExcludedHashTags { get; set; } = new HashSet<HashTagModel>();
        public bool IsMerged => MergedHashTags.Any();

        public HashTagModel()
        { }
  
        public HashTagModel(string tag,HashSet<ArtefactModel> searchResults, int numOfOccurences = 1) : base(tag)
        {
            NumOfOccurences = numOfOccurences;
            SearchResults = searchResults; 
        }

        public int TotalNumOfOccurences()
        {
            return NumOfOccurences + MergedHashTags.Sum(x => x.TotalNumOfOccurences());
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