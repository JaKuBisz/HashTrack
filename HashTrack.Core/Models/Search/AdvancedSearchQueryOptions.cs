using HashTrack.Core.Enums;
using System;

namespace HashTrack.Core.Models.Search
{
    public class AdvancedSearchQueryOptions
    {
        public string Keyword { get; set; }
        public ArtifactTypes Artefacts { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string Tag { get; set; } = Events.DefaultSearchCompleted;
        public bool ExactMatch { get; set; } = false;

    }
}
