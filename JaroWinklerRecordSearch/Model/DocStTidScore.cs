using JetBrains.Annotations;

namespace JaroWinklerRecordSearch.Model
{
    [PublicAPI]
    public struct DocStTidScore
    {
        public DocStTidScore(int docId, StTidScore stTidScore)
        {
            DocId = docId;
            StTidScore = stTidScore;
        }

        public int DocId { get; set; }
        public StTidScore StTidScore { get; set; }

        public void Deconstruct(out int docId, out StTidScore stTidScore)
        {
            docId = DocId;
            stTidScore = StTidScore;
        }
    }
}