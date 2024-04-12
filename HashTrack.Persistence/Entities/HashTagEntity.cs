using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HashTrack.Persistence.Entities
{
    public class HashTagEntity : BaseEntity
    {
        [Required]
        [StringLength(255)]
        public string Tag { get; set; }
        
        [Required]
        public int NumOfOccurrences { get; set; }
        public virtual ICollection<HashTagEntity> MergedHashTags { get; set; }
        public virtual ICollection<HashTagEntity> ExcludedHashTags { get; set; }
        public virtual ICollection<ArtefactEntity> Items { get; set; }
        
        public HashTagEntity()
        {
            MergedHashTags = new HashSet<HashTagEntity>();
            ExcludedHashTags = new HashSet<HashTagEntity>();
            Items = new HashSet<ArtefactEntity>();
        }
    }
}