using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k_clustering.TextClustering.SimilarityMetrics
{
    public class CosineSimilarity : SimilarityMetricBase
    {
        public override double FindDistance(double[] vecA, double[] vecB)
        {
            var dotProduct = DotProduct(vecA, vecB);
            var magnitudeA = Magnitude(vecA);
            var magnitudeB = Magnitude(vecB);
            double result = dotProduct / (magnitudeA * magnitudeB);

            return NaNCheck(result);
        }
    }
}
