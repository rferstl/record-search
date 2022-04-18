using JetBrains.Annotations;

namespace JaroWinklerRecordSearch.Model
{
    [PublicAPI]
    public struct TermTidScoreU
    {
        public TermTidScoreU(string term, StTidScore stTidScore, long u, long umax)
        {
            Term = term;
            StTidScore = stTidScore;
            U = u;
            Umax = umax;
        }

        public string Term { get; set; }
        public StTidScore StTidScore { get; set; }
        public long U { get; set; }
        public long Umax { get; set; }

        public void Deconstruct(out string term, out StTidScore stTidScore, out long u, out long umax)
        {
            term = Term;
            stTidScore = StTidScore;
            u = U;
            umax = Umax;
        }
    }
}