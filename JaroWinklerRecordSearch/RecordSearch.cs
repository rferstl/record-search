using System;
using System.Collections.Generic;
using System.Linq;
using JaroWinklerRecordSearch.Helper;
using JaroWinklerRecordSearch.Model;
using JaroWinklerRecordSearch.MyMath;
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

		public IEnumerable<SearchResult> Search(string searchExpr)
		{
			var searchTerms = searchExpr.SearchTermSplit().Where(t => t.Norm.Length >= 3).ToArray();
			if (!searchTerms.Any())
				return Array.Empty<SearchResult>();
			var stIdTidScores = searchTerms.SelectMany(searchTerm => SearchIndex.SearchTermScore(searchTerm));

			var searchResults = SearchIndex.Find(stIdTidScores);

			var result = searchResults
                .Select(ToSearchResult)
                .OrderByDescending(sr => sr.Score);
			return result;
		}
		
		public SearchResult ToSearchResult(DocFieldPosTidStIdScores docScoreInfo)
		{
            var score = ToTotalScore(docScoreInfo.FieldPosTidStIdScores);
			return new SearchResult(docScoreInfo.DocId, score);
		}

        public static double ToTotalScore(FieldPosTidStIdScore[] scoreInfo)
        {
            var m = ToMatrix(scoreInfo);
            var mk2 = Munkres2.Maximize(m);
            return -mk2.Value;
        }

        public static double[,] ToMatrix(FieldPosTidStIdScore[] scoreInfo)
		{
			var rowKeys = scoreInfo.Select(si => si.FieldPos).Distinct().ToArray();
			var colKeys = scoreInfo.Select(si => si.StId).Distinct().ToArray();
			var m = new double[rowKeys.Length, colKeys.Length];
			foreach (var si in scoreInfo)
			{
				var r = Array.IndexOf(rowKeys, si.FieldPos);
				var c = Array.IndexOf(colKeys, si.StId);
				m[r, c] = Math.Max(si.Score, m[r, c]);
			}
			return m;
		}

        public static ScoreMatrix ToMatrix2(FieldPosTidStIdScore[] scoreInfo)
        {
            var rowKeys = scoreInfo.Select(si => si.FieldPos).Distinct().ToArray();
            var colKeys = scoreInfo.Select(si => si.StId).Distinct().ToArray();
            var m = new double[rowKeys.Length, colKeys.Length];
            foreach (var si in scoreInfo)
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
