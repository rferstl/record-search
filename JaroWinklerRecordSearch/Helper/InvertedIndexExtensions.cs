using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace JaroWinklerRecordSearch.Helper
{
    [PublicAPI]
    public static class InvertedIndexExtensions
    {
        public static IDictionary<TItemOut, IList<TKey>> InvertedIndex<TKey, TItemIn, TItemOut>(
            this IDictionary<TKey, IList<TItemIn>> dictionary, Func<TItemIn, TItemOut> elementSelector)
            where TKey : notnull where TItemIn : notnull where TItemOut : notnull
        {
            return dictionary
                .SelectMany(keyValuePair => keyValuePair.Value.Select(itemIn =>
                    new KeyValuePair<TItemOut, TKey>(elementSelector(itemIn), keyValuePair.Key)))
                .GroupBy(keyValuePair => keyValuePair.Key)
                .ToDictionary(group => group.Key,
                    group => group.Select(keyValuePair => keyValuePair.Value).ToArray() as IList<TKey>);
        }
    }
}