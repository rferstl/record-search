<Query Kind="Program">
  <Reference Relative="..\JaroWinklerRecordSearch\bin\Debug\JaroWinklerRecordSearch.dll">C:\Users\z00017ol\source\repos\JaroWinklerRecordSearch\JaroWinklerRecordSearch\bin\Debug\JaroWinklerRecordSearch.dll</Reference>
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

void Main_()
{
	DiacriticsExtensions.ForeignCharacters.SelectMany(e => e.Key).GroupBy(x => x, (x, g) => (x, c: g.Count())).Where(x => x.c > 1).Dump("double");
}
void Main()
{
	var mySearch = new MySearch();
	//mySearch.Search("rann nico").Dump();
	SearchBox(mySearch);
	//mySearch._searchIndex.PreSelectTermIds("rann").Select(rts => new { Term= mySearch._searchIndex.Terms[rts.tid].Orig, rts.u, rts.umax}).Dump("PreSelectTermIds", 0);
}

void SearchBox(MySearch mySearch)
{
	//AdHocTest();
	"Search:".Dump();
	var searchBox = new TextBox().Dump();
	var keepRunning = Util.KeepRunning();
	var button = new Button("Stop", onClick: _ => keepRunning.Dispose()).Dump();
	var dc1 = new DumpContainer().Dump();
	var dc2 = new DumpContainer().Dump();
	searchBox.TextInput += (sender, args) => (dc1.Content, dc2.Content) = Perf(() => mySearch.Search(searchBox.Text));

	dc1.Content = "";
	dc2.Content = mySearch.Search(searchBox.Text);
	searchBox.Focus();
}

public (string, T) Perf<T>(Func<T> func)
{
	var t0 = Util.ElapsedTime;
	var result = func();
	var elapsed = Math.Round((Util.ElapsedTime - t0).TotalMilliseconds, 2);
	return ($"{elapsed} ms", result);
}
}
namespace MyJaroWinklerRecordSearch{

	public class MySearch
	{
		public static double UnmatchTolerance = 0.75;
		public static double TresholdScore = 0.85;

		public SearchIndex SearchIndex { get; }
		public IDictionary<int, ScdPerson> ScdNameDict { get; }
		
		public MySearch()
		{
			var scdNamesAll = MySearch.LoadScdNamesAll();
			ScdNameDict = scdNamesAll.ToDictionary(e => e.Id);
			SearchIndex = SearchIndex.Build(UnmatchTolerance, TresholdScore);
		}
		
		public (int, IList<MySearchResult>) Search(string searchExpr)
		{
			var searchTerms = searchExpr.SearchTermSplit().Where(t => t.Norm.Length >= 3);
			if (!searchTerms.Any())
				return (0, Array.Empty<MySearchResult>());
			var termScores = searchTerms.SelectMany(searchTerm => SearchIndex.SearchTermScore(searchTerm));
			termScores.Select(rts => new { Term= SearchIndex.Terms[rts.Tid].Orig, rts.StId, rts.Tid, rts.Score}).Dump("resultTermScores", 0);

			var searchResults = SearchIndex.Find(termScores);
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
			var doc = ScdNameDict[docScoreInfo.DocId];
		var text = $"{doc.LastName}, {doc.FirstName} ({doc.Gid} {docScoreInfo.DocId}) {doc.OrgCode}";
			var score = RecordSearch.ToTotalScore(docScoreInfo.FieldPosTidStIdScores);
			var sm = RecordSearch.ToMatrix2(docScoreInfo.FieldPosTidStIdScores);
			var smDateTable = (sm.RowKeys, sm.ColKeys, sm.M).ToDataTable(stId => $"{stId}"); // $"{_searchIndex.Terms[tidStId.tid]}.{tidStId.stId}");
			//var info = new string[] { $"{scoreMatrix.IsAmbiguousScoreMatrix()}" };
			return new MySearchResult(text, score, docScoreInfo.FieldPosTidStIdScores, smDateTable);
		}

		public static IEnumerable<ScdPerson> LoadScdNamesAll()
			=> JsonlExtensions.LoadFromJsonLines<ScdPerson>("scd_all.jsonl");
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

	public readonly struct MyScoreInfo
	{
		public MyScoreInfo(FieldPosTidStIdScore[] fieldPosTidStIdScore, Term[] termInfos)
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

	public static class MySearchExtensions
	{
		
	}
}

class EOF {