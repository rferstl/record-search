using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace JaroWinklerRecordSearch.Model
{
    [PublicAPI]
    public readonly struct ScdDoc
    {
        public int Id { get; }
        public IList<TidFieldPos> Tpl { get; }

        [JsonConstructor]
        public ScdDoc(int id, IList<TidFieldPos> tpl)
        {
            Id = id;
            Tpl = tpl;
        }

        public (int id, IList<TidFieldPos> tpl) ToTuple() => (Id, Tpl);
    }
}