using System;
using System.Collections.Generic;
using System.Linq;
using JaroWinklerRecordSearch.Helper;
using JaroWinklerRecordSearch.Model;
using JetBrains.Annotations;

namespace JaroWinklerRecordSearch
{
    [PublicAPI]
	public class RecordSearch
	{
		public static double UnmatchTolerance = 0.8;
		public static double TresholdScore = 0.85;

		public SearchIndex SearchIndex;
		public IDictionary<int, ScdPerson> ScdNameDict;

        public RecordSearch()
        {
            var scdNamesAll = LoadScdNamesAll();
            ScdNameDict = scdNamesAll.ToDictionary(e => e.Id);
            SearchIndex = SearchIndex.Build(UnmatchTolerance, TresholdScore);
        }

		public (int, IList<SearchResult>) Search(string searchExpr)
		{
			var searchTerms = searchExpr.SearchTermSplit().Where(t => t.Norm.Length >= 3).ToArray();
			if (!searchTerms.Any())
				return (0, Array.Empty<SearchResult>());
			var resultTermScores = searchTerms.SelectMany(searchTerm => SearchIndex.SearchTermScore(searchTerm));
			//resultTermScores.Dump("resultTermScores", 1);

			var scdIds = SearchIndex.Find2(resultTermScores);
			//scdIds.Dump("scdIds", 1);

			var merged = scdIds.MergeBy(x => x.Doc, Aggregate);
			var result = merged
				.Select(ToSearchResult)
				.OrderByDescending(x => x.Score)
				.ToArray();

			return (result.Length, result.Take(20).ToArray());
		}
		
		public static DocStTidScores Aggregate(int docId, IEnumerable<DocStTidScores> xs) =>
            new(docId, xs.SelectMany(ts => ts.StTidScores).ToArray());

		public SearchResult ToSearchResult(DocStTidScores dts)
		{
			var doc = ScdNameDict[dts.Doc];
			var text = $"{doc.LastName}, {doc.FirstName} ({doc.Gid} {dts.Doc}) {doc.OrgCode}";
			var scoreInfo = ToScoreInfo(dts);
			//var sm = ToMatrix(scoreInfo);
            var score = dts.StTidScores.Sum(d => d.Score);
			return new SearchResult(text, score, scoreInfo);
		}

		public ScoreInfo ToScoreInfo(DocStTidScores dts)
		{
			var idxDoc = SearchIndex.Docs[dts.Doc];
			var tids = dts.StTidScores.Select(t => t.Tid).Distinct().ToArray();
			var docPosInfos = tids.SelectMany(tid => idxDoc.Where(d => d.Tid == tid)).ToArray();
			var info = docPosInfos.Join(dts.StTidScores, outer => outer.Tid, inner => inner.Tid, (tfp,sts) => new FieldPosTidStIdScore(tfp.FieldPos, tfp.Tid, sts.StId, sts.Score)).ToArray();
			var termInfos = tids.Select(tid => SearchIndex.Terms[tid]).ToArray();
			return new ScoreInfo(info, termInfos);
		}

		public ScoreMatrix ToMatrix(ScoreInfo info)
		{
			var rowKeys = info.FieldPosTidStIdScore.Select(si => si.FieldPos).Distinct().ToArray();
			var colKeys = info.FieldPosTidStIdScore.Select(si => si.StId).Distinct().ToArray();
			var m = new double[rowKeys.Length, colKeys.Length];
			foreach (var si in info.FieldPosTidStIdScore)
			{
				var r = Array.IndexOf(rowKeys, si.FieldPos);
				var c = Array.IndexOf(colKeys, si.StId);
				m[r, c] = Math.Max(si.Score, m[r, c]);
			}
			return new ScoreMatrix(rowKeys, colKeys, m);

		}

		public static IEnumerable<ScdPerson> LoadScdNamesAll()
			=> JsonlExtensions.LoadFromJsonLines<ScdPerson>("scd_all.jsonl");
	}
    public readonly struct SearchResult
    {
        public SearchResult(string text, double score,  ScoreInfo scoreInfo)
        {
            Text = text;
            Score = score;
            ScoreInfo = scoreInfo;
        }

        public string Text { get; }
        public double Score { get; }
        public ScoreInfo ScoreInfo { get; }
    }

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

    public readonly struct ScoreMatrix
    {
        public ScoreMatrix(FieldPos[] rowKeys, int[] colKeys, double[,] m)
        {
            RowKeys = rowKeys;
            ColKeys = colKeys;
            M = m;
        }

        public FieldPos[] RowKeys { get; }
        public int[] ColKeys { get; }
        public double[,] M { get; }

        public bool IsAmbiguousScoreMatrix()
        {
            var (nRows, nCols) = (M.GetLength(0), M.GetLength(1));
            var rowHasValue = new bool[nRows];
            var colHasValue = new bool[nCols];
            for (var r = 0; r < nRows; r++)
            {
                for (var c = 0; c < nCols; c++)
                {
                    if (M[r, c] == 0.0)
                        continue;
                    if (rowHasValue[r] || colHasValue[c])
                        return false;
                    rowHasValue[r] = true;
                    colHasValue[c] = true;
                }
            }
            return true;
        }
		
        public void Deconstruct(out FieldPos[] rowKeys, out int[] colKeys, out double[,] m)
        {
            rowKeys = RowKeys;
            colKeys = ColKeys;
            m = M;
        }
    }

}
