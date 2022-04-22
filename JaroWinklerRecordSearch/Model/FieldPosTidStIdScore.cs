using JetBrains.Annotations;

namespace JaroWinklerRecordSearch.Model
{
    [PublicAPI]
    public readonly struct FieldPosTidStIdScore
    {
        public FieldPosTidStIdScore(FieldPos fieldPos, int tid, int stId, double score)
        {
            FieldPos = fieldPos;
            Tid = tid;
            StId = stId;
            Score = score;
        }

        public FieldPos FieldPos { get; }
        public int Tid { get; }
        public int StId { get; }
        public double Score { get; }

        public object ToDumpX()
        {
            return "";
        }
    }
}
