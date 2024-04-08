using HashTrack.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace HashTrack.DTOs
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
