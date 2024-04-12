using System.Collections.Generic;

namespace HashTrack.Persistence.Entities
{
    public class ArtefactEntity : BaseEntity
    {
        public string EmailSubject { get; set; }
        // Other properties...
        public virtual ICollection<HashTagEntity> HashTags { get; set; }
    }
}