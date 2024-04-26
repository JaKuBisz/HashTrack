namespace HashTrack.Core
{
    public static class Events
    {
        public const string DefaultSearchInitiated = "DefaultSearchInitiated";
        public const string DefaultSearchCompleted = "DefaultSearchCompleted";
        public const string DefaultSearchProcessed = "DefaultSearchProcessed";
        public const string IndexingSearchCompleted = "IndexingSearchCompleted";
        public const string HashTagsUpdated = "HashTagsUpdated";
        public const string CategoryManagerSearch = "CategoryManagerSearch";
        
        public static class UI
        {
            public const string ChangeSelectedTab = "ChangeSelectedTab";
            public const string MergeTags = "MergeTags";
        }
    }
}