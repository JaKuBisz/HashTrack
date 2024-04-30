using System.Text;
using HashTrack.Core.Attributes;
using HashTrack.Core.Enums;
using HashTrack.Core.Interfaces.Clustering;
using HashTrack.Core.Models.Search;
using SimMetrics.Net.API;
using SimMetrics.Net.Metric;

namespace HashTrack.Clustering.Services
{
    [RegisterService(LifeCycle.Transient, typeof(IClusteringClassifier))]
    public class ClusteringClassifier : IClusteringClassifier
    {
        private const double SimilarityThreshold = 0.75;
        private readonly IStringMetric _metric = new Levenstein();

        public bool Classify(string text1, string text2)
        {
            var tag1 = SplitTag(text1);
            var tag2 = SplitTag(text2);

            if (tag1.NumberPart != tag2.NumberPart) return false;

            var textualSimilarity = _metric.GetSimilarity(tag1.TextPart, tag2.TextPart);

            return textualSimilarity > SimilarityThreshold;
        }

        public bool Classify(HashTagModel tag1, HashTagModel tag2)
        {
            if (tag1.NumOfOccurrences <= tag2.NumOfOccurrences) return false;

            return Classify(tag1.Id, tag2.Id);
        }

        private static (string TextPart, string NumberPart) SplitTag(string tag)
        {
            var textPartBuilder = new StringBuilder();
            var numberPartBuilder = new StringBuilder();

            foreach (var ch in tag)
                if (char.IsDigit(ch))
                    numberPartBuilder.Append(ch);
                else
                    textPartBuilder.Append(ch);

            return (textPartBuilder.ToString(), numberPartBuilder.ToString());
        }
    }
}