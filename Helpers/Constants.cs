using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTrack.Helpers
{
    internal static class Constants
    {
        public const string DefaultSearchTag = "DefaultSearchTag";
        public const string IndexingSearchTag = "IndexingSearchTag";

        public class DaslFilter
        {
            public const string HttpMailSchema = "urn:schemas:httpmail";
            public static string ExactMatch(string keyword) => $"ci_phrasematch '{keyword}'";
            public static string SubString(string keyword) => $"like '%{keyword}%'";
            public const string And = " AND ";
            public const string Or = " OR ";
            
            public class HttpMail
            {
                public const string Body = HttpMailSchema+":textdescription";
                public const string Date = HttpMailSchema+":date";
            }
        }
    }
}
