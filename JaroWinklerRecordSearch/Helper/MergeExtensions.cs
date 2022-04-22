using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using More = MoreLinq.MoreEnumerable;

namespace JaroWinklerRecordSearch.Helper
{
    [PublicAPI]
    public static class MergeExtensions
    {
        public static IEnumerable<TOut> MergeBy2<TIn, TKey, TOut>(this TIn[][] aa, Func<TIn, TKey> keySelector,
            Func<TKey, IEnumerable<TIn>, TOut> merge) where TKey : IComparable<TKey>
        {
            var ii = Enumerable.Range(0, aa.Length).ToArray(); // iterator 1. dimension
            var jj = new int[aa.Length]; // iterators 2.dimension 
            var ll = aa.Select(a => a.Length).ToArray(); // lengths array
            var hasNext = Enumerable.Range(0, aa.Length).Select(_ => true).ToArray();

            while (hasNext.Any(hn => hn))
            {
                var mii = More.MinBy(ii.Where(i => hasNext[i]), i => keySelector(aa[i][jj[i]])).ToArray();
                var currentKey = keySelector(aa[mii[0]][jj[mii[0]]]);
                yield return merge(currentKey, mii.Select(mi => aa[mi][jj[mi]]));
                for (var mi=0; mi<mii.Length; mi++)
                    hasNext[mi] = ++jj[mi] < ll[mi];
            }
        }

        public static int[] MinBy<TKey>(int[] source, Predicate<int> filter, Func<int,TKey> selector) where TKey : IComparable<TKey>
        {
            if (source.Length == 0)
                return Array.Empty<int>();

            var firstIndex = Array.FindIndex(source, filter);
            if(firstIndex < 0)
                return Array.Empty<int>();
            var extrema = new List<int>(source.Length) { source[firstIndex] };
            var extremaKey = selector(source[firstIndex]);

            for (var i=firstIndex+1; i<source.Length; i++)
            {
                var item = source[i];
                if (!filter(item))
                    continue;
                var key = selector(item);
                var comparison = key.CompareTo(extremaKey);
                switch (comparison)
                {
                    case < 0:
                        extrema.Clear();
                        extrema.Add(item);
                        extremaKey = key;
                        break;
                    case 0:
                        extrema.Add(item);
                        break;
                }
            }
            return extrema.ToArray();
        }

        public static IEnumerable<TOut> MergeBy<TIn, TKey, TOut>(this IEnumerable<IEnumerable<TIn>> sources,
            Func<TIn, TKey> keySelector, Func<TKey, IEnumerable<TIn>, TOut> merge)
        {
            var aa = sources.Select(a => a.GetEnumerator()).ToArray();
            var ii = Enumerable.Range(0, aa.Length).ToArray(); // iterator 1.dimension
            var hasNext = Enumerable.Range(0, aa.Length).Select(_ => true).ToArray();

            void MoveNext(IEnumerable<int> mii)
            {
                foreach (var mi in mii)
                    hasNext[mi] = aa[mi].MoveNext();
            }

            MoveNext(ii);
            while (hasNext.Any(hn => hn))
            {
                var mii = More.MinBy(ii.Where(i => hasNext[i]), i => keySelector(aa[i].Current)).ToArray();
                var currentKey = keySelector(aa[mii[0]].Current);
                yield return merge(currentKey, mii.Select(mi => aa[mi].Current));
                MoveNext(mii);
            }
        }
    }
}