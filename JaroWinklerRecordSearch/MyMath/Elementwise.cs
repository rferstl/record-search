namespace JaroWinklerRecordSearch.MyMath
{
    public static class Elementwise
    {
        public static bool[][] Equals(double[][] a, double[][] b, double atol = 0.0, double rtol = 0.0)
        {
            var array = Jagged.CreateAs<double, bool>(a);
            if (rtol > 0.0)
            {
                for (var i = 0; i < a.Length; i++)
                {
                    for (var j = 0; j < a[i].Length; j++)
                    {
                        var num = a[i][j];
                        var num2 = b[i][j];
                        if (num == num2)
                        {
                            array[i][j] = true;
                            continue;
                        }
                        if (double.IsNaN(num) && double.IsNaN(num2))
                        {
                            array[i][j] = true;
                            continue;
                        }
                        if (double.IsNaN(num) ^ double.IsNaN(num2))
                        {
                            array[i][j] = false;
                            continue;
                        }
                        if (double.IsPositiveInfinity(num) ^ double.IsPositiveInfinity(num2))
                        {
                            array[i][j] = false;
                            continue;
                        }
                        if (double.IsNegativeInfinity(num) ^ double.IsNegativeInfinity(num2))
                        {
                            array[i][j] = false;
                            continue;
                        }
                        var num3 = num;
                        var num4 = num2;
                        var num5 = System.Math.Abs(num3 - num4);
                        if (num3 == 0.0)
                        {
                            if (num5 <= rtol)
                            {
                                array[i][j] = true;
                                continue;
                            }
                        }
                        else if (num4 == 0.0 && num5 <= rtol)
                        {
                            array[i][j] = true;
                            continue;
                        }
                        array[i][j] = num5 <= System.Math.Abs(num3) * rtol;
                    }
                }
            }
            else if (atol > 0.0)
            {
                for (var k = 0; k < a.Length; k++)
                {
                    for (var l = 0; l < a[k].Length; l++)
                    {
                        var num6 = a[k][l];
                        var num7 = b[k][l];
                        if (num6 == num7)
                        {
                            array[k][l] = true;
                            continue;
                        }
                        if (double.IsNaN(num6) && double.IsNaN(num7))
                        {
                            array[k][l] = true;
                            continue;
                        }
                        if (double.IsNaN(num6) ^ double.IsNaN(num7))
                        {
                            array[k][l] = false;
                            continue;
                        }
                        if (double.IsPositiveInfinity(num6) ^ double.IsPositiveInfinity(num7))
                        {
                            array[k][l] = false;
                            continue;
                        }
                        if (double.IsNegativeInfinity(num6) ^ double.IsNegativeInfinity(num7))
                        {
                            array[k][l] = false;
                            continue;
                        }
                        var num8 = num6;
                        var num9 = num7;
                        array[k][l] = System.Math.Abs(num8 - num9) <= atol;
                    }
                }
            }
            else
            {
                for (var m = 0; m < a.Length; m++)
                {
                    for (var n = 0; n < a[m].Length; n++)
                    {
                        var num10 = a[m][n];
                        var num11 = b[m][n];
                        if (double.IsNaN(num10) && double.IsNaN(num11))
                        {
                            array[m][n] = true;
                        }
                        else if (double.IsNaN(num10) ^ double.IsNaN(num11))
                        {
                            array[m][n] = false;
                        }
                        else if (double.IsPositiveInfinity(num10) ^ double.IsPositiveInfinity(num11))
                        {
                            array[m][n] = false;
                        }
                        else if (double.IsNegativeInfinity(num10) ^ double.IsNegativeInfinity(num11))
                        {
                            array[m][n] = false;
                        }
                        else
                        {
                            array[m][n] = num10 == num11;
                        }
                    }
                }
            }
            return array;
        }
	
    }
}