using System;

namespace HashTrack.Core.Models.Search
{
    public class SearchResultViewItem
    {
        public string Title { get; set; }
        public string Sender { get; set; }
        //public string Description { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public object OriginalItem { get; set; }
    }
}
