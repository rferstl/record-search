using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace JaroWinklerRecordSearch.Model
{
    [PublicAPI]
    public readonly struct IndexDoc
    {
        public int Id { get; }
        public IList<TidFieldPos> Tpl { get; }

        [JsonConstructor]
        public IndexDoc(int id, IList<TidFieldPos> tpl)
        {
            Id = id;
            Tpl = tpl;
        }

        public (int id, IList<TidFieldPos> tpl) ToTuple() => (Id, Tpl);
    }
}