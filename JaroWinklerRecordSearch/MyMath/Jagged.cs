namespace JaroWinklerRecordSearch.MyMath
{
    public static class Jagged
    {
        public static T[][] Zeros<T>(int rows, int columns)
        {
            var array = new T[rows][];
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = new T[columns];
            }
            return array;
        }
		
        public static double[][] Zeros(int rows, int columns)
        {
            return Zeros<double>(rows, columns);
        }

        public static T[][] Create<T>(int rows, int columns)
        {
            return Zeros<T>(rows, columns);
        }

        public static T[][] Create<T>(int rows, int columns, T value)
        {
            var array = new T[rows][];
            for (var i = 0; i < rows; i++)
            {
                var array2 = (array[i] = new T[columns]);
                for (var j = 0; j < array2.Length; j++)
                {
                    array2[j] = value;
                }
            }
            return array;
        }

        public static T[][] CreateAs<T>(T[][] matrix)
        {
            var array = new T[matrix.Length][];
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = new T[matrix[i].Length];
            }
            return array;
        }
		
        public static TOutput[][] CreateAs<TInput, TOutput>(TInput[][] matrix)
        {
            var array = new TOutput[matrix.Length][];
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = new TOutput[matrix[i].Length];
            }
            return array;
        }

    }
}