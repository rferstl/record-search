using JaroWinklerRecordSearch.Model;
using JetBrains.Annotations;

namespace JaroWinklerRecordSearch
{
    [PublicAPI]
    public readonly struct SearchResultDebug
    {
        public SearchResultDebug(string text, double score,  FieldPosTidStIdScore[] scoreInfo)
        {
            Text = text;
            Score = score;
            ScoreInfo = scoreInfo;
        }

        public string Text { get; }
        public double Score { get; }
        public FieldPosTidStIdScore[] ScoreInfo { get; }
    }
}