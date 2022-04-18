using JetBrains.Annotations;

namespace JaroWinklerRecordSearch.Model
{
    [PublicAPI]
    public struct TermScore
    {
        public TermScore(string term, double score)
        {
            Term = term;
            Score = score;
        }

        public string Term { get; set; }
        public double Score { get; set; }

        public void Deconstruct(out string term, out double score)
        {
            term = Term;
            score = Score;
        }
    }
}