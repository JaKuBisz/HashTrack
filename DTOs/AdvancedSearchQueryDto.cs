using HashTrack.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTrack.DTOs
{
    public class AdvancedSearchQueryDto
    {
        public string Keyword { get; set; }
        public ArtifactTypes Artefacts { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

    }
}
