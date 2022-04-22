using JetBrains.Annotations;

namespace JaroWinklerRecordSearch.Model
{
    [PublicAPI]
    public struct StIdTidScore
    {
        public StIdTidScore(int stId, int tid, double score)
        {
            StId = stId;
            Tid = tid;
            Score = score;
        }

        public int StId { get; }
        public int Tid { get; }
        public double Score { get; }

        public void Deconstruct(out int stId, out int tid, out double score)
        {
            stId = StId;
            tid = Tid;
            score = Score;
        }
    }
}