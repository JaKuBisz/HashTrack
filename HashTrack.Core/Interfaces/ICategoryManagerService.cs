using HashTrack.Core.Models.Search;

namespace HashTrack.Core.Interfaces
{
    public interface ICategoryManagerService
    {
        void AssignHashTagItems(HashTagModel hashTagModel);
        void AddArtefactToCategory(HashTagModel hashTagModel, object artefact);
    }
}