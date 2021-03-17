using Clusterization._2DArray;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Clusterization
{
    public class SOMNetwork
    {
        Operations Operations;

        public int InputNodes;
        public int Length;

        public double DefaultSigma;
        public double Sigma;
        public double MinSigma;
        public double DefaultLearningRate;
        public double LearningRate;
        public double MinLearningRate;
        public double Tay1;
        public double Tay2;

        public Matrix Coordinates;

        public Matrix Weights { get; set; }
        public int Iteration { get; set; }

        public SOMNetwork(int inputNodes, int length, double sigma = -1, double learningRate = 0.1, double tay2 = 1000)
        {
            Operations = new Operations();

            InputNodes = inputNodes;
            Length = length;

            DefaultSigma = Sigma = (sigma < 0) ? (double)length / 2 : sigma;
            MinSigma = DefaultSigma * Math.Exp(-1000 / (1000 / Math.Log(DefaultSigma)));

            DefaultLearningRate = LearningRate = learningRate;
            MinLearningRate = 0.01;

            Tay1 = 1000 / Math.Log(DefaultSigma);
            Tay2 = tay2;

            Weights = Operations.RandomMatrix(length * length, inputNodes, (-1, 1));
            Coordinates = Operations.CoordinateMatrix(length, length);
        }

        public int Competition(Matrix input)
        {
            Matrix distance = Operations.Sqrt(Operations.ReduceSumByColumns(Operations.Square(input - Weights)));

            return distance.MinIndex().Item2;
        }

        public void TrainingAsync()
        {
                Matrix input = Operations.RandomMatrix(1, 3, (0, 1));

                int winner1DPosition = Competition(input);

                Matrix winner2DPosition = new Matrix(1, 2);
                winner2DPosition[0] = winner1DPosition % Length;
                winner2DPosition[1] = winner1DPosition / Length;

                Matrix d = Operations.Sqrt(Operations.ReduceSumByColumns(Operations.Square(Coordinates - winner2DPosition)));

                Sigma = (Iteration > 1000) ? MinSigma : DefaultSigma * Math.Exp(-Iteration / Tay1);

                Matrix h = Operations.Exp(-Operations.Square(d) / (2 * Math.Pow(Sigma, 2)));

                LearningRate = DefaultLearningRate * Math.Exp(-Iteration / Tay2);
                LearningRate = (LearningRate < MinLearningRate) ? MinLearningRate : LearningRate;

                Matrix delta = Operations.Transpose(LearningRate * h * Operations.Transpose(input - Weights));
                Weights += delta;
                Iteration++;
        }
    }
}
