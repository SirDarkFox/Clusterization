using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clusterization._2DArray
{
    public class Matrix
    {
        [JsonProperty]
        private double[,] _data;

        public int Rows => _data.GetLength(0);

        public int Columns => _data.GetLength(1);

        public int Size => _data.GetLength(0) * _data.GetLength(1);

        [JsonConstructor]
        public Matrix(int rows, int columns)
        {
            _data = new double[rows, columns];
        }

        public Matrix(params double[] values)
        {
            _data = new double[1, values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                this[i] = values[i];
            }
        }

        public double this[int i, int j]
        {
            get => _data[i, j];
            set => _data[i, j] = value;
        }

        public double this[int i]
        {
            get => _data[i % Rows, i / Rows];
            set => _data[i % Rows, i / Rows] = value;
        }

        public void Load(double[] data)
        {
            int k = 0;
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++, k++)
                {
                    _data[i, j] = data[k];
                }
            }
        }

        public void Fill(double value)
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    _data[i, j] = value;
                }
            }
        }

        public void ArrangeTo(int rows, int columns)
        {
            double[,] result = new double[rows, columns];

            int iOld = 0;
            int jOld = 0;

            for (int i = 0; i < rows; i++, iOld++)
            {
                if (iOld >= Rows)
                    iOld = 0;

                for (int j = 0; j < columns; j++, jOld++)
                {

                    if (jOld >= Columns)
                        jOld = 0;

                    result[i, j] = this[iOld, jOld];
                }

                jOld = 0;
            }

            _data = result;
        }

        public Matrix GetRow(int index)
        {
            Matrix result = new Matrix(1, Columns);

            for (int i = 0; i < Columns; i++)
            {
                result[i] = this[index, i];
            }

            return result;
        }

        public Matrix GetColumn(int index)
        {
            Matrix result = new Matrix(Rows, 1);

            for (int i = 0; i < Rows; i++)
            {
                result[i] = this[i, index];
            }

            return result;
        }

        public double Min()
        {
            return _data.Cast<double>().Min();
        }

        public (int, int) MinIndex()
        {
            double minVal = Min();

            for (int i = 0; i < Size; i++)
            {
                if (this[i] == minVal)
                    return (i % Rows, i / Rows);
            }

            return (-1, -1);
        }

        public double Max()
        {
            return _data.Cast<double>().Max();
        }

        public (int, int) MaxIndex()
        {
            double minVal = Max();

            for (int i = 0; i < Size; i++)
            {
                if (this[i] == minVal)
                    return (i % Rows, i / Rows);
            }

            return (-1, -1);
        }

        public Matrix Clone()
        {
            Matrix result = new Matrix(Rows, Columns);

            for (int i = 0; i < Size; i++)
            {
                result[i] = this[i];
            }

            return result;
        }

        public static Matrix operator +(Matrix a, Matrix b)
        {
            Matrix result = new Matrix(Math.Max(a.Rows, b.Rows), Math.Max(a.Columns, b.Columns));

            if (a.Size < result.Size)
                a.ArrangeTo(result.Rows, result.Columns);

            if (b.Size < result.Size)
                b.ArrangeTo(result.Rows, result.Columns);

            for (int i = 0; i < result.Size; i++)
            {
                result[i] = a[i] + b[i];
            }

            return result;
        }

        public static Matrix operator +(Matrix a, double b)
        {
            Matrix result = new Matrix(a.Rows, a.Columns);

            for (int i = 0; i < result.Size; i++)
            {
                result[i] = a[i] + b;
            }

            return result;
        }

        public static Matrix operator +(double b, Matrix a)
        {
            Matrix result = new Matrix(a.Rows, a.Columns);

            for (int i = 0; i < result.Size; i++)
            {
                result[i] = a[i] + b;
            }

            return result;
        }

        public static Matrix operator -(Matrix a, Matrix b)
        {
            Matrix result = new Matrix(Math.Max(a.Rows, b.Rows), Math.Max(a.Columns, b.Columns));

            if (a.Size < result.Size)
                a.ArrangeTo(result.Rows, result.Columns);

            if (b.Size < result.Size)
                b.ArrangeTo(result.Rows, result.Columns);

            for (int i = 0; i < result.Size; i++)
            {
                result[i] = a[i] - b[i];
            }

            return result;
        }

        public static Matrix operator -(Matrix a, double b)
        {
            Matrix result = new Matrix(a.Rows, a.Columns);

            for (int i = 0; i < result.Size; i++)
            {
                result[i] = a[i] - b;
            }

            return result;
        }

        public static Matrix operator -(double a, Matrix b)
        {
            Matrix result = new Matrix(a);

            result = result - b;

            return result;
        }

        public static Matrix operator -(Matrix a)
        {
            Matrix result = new Matrix(a.Rows, a.Columns);

            for (int i = 0; i < result.Size; i++)
            {
                result[i] = a[i] * -1;
            }

            return result;
        }

        public static Matrix operator *(Matrix a, Matrix b)
        {
            Matrix result = new Matrix(Math.Max(a.Rows, b.Rows), Math.Max(a.Columns, b.Columns));

            if (a.Size < result.Size)
                a.ArrangeTo(result.Rows, result.Columns);

            if (b.Size < result.Size)
                b.ArrangeTo(result.Rows, result.Columns);

            for (int i = 0; i < result.Size; i++)
            {
                result[i] = a[i] * b[i];
            }

            return result;
        }

        public static Matrix operator *(Matrix a, double b)
        {
            Matrix result = new Matrix(a.Rows, a.Columns);

            for (int i = 0; i < result.Size; i++)
            {
                result[i] = a[i] * b;
            }

            return result;
        }

        public static Matrix operator *(double b, Matrix a)
        {
            Matrix result = new Matrix(a.Rows, a.Columns);

            for (int i = 0; i < result.Size; i++)
            {
                result[i] = a[i] * b;
            }

            return result;
        }

        public static Matrix operator /(Matrix a, Matrix b)
        {
            Matrix result = new Matrix(Math.Max(a.Rows, b.Rows), Math.Max(a.Columns, b.Columns));

            if (a.Size < result.Size)
                a.ArrangeTo(result.Rows, result.Columns);

            if (b.Size < result.Size)
                b.ArrangeTo(result.Rows, result.Columns);

            for (int i = 0; i < result.Size; i++)
            {
                result[i] = a[i] / b[i];
            }

            return result;
        }

        public static Matrix operator /(Matrix a, double b)
        {
            Matrix result = new Matrix(a.Rows, a.Columns);

            for (int i = 0; i < result.Size; i++)
            {
                result[i] = a[i] / b;
            }

            return result;
        }

        public static Matrix operator /(double a, Matrix b)
        {
            Matrix result = new Matrix(a);

            result = result / b;

            return result;
        }
    }
}
