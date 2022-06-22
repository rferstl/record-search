using System;
using System.Collections.Generic;
using System.Linq;
using JaroWinklerRecordSearch.Model;
using JetBrains.Annotations;

namespace JaroWinklerRecordSearch.Helper
{
    [PublicAPI]
    public static class InvertedIndexExtensions
    {
        public static IList<IList<TItemOut>> InvertedIndex<TItemIn, TItemOut>(
            this IDictionary<int, IList<TItemIn>> docs, Func<TItemIn, int> keySelector, Func<TItemIn, TItemOut> elementSelector, int count)
            where TItemIn : notnull where TItemOut : notnull
        {
            var gs = docs
                .SelectMany(d => d.Value.Select(tfp => new KeyValuePair<int, TItemOut>(keySelector(tfp), elementSelector(tfp))).Where(kv => kv.Key < count))
                .GroupBy(e => e.Key);
            var array = new IList<TItemOut>[count];	
            foreach(var g in gs)
                array[g.Key] = g.Select(kv => kv.Value).ToArray();
            return array;
        }

        public static IList<IList<DocTidFieldPos>> InvertedIndex(this IDictionary<int, IList<TidFieldPos>> docs, int tidCount)
        {
            var gs = docs
                .SelectMany(d => d.Value.Select(tfp => new KeyValuePair<int, DocTidFieldPos>(tfp.Tid, new DocTidFieldPos(d.Key, tfp))).Where(kv => kv.Key < tidCount))
                .GroupBy(e => e.Key);
            var array = new IList<DocTidFieldPos>[tidCount];	
            foreach(var g in gs)
                array[g.Key] = g.Select(kv => kv.Value).ToArray();
            return array;
        }

    }
}