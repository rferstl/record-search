using System;
using JetBrains.Annotations;

namespace JaroWinklerRecordSearch.MyMath
{
    [PublicAPI]
    public static class Elementwise
    {
        public static bool[,] Equals(double[,] a, double[,] b, bool[,] result, double atol = 0.0)
        {
            for (var m = 0; m < a.GetLength(0); m++)
                for (var n = 0; n < a.GetLength(1); n++)
                    result[m,n] = Math.Abs(a[m,n] - b[m,n]) <= atol;
            return result;
        }

        public static bool Equals(double[,] a, double[,] b, double atol = 0.0)
        {
            for (var m = 0; m < a.GetLength(0); m++)
            for (var n = 0; n < a.GetLength(1); n++)
                if (Math.Abs(a[m, n] - b[m, n]) > atol)
                    return false;
            return true;
        }

    }
}