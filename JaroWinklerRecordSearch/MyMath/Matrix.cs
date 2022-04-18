using System;
using System.Collections.Generic;
using System.Linq;

namespace JaroWinklerRecordSearch.MyMath
{
    public static class Matrix
    {
        public static int Columns<T>(this T[][] matrix)
        {
            if (matrix.Length == 0)
            {
                return 0;
            }
            return matrix[0].Length;
        }

        public static int Rows<T>(this T[][] matrix)
        {
            return matrix.Length;
        }

        public static double[][] Multiply(this double[][] a, int b)
        {
            return a.Multiply(b, Jagged.CreateAs<double, double>(a));
        }

        public static double[][] Multiply(this double[][] a, int b, double[][] result)
        {
            for (var i = 0; i < a.Length; i++)
            {
                for (var j = 0; j < a[i].Length; j++)
                {
                    result[i][j] = a[i][j] * (double)b;
                }
            }
            return result;
        }

        public static double[][] Subtract(this double[][] a, double[] b, VectorType dimension)
        {
            return a.Subtract(b, dimension, Jagged.CreateAs<double, double>(a));
        }
		
        public static double[][] Subtract(this double[][] a, double[] b, VectorType dimension, double[][] result)
        {
            if (dimension == VectorType.RowVector)
            {
                for (var i = 0; i < a.Length; i++)
                {
                    for (var j = 0; j < a[i].Length; j++)
                    {
                        result[i][j] = a[i][j] - b[j];
                    }
                }
            }
            else
            {
                for (var k = 0; k < a.Length; k++)
                {
                    for (var l = 0; l < a[k].Length; l++)
                    {
                        result[k][l] = a[k][l] - b[k];
                    }
                }
            }
            return result;
        }

        public static T[][] Copy<T>(this T[][] a)
        {
            var array = new T[a.Length][];
            for (var i = 0; i < a.Length; i++)
            {
                array[i] = (T[])a[i].Clone();
            }
            return array;
        }

        public static T[][] Get<T>(this T[][] source, bool[] rowMask, bool[] columnMask, bool reuseMemory, T[][] result)
        {
            return source.get(result, rowMask.Find(x => x), columnMask.Find(x => x), reuseMemory);
        }

        public static int[] Find<T>(this T[] data, Func<T, bool> func)
        {
            var list = new List<int>();
            for (var i = 0; i < data.Length; i++)
            {
                if (func(data[i]))
                {
                    list.Add(i);
                }
            }
            return list.ToArray();
        }

        public static T[] Min<T>(this T[][] matrix, int dimension) where T : IComparable<T>
        {
            var length = GetLength(matrix, dimension);
            var result = new T[length];
            var indices = new int[length];
            return matrix.Min(dimension, indices, result);
        }

        public static T[] Min<T>(this T[][] matrix, int dimension, int[] indices, T[] result) where T : IComparable<T>
        {
            if (dimension == 1)
            {
                matrix.GetColumn(0, result);
                for (var i = 0; i < matrix.Length; i++)
                {
                    for (var j = 0; j < matrix[i].Length; j++)
                    {
                        if (matrix[i][j].CompareTo(result[i]) < 0)
                        {
                            result[i] = matrix[i][j];
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
                    for (var l = 0; l < matrix.Length; l++)
                    {
                        if (matrix[l][k].CompareTo(result[k]) < 0)
                        {
                            result[k] = matrix[l][k];
                            indices[k] = l;
                        }
                    }
                }
            }
            return result;
        }
        public static T[] GetColumn<T>(this T[][] m, int index, T[] result)
        {
            if (result == null)
            {
                result = new T[m.Length];
            }
            index = Matrix.index(index, m.Columns());
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = m[i][index];
            }
            return result;
        }
        public static T[] GetRow<T>(this T[][] m, int index, T[] result)
        {
            index = Matrix.index(index, m.Rows());
            if (result == null)
            {
                return (T[])m[index].Clone();
            }
            m[index].CopyTo(result, 0);
            return result;
        }
        public static T[][] SetColumn<T>(this T[][] m, int index, T value)
        {
            index = Matrix.index(index, m.Columns());
            for (var i = 0; i < m.Length; i++)
            {
                m[i][index] = value;
            }
            return m;
        }
        public static T[][] SetRow<T>(this T[][] m, int index, T value)
        {
            index = Matrix.index(index, m.Rows());
            for (var i = 0; i < m[index].Length; i++)
            {
                m[index][i] = value;
            }
            return m;
        }
        private static int index(int end, int length)
        {
            if (end < 0)
            {
                end = length + end;
            }
            return end;
        }
        internal static int GetLength<T>(T[][] values, int dimension)
        {
            if (dimension == 1)
            {
                return values.Length;
            }
            return values[0].Length;
        }
        public static void Clear(this Array array)
        {
            Array.Clear(array, 0, array.Length);
        }
        public static bool IsEqual(this double a, double b, double atol = 0.0, double rtol = 0.0)
        {
            if (rtol > 0.0)
            {
                if (a == b)
                {
                    return true;
                }
                if (double.IsNaN(a) && double.IsNaN(b))
                {
                    return true;
                }
                if (double.IsNaN(a) ^ double.IsNaN(b))
                {
                    return false;
                }
                if (double.IsPositiveInfinity(a) ^ double.IsPositiveInfinity(b))
                {
                    return false;
                }
                if (double.IsNegativeInfinity(a) ^ double.IsNegativeInfinity(b))
                {
                    return false;
                }
                var num = System.Math.Abs(a - b);
                if (a == 0.0)
                {
                    if (num <= rtol)
                    {
                        return true;
                    }
                }
                else if (b == 0.0 && num <= rtol)
                {
                    return true;
                }
                if (num <= System.Math.Abs(a) * rtol)
                {
                    return true;
                }
                return false;
            }
            if (atol > 0.0)
            {
                if (a == b)
                {
                    return true;
                }
                if (double.IsNaN(a) && double.IsNaN(b))
                {
                    return true;
                }
                if (double.IsNaN(a) ^ double.IsNaN(b))
                {
                    return false;
                }
                if (double.IsPositiveInfinity(a) ^ double.IsPositiveInfinity(b))
                {
                    return false;
                }
                if (double.IsNegativeInfinity(a) ^ double.IsNegativeInfinity(b))
                {
                    return false;
                }
                if (System.Math.Abs(a - b) <= atol)
                {
                    return true;
                }
                return false;
            }
            if (double.IsNaN(a) && double.IsNaN(b))
            {
                return true;
            }
            if (double.IsNaN(a) ^ double.IsNaN(b))
            {
                return false;
            }
            if (double.IsPositiveInfinity(a) ^ double.IsPositiveInfinity(b))
            {
                return false;
            }
            if (double.IsNegativeInfinity(a) ^ double.IsNegativeInfinity(b))
            {
                return false;
            }
            if (a != b)
            {
                return false;
            }
            return true;
        }
        private static T[][] get<T>(this T[][] source, T[][] destination, IReadOnlyList<int> rowIndexes, IReadOnlyList<int> columnIndexes, bool reuseMemory)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (source.Length == 0)
            {
                return Array.Empty<T[]>();
            }
            var num = source.Length;
            var num2 = source[0].Length;
            var num3 = num;
            var num4 = num2;
            num3 = rowIndexes.Count;
            if (rowIndexes.Any(t => t < 0 || t >= num))
                throw new ArgumentException("Argument out of range.");
            num4 = columnIndexes.Count;
            if (columnIndexes.Any(t => t < 0 || t >= num2))
                throw new ArgumentException("Argument out of range.");
            if (destination != null)
            {
                if (destination.Length < num3)
                {
                    throw new ArgumentException("destination The destination matrix must be big enough to accommodate the results.");
                }
            }
            else
            {
                destination = new T[num3][];
                if (columnIndexes != null && !reuseMemory)
                {
                    for (var k = 0; k < destination.Length; k++)
                    {
                        destination[k] = new T[num4];
                    }
                }
            }
            if (columnIndexes == null)
            {
                if (reuseMemory)
                {
                    for (var l = 0; l < rowIndexes.Count; l++)
                    {
                        destination[l] = source[rowIndexes[l]];
                    }
                }
                else
                {
                    for (var m = 0; m < rowIndexes.Count; m++)
                    {
                        destination[m] = (T[])source[rowIndexes[m]].Clone();
                    }
                }
            }
            else if (rowIndexes == null)
            {
                for (var n = 0; n < source.Length; n++)
                {
                    for (var num5 = 0; num5 < columnIndexes.Count; num5++)
                    {
                        destination[n][num5] = source[n][columnIndexes[num5]];
                    }
                }
            }
            else
            {
                for (var num6 = 0; num6 < rowIndexes.Count; num6++)
                {
                    for (var num7 = 0; num7 < columnIndexes.Count; num7++)
                    {
                        destination[num6][num7] = source[rowIndexes[num6]][columnIndexes[num7]];
                    }
                }
            }
            return destination;
        }
    }
}