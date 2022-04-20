using System;
using System.Collections.Generic;
using System.Linq;

namespace JaroWinklerRecordSearch.MyMath
{
    public static class Matrix
    {
        public static int Columns<T>(this T[,] matrix)
        {
            return matrix.GetLength(1);
        }

        public static int Rows<T>(this T[,] matrix)
        {
            return matrix.GetLength(0);
        }

        public static double[,] Multiply(this double[,] a, int b)
        {
            var rows = a.GetLength(0);
            var cols = a.GetLength(1);
            var result = new double[rows, cols];

            for (var i = 0; i < rows; i++)
            for (var j = 0; j < cols; j++)
                result[i, j] = a[i, j] * b;
            return result;
        }

        public static double[,] Subtract(this double[,] a, double[] b, VectorType dimension)
        {
            return a.Subtract(b, dimension, CreateAs(a));
        }
		
        public static double[,] Subtract(this double[,] a, double[] b, VectorType dimension, double[,] result)
        {
            if (dimension == VectorType.RowVector)
            {
                for (var i = 0; i < a.GetLength(0); i++)
                {
                    for (var j = 0; j < a.GetLength(1); j++)
                    {
                        result[i,j] = a[i,j] - b[j];
                    }
                }
            }
            else
            {
                for (var k = 0; k < a.GetLength(0); k++)
                {
                    for (var l = 0; l < a.GetLength(1); l++)
                    {
                        result[k,l] = a[k,l] - b[k];
                    }
                }
            }
            return result;
        }

        public static T[,] Copy<T>(this T[,] a)
        {
            return (T[,])a.Clone();
        }

        public static T[] Min<T>(this T[,] matrix, int dimension) where T : IComparable<T>
        {
            var length = GetLength(matrix, dimension);
            var result = new T[length];
            var indices = new int[length];
            return matrix.Min(dimension, indices, result);
        }

        public static T[] Min<T>(this T[,] matrix, int dimension, int[] indices, T[] result) where T : IComparable<T>
        {
            if (dimension == 1)
            {
                matrix.GetColumn(0, result);
                for (var i = 0; i < matrix.GetLength(0); i++)
                {
                    for (var j = 0; j < matrix.GetLength(1); j++)
                    {
                        if (matrix[i,j].CompareTo(result[i]) < 0)
                        {
                            result[i] = matrix[i,j];
                            indices[i] = j;
                        }
                    }
                }
            }
            else
            {
                matrix.GetRow(0, result);
                for (var k = 0; k < result.Length; k++)
                {
                    for (var l = 0; l < matrix.GetLength(0); l++)
                    {
                        if (matrix[l,k].CompareTo(result[k]) < 0)
                        {
                            result[k] = matrix[l,k];
                            indices[k] = l;
                        }
                    }
                }
            }
            return result;
        }
        public static T[] GetColumn<T>(this T[,] m, int index, T[] result)
        {
            index = Index(index, m.Columns());
            for (var i = 0; i < result.Length; i++)
                result[i] = m[i,index];
            return result;
        }
        public static T[] GetRow<T>(this T[,] m, int index, T[] result)
        {
            index = Index(index, m.Rows());
            for (var i = 0; i < result.Length; i++)
                result[i] = m[index, i];
            return result;
        }
        public static T[,] SetColumn<T>(this T[,] m, int index, T value)
        {
            index = Index(index, m.Columns());
            for (var i = 0; i < m.GetLength(0); i++)
                m[i,index] = value;
            return m;
        }
        public static T[,] SetRow<T>(this T[,] m, int index, T value)
        {
            index = Index(index, m.Rows());
            for (var i = 0; i < m.GetLength(1); i++)
                m[index,i] = value;
            return m;
        }
        private static int Index(int end, int length)
        {
            if (end < 0)
            {
                end = length + end;
            }
            return end;
        }
        internal static int GetLength<T>(T[,] values, int dimension)
        {
            return values.GetLength(dimension == 1 ? 0 : 1);
        }
        public static void Clear(this Array array)
        {
            Array.Clear(array, 0, array.Length);
        }
        public static bool IsEqual(this double a, double b, double atol = 0.0)
        {
            return Math.Abs(a - b) <= atol;
        }

        public static T[,] ToMatrix<T>(this T[][] array, bool transpose = false)
        {
            var nrow = array.Length;
            if (nrow == 0)
                return new T[0, nrow];
            var ncol = array[0].Length;
            T[,] matrix;
            if (transpose)
            {
                matrix = new T[ncol, nrow];
                for (var r = 0; r < nrow; ++r)
                {
                    for (var c = 0; c < ncol; ++c)
                        matrix[c, r] = array[r][c];
                }
            }
            else
            {
                matrix = new T[nrow, ncol];
                for (var r = 0; r < nrow; ++r)
                {
                    for (var c = 0; c < ncol; ++c)
                        matrix[r, c] = array[r][c];
                }
            }
            return matrix;
        }

        public static T[,] Create<T>(int rows, int columns)
        {
            var matrix = new T[rows, columns];
            return matrix;
        }

        public static T[,] Create<T>(int rows, int columns, T value)
        {
            var matrix = new T[rows, columns];
            for (var i = 0; i < rows; i++)
            for (var j = 0; j < columns; j++)
                matrix[i, j] = value;
            return matrix;
        }

        public static T[,] CreateAs<T>(this T[,] matrix)
        {
            return new T[matrix.GetLength(0), matrix.GetLength(1)];
        }
		
        public static TOutput[,] CreateAs<TInput, TOutput>(this TInput[,] matrix)
        {
            return new TOutput[matrix.GetLength(0), matrix.GetLength(1)];
        }

        public static double[,] ToSquare(this double[,] m, int n, double fill)
        {
            var (nRows, nCols) = (m.GetLength(0), m.GetLength(1));
            var mSquare = new double[n, n];
            for (var i = 0; i < n; i++)
            for (var j = 0; j < n; j++)
                mSquare[i, j] = i < nRows && j < nCols ?  m[i, j] : fill;
            return mSquare;
        }

        public static double MaxAbs(this double[,] m)
        {
            var (nRows, nCols) = (m.GetLength(0), m.GetLength(1));
            var max = double.MinValue;
            for (var i = 0; i < nRows; i++)
            for (var j = 0; j < nCols; j++)
            {
                var v = Math.Abs(m[i, j]);
                if (double.IsInfinity(v) || double.IsNaN(v))
                    throw new ArgumentOutOfRangeException();
                if (v > max)
                    max = v;
            }
            return max;
        }

    }
}