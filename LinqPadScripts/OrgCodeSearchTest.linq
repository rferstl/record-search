<Query Kind="Program">
  <Reference>C:\Users\z00017ol\source\repos\JaroWinklerRecordSearch\JaroWinklerRecordSearch\bin\Debug\JaroWinklerRecordSearch.dll</Reference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>JaroWinklerRecordSearch</Namespace>
  <Namespace>JaroWinklerRecordSearch.Helper</Namespace>
  <Namespace>JaroWinklerRecordSearch.Model</Namespace>
  <Namespace>LINQPad.Controls</Namespace>
  <Namespace>MyJaroWinklerRecordSearch</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>System.Globalization</Namespace>
  <DisableMyExtensions>true</DisableMyExtensions>
</Query>

void Main()
{
	var mySearch = new MySearch();
	mySearch.Search("APD R&D").Dump();
}

}
namespace MyJaroWinklerRecordSearch
{

	public class MySearch
	{
		public OrgCodeSearchIndex OrgCodeSearchIndex { get; }
		public IDictionary<int, OrgCode> OrgCodeDict { get; }

		public MySearch()
		{
			var orgCodes = MySearch.LoadOrgCodes();
			OrgCodeDict = orgCodes.ToDictionary(e => e.Id);
			OrgCodeSearchIndex = OrgCodeSearchIndex.Build();
		}

		public (int, IList<MySearchResult>) Search(string searchExpr)
		{
			var searchTerms = searchExpr.SearchTermSplit().Where(t => t.Norm.Length >= 2);
			if (!searchTerms.Any())
				return (0, Array.Empty<MySearchResult>());
			var termScores = searchTerms.SelectMany(searchTerm => OrgCodeSearchIndex.SearchTermScore(searchTerm));
			termScores.Select(rts => new { Term = OrgCodeSearchIndex.Terms[rts.Tid].Orig, rts.StId, rts.Tid, rts.Score }).Dump("resultTermScores", 1);
			//termScores.Dump("resultTermScores", 1);

			var searchResults = OrgCodeSearchIndex.Find(termScores);
			searchResults.Dump("searchResults", 0);

			var result = searchResults
				//.Where(sr => sr.DocId == 245221 )
				.Select(ToSearchResult)
				.OrderByDescending(sr => sr.Score)
				.ToArray();

			return (result.Length, result.Take(20).ToArray());
		}

		public MySearchResult ToSearchResult(DocFieldPosTidStIdScores docScoreInfo)
		{
			var doc = OrgCodeDict[docScoreInfo.DocId];
			var text = $"{doc.Name} ({docScoreInfo.DocId})";
			var f = RecordSearch.ToTotalScore(docScoreInfo.FieldPosTidStIdScores);
			var score = f / (f + 1);
			var sm = RecordSearch.ToMatrix2(docScoreInfo.FieldPosTidStIdScores);
			var smDateTable = (sm.RowKeys, sm.ColKeys, sm.M).ToDataTable(stId => $"{stId}");
			return new MySearchResult(text, score, docScoreInfo.FieldPosTidStIdScores, smDateTable);
		}

		public static IEnumerable<OrgCode> LoadOrgCodes()
			=> JsonlExtensions.LoadFromJsonLines<OrgCode>("orgcodes.jsonl");
	}

	public readonly struct MySearchResult
	{
		public MySearchResult(string text, double score, FieldPosTidStIdScore[] scoreInfo, DataTable scoreMatrix)
		{
			Text = text;
			Score = score;
			ScoreInfo = scoreInfo;
			ScoreMatrix = scoreMatrix;
		}

		public string Text { get; }
		public double Score { get; }
		public FieldPosTidStIdScore[] ScoreInfo { get; }
		public DataTable ScoreMatrix { get; }
	}

}

class EOF {