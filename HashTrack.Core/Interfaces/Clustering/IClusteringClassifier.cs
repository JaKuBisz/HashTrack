using HashTrack.Core.Models.Search;

namespace HashTrack.Core.Interfaces.Clustering
{
    public interface IClusteringClassifier
    {
        bool Classify(string text1, string text2);
        bool Classify(HashTagModel tag1, HashTagModel tag2);
    }
}