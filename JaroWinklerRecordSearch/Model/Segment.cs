using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace JaroWinklerRecordSearch.Model
{
    [PublicAPI]
    public readonly struct Segment
    {
        public char First { get; }
        public int Len { get; }
        public IList<int> TermIds { get; }
        public IList<ulong> Bitmaps { get; }

        public Segment(char first, int len, IList<int> termIds, IList<ulong> bitmaps)
        {
            First = first;
            Len = len;
            TermIds = termIds;
            Bitmaps = bitmaps;
        }

        public (char, int, IList<int>, IList<ulong>) ToTuple() => (First, Len, TermIds, Bitmaps);

        public object ToDump() => new
        { First, Len, TermIds, Bitmaps = Bitmaps.Select(n => Convert.ToString((long)n, 2).PadLeft(64, '0')) };
    }

}