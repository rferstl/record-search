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
}