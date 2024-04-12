using HashTrack.Core.Models.Search;

namespace HashTrack.Core.Interfaces.Clustering
{
    public interface IClusteringClassifier
    {
        bool Classify(string text1, string text2);
        bool Classify(HashTagDto tag1, HashTagDto tag2);
    }
}