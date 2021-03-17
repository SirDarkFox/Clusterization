using k_clustering.k_means;
using k_clustering.k_means.Objects;
using k_clustering.TextClustering;
using k_clustering.TextClustering.SimilarityMetrics;
using k_clustering.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace k_clustering
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DocumentVectorModel documentVectorModel;
        K_means k_Means;

        public MainWindow()
        {
            InitializeComponent();

            ClustersCount.Text = "2";

            documentVectorModel = new DocumentVectorModel(false);
            k_Means = new K_means(new CosineSimilarity());
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            List<Cluster> clusterCollection = new List<Cluster>();

            if (ClustersCount.Text == string.Empty || Convert.ToInt32(ClustersCount.Text) <= 1)
            {
                clusterCollection = k_Means.XCluster(documentVectorModel.DocumentVectorCollection);
            }
            else
            {
                clusterCollection = k_Means.Cluster(documentVectorModel.DocumentVectorCollection, Convert.ToInt32(ClustersCount.Text));
            }

            string str = "";

            for (int i = 0; i < clusterCollection.Count; i++)
            {
                str += $"------------------------------[ CLUSTER {i+1} ]-----------------------------\n";
                foreach (DocumentVector document in clusterCollection[i].DocumentGroup)
                {
                    str += document.Name + "\n";
                }
                str += "--------------------------------------------------------------------------\n";
            }

            Results.Text = str;
            ClustersCount.Text = Convert.ToString(clusterCollection.Count);
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            Clear();

            if (!File.Exists("Save.json"))
            {
                string[] files = Directory.GetFiles("TestData");
                List<string> textCollection = new List<string>();

                for (int i = 0; i < files.Length; i++)
                {
                    textCollection.Add(File.ReadAllText(files[i], Encoding.Default));
                }

                documentVectorModel.CreateDVCollection(textCollection, files);

                string ser = JsonConvert.SerializeObject(documentVectorModel);
                File.WriteAllText("Save.json", ser);
            }

            documentVectorModel = JsonConvert.DeserializeObject<DocumentVectorModel>(File.ReadAllText("Save.json"));
            DocsCount.Text = Convert.ToString(documentVectorModel.DocumentVectorCollection.Count());
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            Clear();

            if (File.Exists("Save.json"))
            {
                File.Delete("Save.json");
            }
        }

        void Clear()
        {
            Results.Text = "";
            documentVectorModel.DocumentVectorCollection?.Clear();
            documentVectorModel.Vocabulary?.Clear();
        }
    }
}
