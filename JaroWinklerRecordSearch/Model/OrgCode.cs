using JetBrains.Annotations;
using Newtonsoft.Json;

namespace JaroWinklerRecordSearch.Model
{
    [PublicAPI]
    public readonly struct OrgCode
    {
        public int Id { get; }
        public string Name { get; }

        [JsonConstructor]
        public OrgCode(int id, string name)
        {
            Id = id;
            Name = name;
        }
        public (int id, string name) ToTuple() => (Id, Name);
    }
}
