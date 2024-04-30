namespace HashTrack.Core
{
    public static class Constants
    {
        public class DaslFilter
        {
            public const string HttpMailSchema = "urn:schemas:httpmail";
            public const string And = " AND ";
            public const string Or = " OR ";

            public static string ExactMatch(string keyword)
            {
                return $"ci_phrasematch '{keyword}'";
            }

            public static string StartsWith(string keyword)
            {
                return $"ci_startswith '{keyword}'";
            }

            public static string SubString(string keyword)
            {
                return $"like '%{keyword}%'";
            }

            public static string Equals(string keyword)
            {
                return $"= '{keyword}'";
                // For custom properties
            }

            public class HttpMail
            {
                public const string Body = HttpMailSchema + ":textdescription";
                public const string Date = HttpMailSchema + ":date";
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