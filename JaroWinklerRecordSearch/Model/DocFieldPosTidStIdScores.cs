using JetBrains.Annotations;

namespace JaroWinklerRecordSearch.Model
{
    [PublicAPI]
    public readonly struct DocFieldPosTidStIdScores
    {
        public DocFieldPosTidStIdScores(int docId, FieldPosTidStIdScore[] fieldPosTidStIdScores)
        {
            DocId = docId;
            FieldPosTidStIdScores = fieldPosTidStIdScores;
        }

        public int DocId { get; }
        public FieldPosTidStIdScore[] FieldPosTidStIdScores { get; }

        public object ToDumpX()
        {
            return "";
        }
    }
}