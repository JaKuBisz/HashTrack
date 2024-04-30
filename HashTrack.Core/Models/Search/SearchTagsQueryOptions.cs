using System;
using System.Collections.Generic;
using HashTrack.Core.Enums;

namespace HashTrack.Core.Models.Search
{
    public class SearchTagsQueryOptions
    {
        public string Tag { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public ArtifactTypes Artefacts { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string EventTag { get; set; } = Events.DefaultSearchCompleted;
        public bool ExactMatch { get; set; } = false;
        public bool UseCustomProperty { get; set; } = false;
    }
}