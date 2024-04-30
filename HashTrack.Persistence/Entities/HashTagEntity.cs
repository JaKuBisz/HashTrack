using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace HashTrack.Persistence.Entities
{
    public class HashTagEntity
    {
        public HashTagEntity()
        {
            MergedHashTags = new HashSet<string>();
            ExcludedHashTags = new HashSet<string>();
        }

        [Key]
        [Required]
        [StringLength(512)]
        public string Tag { get; set; }

        [Required]
        public int NumOfOccurrences { get; set; }

        public string ArtefactIdsJson { get; set; }
        public DateTime LastUpdated { get; set; }
        public string MergedHashTagIdsJson { get; set; }
        public string ExcludedHashTagIdsJson { get; set; }

        public bool CreateFolder { get; set; }
        public bool CreateCategory { get; set; }

        [StringLength(512)]
        public string FolderName { get; set; }

        [StringLength(512)]
        public string CategoryName { get; set; }

        public int CategoryColor { get; set; }

        [NotMapped]
        public HashSet<Guid> ArtefactIds
        {
            get => string.IsNullOrEmpty(ArtefactIdsJson)
                ? new HashSet<Guid>()
                : JsonConvert.DeserializeObject<HashSet<Guid>>(ArtefactIdsJson);
            set => ArtefactIdsJson = JsonConvert.SerializeObject(value);
        }

        [NotMapped]
        public HashSet<string> MergedHashTags
        {
            get => MergedHashTagIdsJson == null
                ? new HashSet<string>()
                : JsonConvert.DeserializeObject<HashSet<string>>(MergedHashTagIdsJson);
            set => MergedHashTagIdsJson = JsonConvert.SerializeObject(value);
        }

        [NotMapped]
        public HashSet<string> ExcludedHashTags
        {
            get => ExcludedHashTagIdsJson == null
                ? new HashSet<string>()
                : JsonConvert.DeserializeObject<HashSet<string>>(ExcludedHashTagIdsJson);
            set => ExcludedHashTagIdsJson = JsonConvert.SerializeObject(value);
        }
    }
}