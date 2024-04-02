using HashTrack.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HashTrack.Helpers;

namespace HashTrack.DTOs
{
    public class AdvancedSearchQueryOptions
    {
        public string Keyword { get; set; }
        public ArtifactTypes Artefacts { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string Tag { get; set; } = Constants.DefaultSearchTag;
        public bool ExactMatch { get; set; } = false;

    }
}
