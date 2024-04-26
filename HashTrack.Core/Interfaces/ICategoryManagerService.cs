using HashTrack.Core.Enums;
using HashTrack.Core.Models.Search;

namespace HashTrack.Core.Interfaces
{
    public interface ICategoryManagerService
    {
        void AssignHashTagItems(HashTagModel hashTagModel);
        void AddItemToCategory(string categoryName, CategoryColor categoryColor, object item);
    }
}