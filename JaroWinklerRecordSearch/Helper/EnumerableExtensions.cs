using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace JaroWinklerRecordSearch.Helper
{
    [PublicAPI]
    public static class EnumerableExtensions
    {
        public static Dictionary<TKey, TElement> ToDictionaryFirst<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer = null)
        {
            var d = new Dictionary<TKey, TElement>(comparer);
            foreach (var element in source)
            {
                var key = keySelector(element);
                if (!d.ContainsKey(key))
                    d.Add(key, elementSelector(element));
            }
            return d;
        }

    }
}
