using HashTrack.Core.Enums;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HashTrack.Core.Models.Search
{
    public class SearchQueryOptions
    {
        public string Tag { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public ArtifactTypes Artefacts { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string EventTag { get; set; } = Events.DefaultSearchCompleted;
        public bool ExactMatch { get; set; } = false;
        public bool QuickSearch { get; set; } = false;
    }
}
