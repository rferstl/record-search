using JaroWinklerRecordSearch.Model;
using JetBrains.Annotations;

namespace JaroWinklerRecordSearch
{
    [PublicAPI]
    public readonly struct ScoreInfo
    {
        public ScoreInfo(FieldPosTidStIdScore[] fieldPosTidStIdScore, Term[] termInfos)
        {
            FieldPosTidStIdScore = fieldPosTidStIdScore;
            TermInfos = termInfos;
        }

        public FieldPosTidStIdScore[] FieldPosTidStIdScore { get; }
        public Term[] TermInfos { get; }

        public object ToDumpX()
        {
            return "";
        }
    }
}