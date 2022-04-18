using System.Collections.Generic;

namespace JaroWinklerRecordSearch.Model
{
    public struct NameParts<T>
    {
        public int Id { get; }
        public IList<T> NormalizedFirstNames { get; }
        public IList<T> OriginalFirstNames { get; }
        public IList<T> NormalizedLastNames { get; }
        public IList<T> OriginalLastNames { get; }

        public NameParts(int id, IList<T> nfns, IList<T> ofns, IList<T> nlns, IList<T> olns)
        {
            Id = id;
            NormalizedFirstNames = nfns;
            OriginalFirstNames = ofns;
            NormalizedLastNames = nlns;
            OriginalLastNames = olns;
        }
    }
}