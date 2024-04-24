using HashTrack.Attributes;

namespace HashTrack.Enums
{
    public enum OrderByHashTagsOverview
    {
        [DisplayName("Most recent")]
        DateDesc,
        [DisplayName("Most used")]
        OccurrencesDesc,
        [DisplayName("Least used")]
        OccurrencesAsc
    }
}