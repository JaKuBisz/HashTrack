using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HashTrack.Core.Interfaces.Clustering;
using HashTrack.Core.Models.Search;
using SimMetrics.Net.API;
using SimMetrics.Net.Metric;

namespace HashTrack.Clustering.Services
{
    public class ClusteringClassifier : IClusteringClassifier
    {
        private const double SimilarityThreshold = 0.75;
        private readonly IStringMetric _metric = new Levenstein();
        
        public bool Classify(string text1, string text2)
        {
            var tag1 = SplitTag(text1);
            var tag2 = SplitTag(text2);

            if (tag1.NumberPart != tag2.NumberPart)
            {
                return false;
            }

            double textualSimilarity = _metric.GetSimilarity(tag1.TextPart, tag2.TextPart);

            return textualSimilarity > SimilarityThreshold;
        }
        
        public bool Classify(HashTagModel tag1, HashTagModel tag2)
        {
            if (tag1.NumOfOccurences <= tag2.NumOfOccurences)
            {
                return false;
            }
            
            return Classify(tag1.Tag, tag2.Tag);
        }
        
        private static (string TextPart, string NumberPart) SplitTag(string tag)
        {
            var textPartBuilder = new StringBuilder();
            var numberPartBuilder = new StringBuilder();

            foreach (char ch in tag)
            {
                if (char.IsDigit(ch))
                    numberPartBuilder.Append(ch);
                else
                    textPartBuilder.Append(ch);
            }

            return (textPartBuilder.ToString(), numberPartBuilder.ToString());
        }
    }
}