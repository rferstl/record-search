using JetBrains.Annotations;
using Newtonsoft.Json;

namespace JaroWinklerRecordSearch.Model
{
    [PublicAPI]
    public readonly struct SearchTerm
    {
        public int Id { get; }
        public string Orig { get; }
        public string Norm { get; }

        [JsonConstructor]
        public SearchTerm(int id, string orig, string norm)
        {
            Id = id;
            Orig = orig;
            Norm = norm;
        }

        public void Deconstruct(out int id, out string orig, out string norm)
        {
            id = Id;
            orig = Orig;
            norm = Norm;
        }

        public override string ToString()
        {
            return $"{Orig}|{Norm} ({Id})";
        }
    }
}