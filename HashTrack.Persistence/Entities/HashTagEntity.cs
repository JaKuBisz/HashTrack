using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HashTrack.Persistence.Entities
{
    public class HashTagEntity //: BaseEntity
    {
        [Key]
        [Required]
        [StringLength(512)]
        public string Tag { get; set; }

        [Required]
        public int NumOfOccurrences { get; set; }

        public DateTime LastUpdated { get; set; } 
        public virtual ICollection<string> MergedHashTags { get; set; }
        public virtual ICollection<string> ExcludedHashTags { get; set; }

        public HashTagEntity()
        {
            MergedHashTags = new HashSet<string>();
            ExcludedHashTags = new HashSet<string>();
        }
    }
}