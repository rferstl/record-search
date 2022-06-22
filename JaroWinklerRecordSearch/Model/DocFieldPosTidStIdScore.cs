using JetBrains.Annotations;

namespace JaroWinklerRecordSearch.Model
{
    [PublicAPI]
    public readonly struct DocFieldPosTidStIdScore
    {
        // ReSharper disable once UseDeconstructionOnParameter
        public DocFieldPosTidStIdScore(DocTidFieldPos docTidFieldPos, StIdTidScore stIdTidScore)
            : this(docTidFieldPos.DocId, new FieldPosTidStIdScore(docTidFieldPos.FieldPos, stIdTidScore.Tid, stIdTidScore.StId, stIdTidScore.Score))
        {
        }

        public DocFieldPosTidStIdScore(int docId, FieldPosTidStIdScore fieldPosTidStIdScore)
        {
            DocId = docId;
            FieldPosTidStIdScore = fieldPosTidStIdScore;
        }

        public int DocId { get; }
        public FieldPosTidStIdScore FieldPosTidStIdScore { get; }

        public object ToDumpX()
        {
            return "";
        }
    }
}