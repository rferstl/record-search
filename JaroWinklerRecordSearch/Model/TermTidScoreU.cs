using JetBrains.Annotations;

namespace JaroWinklerRecordSearch.Model
{
    [PublicAPI]
    public struct TermTidScoreU
    {
        public TermTidScoreU(string term, StIdTidScore stIdTidScore, long u, long umax)
        {
            Term = term;
            StIdTidScore = stIdTidScore;
            U = u;
            Umax = umax;
        }

        public string Term { get; set; }
        public StIdTidScore StIdTidScore { get; set; }
        public long U { get; set; }
        public long Umax { get; set; }

        public void Deconstruct(out string term, out StIdTidScore stIdTidScore, out long u, out long umax)
        {
            term = Term;
            stIdTidScore = StIdTidScore;
            u = U;
            umax = Umax;
        }
    }
}