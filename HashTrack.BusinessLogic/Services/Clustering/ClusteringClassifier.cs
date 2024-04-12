using System.Linq;
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
        
        public bool Classify(string text1, string text2)
        {
            IStringMetric metric = new Levenstein();
            var tag1 = SplitTag(text1);
            var tag2 = SplitTag(text2);

            double textualSimilarity = metric.GetSimilarity(tag1.TextPart, tag2.TextPart);
            
            bool numbersEqual = tag1.NumberPart == tag2.NumberPart;

            return numbersEqual && textualSimilarity > SimilarityThreshold;
        }
        
        public bool Classify(HashTagDto tag1, HashTagDto tag2)
        {
            if (tag1.NumOfOccurences <= tag2.NumOfOccurences)
            {
                return false;
            }
            
            return Classify(tag1.Id, tag2.Id);
        }
        
        private static (string TextPart, string NumberPart) SplitTag(string tag)
        {
            var textPart = Regex.Replace(tag, @"\d", "");
            var numberPart = new string(tag.Where(char.IsDigit).ToArray());
            return (textPart, numberPart);
        }
    }
}