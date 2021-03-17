using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k_clustering.k_means.Objects
{
    public class Cluster
    {
        public double[] Centroid { get; set; }
        public List<DocumentVector> DocumentGroup { get; set; }
    }
}
