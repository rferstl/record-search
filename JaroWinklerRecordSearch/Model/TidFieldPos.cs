using JetBrains.Annotations;
using Newtonsoft.Json;

namespace JaroWinklerRecordSearch.Model
{
    [PublicAPI]
    public readonly struct TidFieldPos
    {
        public int Tid { get; }
        public FieldPos FieldPos { get; }

        public TidFieldPos(int tid, FieldEnum field, int pos) : this(tid, new FieldPos(field, pos))
        {
        }

        [JsonConstructor]
        public TidFieldPos(int tid, FieldPos fieldPos)
        {
            Tid = tid;
            FieldPos = fieldPos;
        }
    }
}