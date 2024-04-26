using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HashTrack.Core.Enums;

namespace HashTrack.Persistence.Entities
{
    public class HashTagEntity //: BaseEntity
    {//TODO: Fix collections either add reference to self or make a table for the strings
        // remember i count on it being referenced in HashTagModel in case of change of one hashtag i dont have to propage to all other hashtags
        //maybe its not acctualy a proble as in indexing i load all the hashtags and then edit them and then add references to themselves save at once before; but again i should load them from db fist to not load all and we are back to the same problem
        //maybe load all hashtags every time in indexing???
        [Key]
        [Required]
        [StringLength(512)]
        public string Tag { get; set; }

        [Required]
        public int NumOfOccurrences { get; set; }

        public DateTime LastUpdated { get; set; } 
        public virtual ICollection<Guid> ArtifactsIds { get; set; } = new HashSet<Guid>();
        public virtual ICollection<HashTagEntity> MergedHashTags { get; set; }
        public virtual ICollection<HashTagEntity> ExcludedHashTags { get; set; }
        public bool CreateFolder { get; set; }
        public bool CreateCategory { get; set; }
        public string FolderName { get; set; }
        public string CategoryName { get; set; }
        public int CategoryColor { get; set; } = 0;

        public HashTagEntity()
        {
            MergedHashTags = new HashSet<HashTagEntity>();
            ExcludedHashTags = new HashSet<HashTagEntity>();
        }
    }
}