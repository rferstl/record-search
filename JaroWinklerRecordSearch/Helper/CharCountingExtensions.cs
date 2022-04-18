using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace JaroWinklerRecordSearch.Helper
{
    [PublicAPI]
    public static class CharCountingExtensions
    {
        public static void Add<T>(this IDictionary<T, int> dict, T x) => dict[x] = dict.GetValueOrDefault(x, 0) + 1;

        public static IDictionary<string, int> Accumulate(this IDictionary<string, int> dict, IEnumerable<string> list)
        {
            foreach (var item in list)
                dict[item] = dict.GetValueOrDefault(item, 0) + 1;
            return dict;
        }

        public static ulong StringToBitmask(this IDictionary<string, int> bitArrayCharMap, string s)
        {
            var charList = s.ToCharList();
            var bitmask = bitArrayCharMap.ToBitArray64(charList).ToUInt64();
            return bitmask;
        }

        public static IList<string> CharList(this IDictionary<char, int> dict) =>
            dict.SelectMany(e => CharList(e.Key, e.Value)).ToArray();

        public static IList<string> CharList(this char c, int n) =>
            Enumerable.Range(1, n).Select(i => $"{c}{i}").ToArray();

        public static int Count<T>(this IDictionary<T, int> dict, T x) => dict.GetValueOrDefault(x, 0);

        public static bool ContainsNtimes<T>(this IDictionary<T, int> dict, T x, int n) =>
            dict.GetValueOrDefault(x, 0) >= n;

        public static ulong ToUInt64(this BitArray bitArray)
        {
            var array = new byte[8];
            bitArray.CopyTo(array, 0);
            return BitConverter.ToUInt64(array, 0);
        }

        public static BitArray ToBitArray64(this IDictionary<string, int> bitArrayCharMap, IList<string> charList)
        {
            var bitArray = new BitArray(64);
            foreach (var c in charList)
            {
                var i = bitArrayCharMap.GetValueOrDefault(c, -1);
                if (i < 0) continue;
                bitArray.Set(i, true);
            }

            return bitArray;
        }

        public static IDictionary<char, int> CountChars(this string s)
        {
            var charCountDict = new SortedDictionary<char, int>();
            foreach (var c in s)
                charCountDict[c] = charCountDict.GetValueOrDefault(c, 0) + 1;
            return charCountDict;
        }

        public static IList<string> ToCharList(this string s) => s.CountChars().CharList();

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
            TValue defaultValue)
            => dictionary.TryGetValue(key, out var value) ? value : defaultValue;

        public static IEnumerable<TSource> IntersectByImpl<TSource, TKey>(
            this IEnumerable<TSource> first,
            IEnumerable<TSource> second,
            Func<TSource, TKey> keySelector)
        {
            var keyComparer = EqualityComparer<TKey>.Default;

            var keys = new HashSet<TKey>(keyComparer);
            foreach (var item in first)
            {
                var k = keySelector(item);
                if (keys.Add(k))
                {
                    yield return item;
                    keys.Remove(k);
                }
            }
        }
    }
}