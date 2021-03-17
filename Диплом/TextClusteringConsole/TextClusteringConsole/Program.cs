using Microsoft.ML;
using Microsoft.ML.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace TextClusteringConsole
{
    class Program
    {

        private static MLContext mlContext = new MLContext();
        private static Regex r = new Regex("([ \\t{}()\",:;. \n])");
        private static bool porter = false;
        private static int maxTextLength = 3000;

        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            string[] files = Directory.GetFiles("TestData");

            List<Book> bookCollection = new List<Book>();

            for (int i = 0; i < files.Length; i++)
            {
                bookCollection.Add(new Book 
                {
                    Title = files[i],
                    Text = Regex.Replace(File.ReadAllText(files[i], Encoding.GetEncoding(1251)).ToLower(), @"[^ а-я]", string.Empty)
                });
            }

            var dataView = mlContext.Data.LoadFromEnumerable(bookCollection);

            var transformedData = mlContext.Transforms.Text.FeaturizeText("Features", "Text").Fit(dataView).Transform(dataView);

            var pipeline = mlContext.Clustering.Trainers.KMeans(numberOfClusters: 2);

            var model = pipeline.Fit(transformedData);

            var clusterPredictions = mlContext.Data.CreateEnumerable<ClusterPrediction>(model.Transform(transformedData), false).ToList();

            clusterPredictions.Sort((x, y) => x.PredictedLabel.CompareTo(y.PredictedLabel));

            foreach (var prediction in clusterPredictions)
            {
                Console.WriteLine("Название  => " + prediction.Title);
                Console.WriteLine("Кластер   => " + prediction.PredictedLabel);
                Console.WriteLine("Дистанция => " + String.Join(';', prediction.Score));
                Console.WriteLine();
            }
        }
    }
}