using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTrack.Exception
{
    public class SearchQueryException : System.Exception
    {
        public SearchQueryException(string message) : base(message)
        { }
    }
}
