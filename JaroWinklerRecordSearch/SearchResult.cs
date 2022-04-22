using JaroWinklerRecordSearch.Model;

namespace JaroWinklerRecordSearch
{
    public readonly struct SearchResult
    {
        public SearchResult(int docId, double score)
        {
            DocId = docId;
            Score = score;
        }

        public int DocId { get; }
        public double Score { get; }
    }

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