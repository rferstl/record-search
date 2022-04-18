using JetBrains.Annotations;

namespace JaroWinklerRecordSearch.Model
{
    [PublicAPI]
    public struct DocStTidScores
    {
        public DocStTidScores(int doc, StTidScore[] stTidScores)
        {
            Doc = doc;
            StTidScores = stTidScores;
        }

        public int Doc { get; set; }
        public StTidScore[] StTidScores { get; set; }

        public void Deconstruct(out int doc, out StTidScore[] stTidScores)
        {
            doc = Doc;
            stTidScores = StTidScores;
        }
    }
}