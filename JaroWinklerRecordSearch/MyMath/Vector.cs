namespace JaroWinklerRecordSearch.MyMath
{
    public static class Vector
    {
        public static T[] Create<T>(int size, T value)
        {
            var array = new T[size];
            for (var i = 0; i < array.Length; i++)
                array[i] = value;
            return array;
        }

    }
}