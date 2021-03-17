using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k_clustering.k_means.Objects
{
    public class DocumentVector
    {
        public string Name { get; set; }
        public List<string> Content { get; set; }
        public double[] Vector { get; set; }
    }
}
