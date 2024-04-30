using HashTrack.Core.Models.Search;

namespace HashTrack.Core.Interfaces
{
    public interface ICategoryManagerService
    {
        void AssignHashTagItems(HashTagModel hashTagModel);
        void AddItemToCategory(HashTagModel hashTagModel, object item);
    }
}