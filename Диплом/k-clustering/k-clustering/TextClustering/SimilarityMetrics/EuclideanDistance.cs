using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k_clustering.TextClustering.SimilarityMetrics
{
    public class EuclideanDistance : SimilarityMetricBase
    {
        public override double FindDistance(double[] vecA, double[] vecB)
        {
            double euclideanDistance = 0;
            for (var i = 0; i < vecA.Length; i++)
            {
                euclideanDistance += (vecA[i] - vecB[i]) * (vecA[i] - vecB[i]);
            }

            return Math.Sqrt(euclideanDistance);
        }
    }
}
