using HashTrack.Core;
using HashTrack.Core.Models.Search;

namespace HashTrack
{
    public class ChangeTabEvent
    {
        public const string Tag = Events.UI.ChangeSelectedTab;

        public ChangeTabEvent(ChangeTabEventTarget target, HashTagModel tagModel)
        {
            Target = target;
            TagModel = tagModel;
        }

        public ChangeTabEventTarget Target { get; set; }
        public HashTagModel TagModel { get; set; }
    }

    public enum ChangeTabEventTarget
    {
        SearchTab = 0,
        TagOverviewTab = 1,
        TagDetailsTab = 2
    }
}