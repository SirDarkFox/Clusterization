using k_clustering.k_means.Objects;
using k_clustering.TextClustering.SimilarityMetrics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k_clustering.TextClustering
{
    public class K_means
    {
        const int max_k = 4;

        private int iterationCount;
        private int vocabularyCount;

        public SimilarityMetricBase SimilarityMetric { get; set; }

        public K_means(SimilarityMetricBase similarityMetric)
        {
            SimilarityMetric = similarityMetric;
        }

        public List<Cluster> XCluster(List<DocumentVector> DVCollection)
        {
            List<List<Cluster>> kResults = new List<List<Cluster>>();

            for (int k = 2; k <= max_k; k++)
            {
                kResults.Add(Cluster(DVCollection, k));
            }

            List<List<double>> Silhouette = new List<List<double>>();

            foreach (var kResult in kResults)
            {
                Silhouette.Add(new List<double>());
                double a = 0;
                double b = 0;

                foreach (var currentDocument in DVCollection)
                {
                    Cluster currentCluster = kResult.Find(x => x.DocumentGroup.Contains(currentDocument));
                    List<Cluster> otherClusters = kResult.Where(x => x != currentCluster).ToList();

                    List<double> allB = new List<double>();

                    foreach (var document in currentCluster.DocumentGroup)
                    {
                        if (currentDocument == document)
                            continue;

                        a += SimilarityMetric.FindDistance(currentDocument.Vector, document.Vector);
                    }

                    a /= currentCluster.DocumentGroup.Count;

                    foreach (var otherCluster in otherClusters)
                    {
                        double currentB = 0;

                        foreach (var otherDocument in otherCluster.DocumentGroup)
                        {
                            currentB += SimilarityMetric.FindDistance(currentDocument.Vector, otherDocument.Vector);
                        }

                        currentB /= otherCluster.DocumentGroup.Count;
                        allB.Add(currentB);
                    }

                    if (SimilarityMetric is CosineSimilarity)
                    {
                        b = allB.Max();
                        Silhouette.Last().Add((b - a) / Math.Min(a, b));
                    }
                    else
                    {
                        b = allB.Min();
                        Silhouette.Last().Add((b - a) / Math.Max(a, b));
                    }
                }
            }

            var temp = Silhouette.Select(x => x.Average()).ToList();
            int bestKResult = 0;

            if (SimilarityMetric is CosineSimilarity)
                bestKResult = temp.IndexOf(temp.Min());
            else
                bestKResult = temp.IndexOf(temp.Max());

            return kResults[bestKResult];
        }

        public List<Cluster> Cluster(List<DocumentVector> DVCollection, int k)
        {
            iterationCount = 0;
            vocabularyCount = DVCollection[0].Vector.Length;

            if (k > DVCollection.Count)
                k = DVCollection.Count;


            //Create Seed

            List<Cluster> clusterCollection = new List<Cluster>();
            List<Cluster> prevClusterCollection;

            HashSet<int> seed = CreateSeed(DVCollection, k);

            foreach (int index in seed)
            {
                Cluster cluster = new Cluster
                {
                    Centroid = DVCollection[index].Vector,
                    DocumentGroup = new List<DocumentVector>()
                };

                clusterCollection.Add(cluster);
            }

            //Create Seed

            do
            {
                prevClusterCollection = clusterCollection;

                AttachDocuments(ref clusterCollection, DVCollection);
                UpdateMeanPoints(ref clusterCollection);

                iterationCount++;

            } while (!StopCheck(prevClusterCollection, clusterCollection));

            return clusterCollection;
        }

        HashSet<int> CreateSeed(List<DocumentVector> DVCollection, int k)
        {
            Random r = new Random();
            HashSet<int> result = new HashSet<int>();
            List<List<double>> distMatrix = new List<List<double>>();

            result.Add(r.Next(DVCollection.Count));

            for (int i = 0; i < k - 1; i++)
            {
                distMatrix.Add(new List<double>());

                List<double> distResult = new List<double>();

                for (int j = 0; j < DVCollection.Count; j++)
                {
                    distMatrix[i].Add(SimilarityMetric.FindDistance(DVCollection[result.Last()].Vector, DVCollection[j].Vector));
                    double sum = 0;

                    for (int s = 0; s < result.Count; s++)
                    {
                        if (result.Contains(j))
                        {
                            if (SimilarityMetric is CosineSimilarity)
                                sum = double.PositiveInfinity;
                            else
                                sum = double.NegativeInfinity;
                        }
                        else
                            sum += distMatrix[s][j];
                    }

                    distResult.Add(sum / result.Count);
                }

                if (SimilarityMetric is CosineSimilarity)
                    result.Add(distResult.IndexOf(distResult.Min()));
                else
                    result.Add(distResult.IndexOf(distResult.Max()));
            }

            return result;
        }

        void AttachDocuments(ref List<Cluster> clusterCollection, List<DocumentVector> DVCollection)
        {
            foreach (var cluster in clusterCollection)
            {
                cluster.DocumentGroup = new List<DocumentVector>();
            }

            foreach (var document in DVCollection)
            {
                List<double> similarityMeasure = new List<double>();

                foreach (var cluster in clusterCollection)
                {
                    similarityMeasure.Add(SimilarityMetric.FindDistance(document.Vector, cluster.Centroid));
                }

                if(SimilarityMetric is CosineSimilarity)
                    clusterCollection[similarityMeasure.IndexOf(similarityMeasure.Max())].DocumentGroup.Add(document);
                else
                    clusterCollection[similarityMeasure.IndexOf(similarityMeasure.Min())].DocumentGroup.Add(document);
            }
        }

        void UpdateMeanPoints(ref List<Cluster> clusterCollection)
        {
            foreach (var cluster in clusterCollection)
            {
                cluster.Centroid = new double[vocabularyCount];
            }

            for (int i = 0; i < clusterCollection.Count(); i++)
            {
                for (int j = 0; j < clusterCollection[i].Centroid.Count(); j++)
                {
                    double sum = 0;

                    foreach (DocumentVector document in clusterCollection[i].DocumentGroup)
                    {
                        sum += document.Vector[j];
                    }

                    clusterCollection[i].Centroid[j] = sum / clusterCollection[i].DocumentGroup.Count();
                }
            }
        }

        bool StopCheck(List<Cluster> prevClusterCollection, List<Cluster> newClusterCollection)
        {
            if (iterationCount >= 11000)
                return true;

            for (int i = 0; i < prevClusterCollection.Count; i++)
            {
                if (prevClusterCollection[i].Centroid != newClusterCollection[i].Centroid)
                    return true;
            }

            return false;
        }
    }
}
