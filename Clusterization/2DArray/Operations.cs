using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Clusterization._2DArray
{
    public class Operations
    {

        public Matrix RandomMatrix(int rows, int columns, (double, double) range)
        {
            Matrix result = new Matrix(rows, columns);
            Random random = new Random();

            for (int i = 0; i < result.Size; i++)
            {
                result[i] = random.NextDouble() * (range.Item2 - range.Item1) + range.Item1;
            }

            return result;
        }

        public Matrix Dot(Matrix a, Matrix b)
        {
            if (a.Columns != b.Rows)
            {
                throw new Exception("a->Cols must be equal to b->Rows");
            }

            int m = a.Rows;
            int q = b.Columns;
            int n = a.Columns;
            Matrix result = new Matrix(m, q);

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < q; j++)
                {
                    result[i, j] = 0;
                    for (int k = 0; k < n; k++)
                    {
                        result[i, j] += a[i, k] * b[k, j];
                    }
                }
            }

            return result;
        }

        public Matrix Exp(Matrix x)
        {
            Matrix result = new Matrix(x.Rows, x.Columns);

            for (int i = 0; i < x.Size; i++)
            {
                result[i] = Math.Exp(x[i]);
            }

            return result;
        }

        public Matrix Log(Matrix x)
        {
            Matrix result = new Matrix(x.Rows, x.Columns);

            for (int i = 0; i < x.Size; i++)
            {
                result[i] = Math.Log(x[i]);
            }

            return result;
        }

        public Matrix Sqrt(Matrix x)
        {
            Matrix result = new Matrix(x.Rows, x.Columns);

            for (int i = 0; i < x.Size; i++)
            {
                result[i] = Math.Sqrt(x[i]);
            }

            return result;
        }

        public Matrix Square(Matrix x)
        {
            Matrix result = new Matrix(x.Rows, x.Columns);

            for (int i = 0; i < x.Size; i++)
            {
                result[i] = Math.Pow(x[i], 2);
            }

            return result;
        }

        public Matrix Transpose(Matrix x)
        {
            Matrix result = new Matrix(x.Columns, x.Rows);

            for (int i = 0; i < x.Columns; i++)
            {
                for (int j = 0; j < x.Rows; j++)
                {
                    result[i, j] = x[j, i];
                }
            }

            return result;
        }

        public Matrix ReduceSumByRows(Matrix x)
        {
            Matrix result = new Matrix(1, x.Columns);

            for (int i = 0; i < x.Columns; i++)
            {
                for (int j = 0; j < x.Rows; j++)
                {
                    result[0, i] += x[j, i];
                }
            }

            return result;
        }

        public Matrix ReduceSumByColumns(Matrix x)
        {
            Matrix result = new Matrix(1, x.Rows);

            for (int i = 0; i < x.Rows; i++)
            {
                for (int j = 0; j < x.Columns; j++)
                {
                    result[0, i] += x[i, j];
                }
            }

            return result;
        }

        public Matrix CoordinateMatrix(int rows, int columns)
        {
            Matrix result = new Matrix(rows * columns, 2);

            for (int i = 0; i < result.Rows; i++)
            {
                result[i, 0] = i % rows;
                result[i, 1] = i / rows;
            }

            return result;
        }

        public Matrix ReLU(Matrix matrix)
        {
            var result = new Matrix(matrix.Rows, matrix.Columns);

            for (int i = 0; i < matrix.Size; i++)
            {
                if (matrix[i] < 0)
                    result[i] = 0;
                else if (matrix[i] > 1)
                    result[i] = 1;
                else
                    result[i] = matrix[i];
            }

            return result;
        }

        public SolidColorBrush[] GetRGBFromMatrix(Matrix matrix)
        {
            var result = new SolidColorBrush[matrix.Rows];
            var normalizedMatrix = ReLU(matrix);

            for (int i = 0; i < matrix.Rows; i++)
            {
                byte r = Convert.ToByte(normalizedMatrix[i, 0] * 255);
                byte g = Convert.ToByte(normalizedMatrix[i, 1] * 255);
                byte b = Convert.ToByte(normalizedMatrix[i, 2] * 255);

                result[i] = new SolidColorBrush(Color.FromRgb(r, g, b));
            }

            return result;
        }

        public int FindMinDistanceIndex(Matrix weights, Matrix vector)
        {
            List<double> distances = new List<double>();

            for (int i = 0; i < weights.Rows; i++)
            {
                distances.Add(FindDistance(weights.GetRow(i), vector));
            }

            return distances.IndexOf(distances.Min());
        }

        double FindDistance(Matrix vector1, Matrix vector2)
        {
            double euclideanDistance = 0;
            for (var i = 0; i < vector1.Size; i++)
            {
                euclideanDistance += (vector1[i] - vector2[i]) * (vector1[i] - vector2[i]);
            }

            return Math.Sqrt(euclideanDistance);
        }

        public void NaNCheck(Matrix matrix, int iter)
        {
            for (int i = 0; i < matrix.Rows; i++)
            {
                for (int j = 0; j < matrix.Columns; j++)
                {
                    if (Double.IsNaN(matrix[i, j]))
                    {

                    }
                }
            }
        }
    }
}
