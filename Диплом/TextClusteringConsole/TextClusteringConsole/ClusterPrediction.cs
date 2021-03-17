using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace TextClusteringConsole
{
    public class ClusterPrediction
    {
        [ColumnName("Title")] public string Title; 
        [ColumnName("PredictedLabel")] public uint PredictedLabel;
        [ColumnName("Score")] public float[] Score;
    }
}
