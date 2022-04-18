using JetBrains.Annotations;

namespace JaroWinklerRecordSearch.Model
{
    [PublicAPI]
    public readonly struct SearchResult
    {
        public SearchResult(string text, TermScore[] termScores, double score)
        {
            Text = text;
            TermScores = termScores;
            Score = score;
        }

        public string Text { get; }
        public TermScore[] TermScores { get; }
        public double Score { get; }

        public void Deconstruct(out string text, out TermScore[] termScores, out double score)
        {
            text = Text;
            termScores = TermScores;
            score = Score;
        }
    }
}