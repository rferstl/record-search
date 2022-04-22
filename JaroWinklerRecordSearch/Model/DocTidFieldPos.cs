using Newtonsoft.Json;

namespace JaroWinklerRecordSearch.Model
{
    public readonly struct DocTidFieldPos
    {
        public int DocId { get; }
        public int Tid { get; }
        public FieldPos FieldPos { get; }

        public DocTidFieldPos(int docId, TidFieldPos tidFieldPos) : this(docId, tidFieldPos.Tid, tidFieldPos.FieldPos)
        {
        }

        [JsonConstructor]
        public DocTidFieldPos(int docId, int tid, FieldPos fieldPos)
        {
            DocId = docId;
            Tid = tid;
            FieldPos = fieldPos;
        }
    }
}