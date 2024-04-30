using System;

namespace HashTrack.Core.Models.Search
{
    public class ArtefactModel : UniqueId<Guid>
    {
        public string Title { get; set; }

        public string Sender { get; set; }

        //public string Description { get; set; }
        public string Type { get; set; }

        public DateTime Date { get; set; }

        //TODO: Store just the EntityId (possibly + StoreID)
        public object OriginalItem { get; set; }
    }
}