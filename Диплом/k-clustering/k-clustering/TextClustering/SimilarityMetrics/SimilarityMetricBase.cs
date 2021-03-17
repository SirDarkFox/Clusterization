using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k_clustering.TextClustering.SimilarityMetrics
{
    public abstract class SimilarityMetricBase
    {
        public abstract double FindDistance(double[] vecA, double[] vecB);

        public double DotProduct(double[] vecA, double[] vecB)
        {
            double dotProduct = 0;
            for (var i = 0; i < vecA.Length; i++)
            {
                dotProduct += (vecA[i] * vecB[i]);
            }

            return dotProduct;
        }

        public double Magnitude(double[] vector)
        {
            return Math.Sqrt(DotProduct(vector, vector));
        }

        public double NaNCheck(double result)
        {
            if (double.IsNaN(result))
                return 0;
            else
                return result;
        }
    }
}
