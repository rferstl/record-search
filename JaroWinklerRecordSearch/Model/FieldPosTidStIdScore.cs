using JetBrains.Annotations;

namespace JaroWinklerRecordSearch.Model
{
    [PublicAPI]
    public readonly struct FieldPosTidStIdScore
    {
        public FieldPosTidStIdScore(FieldPos fieldPos, int tid, int stId, double score)
        {
            FieldPos = fieldPos;
            Tid = tid;
            StId = stId;
            Score = score;
        }

        public FieldPos FieldPos { get; }
        public int Tid { get; }
        public int StId { get; }
        public double Score { get; }

        public object ToDumpX()
        {
            return "";
        }
    }

    [PublicAPI]
    public readonly struct DocFieldPosTidStIdScore
    {
        public DocFieldPosTidStIdScore(DocTidFieldPos docTidFieldPos, StTidScore stTidScore)
            : this(docTidFieldPos.DocId, new FieldPosTidStIdScore(docTidFieldPos.FieldPos, stTidScore.Tid, stTidScore.StId, stTidScore.Score))
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
