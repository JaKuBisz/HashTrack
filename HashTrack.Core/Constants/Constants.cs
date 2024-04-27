namespace HashTrack.Core
{
    public static class Constants
    {
        
        public class DaslFilter
        {
            public const string HttpMailSchema = "urn:schemas:httpmail";
            public static string ExactMatch(string keyword) => $"ci_phrasematch '{keyword}'";
            public static string StartsWith(string keyword) => $"ci_startswith '{keyword}'";
            public static string SubString(string keyword) => $"like '%{keyword}%'";
            public const string And = " AND ";
            public const string Or = " OR ";
            
            public class HttpMail
            {
                public const string Body = HttpMailSchema+":textdescription";
                public const string Date = HttpMailSchema+":date";
            }
        }

        public class Storage
        {
            public const string Artefacts = "Artefacts";
            public const string LastSearch = "LastSearch";
            public const string IndexedHashTags = "IndexedHashTags";
            public const string ClusteredHashTags = "ClusteredHashTags";
            public const string HashtagClusteringSettings = "HashtagClusteringSettings";
        }
        
        public class CustomProperties
        {
            public const string artefactID = "hashTrackArtefactID";
            public const string Tags = "hashTrackTags";
        }
    }
}
