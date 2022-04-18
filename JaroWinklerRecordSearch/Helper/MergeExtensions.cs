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
        public static IEnumerable<TOut> MergeBy<TIn, TKey, TOut>(this IList<IList<TIn>> aa, Func<TIn, TKey> keySelector,
            Func<TKey, IEnumerable<TIn>, TOut> merge)
        {
            var ii = Enumerable.Range(0, aa.Count).ToArray(); // iterator 1. dimension
            var jj = new int[aa.Count]; // iterators 2.dimension 
            var ll = aa.Select(a => a.Count).ToArray(); // lengths array
            var hasNext = Enumerable.Range(0, aa.Count).Select(_ => true).ToArray();

            void MoveNext(IEnumerable<int> mii)
            {
                foreach (var mi in mii)
                    hasNext[mi] = ++jj[mi] < ll[mi];
            }

            while (hasNext.Any(hn => hn))
            {
                var mii = More.MinBy(ii.Where(i => hasNext[i]), i => keySelector(aa[i][jj[i]])).ToArray();
                var currentKey = keySelector(aa[mii[0]][jj[mii[0]]]);
                yield return merge(currentKey, mii.Select(mi => aa[mi][jj[mi]]));
                MoveNext(mii);
            }
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