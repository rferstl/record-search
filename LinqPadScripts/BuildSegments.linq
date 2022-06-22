<Query Kind="Program">
  <Reference>C:\Users\z00017ol\source\repos\JaroWinklerRecordSearch\JaroWinklerRecordSearch\bin\Debug\JaroWinklerRecordSearch.dll</Reference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>JaroWinklerRecordSearch</Namespace>
  <Namespace>JaroWinklerRecordSearch.Helper</Namespace>
  <Namespace>JaroWinklerRecordSearch.Model</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>System.Globalization</Namespace>
  <DisableMyExtensions>true</DisableMyExtensions>
</Query>

void Main()
{
	//AdHocTest();
	BuildSegmentsMain();
}

void AdHocTest()
{
	var mockBitArrayCharMap = (Enumerable.Range(0, 26)
			.Select(i => Convert.ToChar('a' + i))
			.SelectMany(c => new[] { $"{c}1", $"{c}2" }))
		.Concat(Enumerable.Range(0, 10)
			.Select(i => Convert.ToChar('0' + i))
			.Select(c => $"{c}1"))
		.Concat(new[] {"-1", "_1"})
		.Select((x, i) => (x, i))	
		.ToDictionary(x => x.x, x => x.i)
		//.Dump("mockBitArrayCharMap")
		;
	var s = "christoph";
	var charList = s.MyNormalize().ToCharList();
	var bitmask = mockBitArrayCharMap.ToBitArray64(charList).ToUInt64();
	Convert.ToString((long)bitmask, 2).Dump($"bitmask {s}");
	charList.Dump("charList");
}

void BuildSegmentsMain()
{
	var terms = JsonlExtensions.LoadFromJsonLines<Term>("scd_terms.jsonl");
	
	var topUsedChars = terms
		.Select(t => t.Norm)
		.Select(n => n.ToCharList())
		.Aggregate(Seed(), (acc, e) => acc.Accumulate(e))
		.OrderByDescending(e => e.Value)
		.ToArray();
		
	var bitArrayCharMap = topUsedChars.Take(64).Select((s, i) => (k:s.Key, i)).ToDictionary(x => x.k, x => x.i);
	//bitArrayCharMap.Dump("bitArrayCharMap");
	bitArrayCharMap.SaveToJsonLines("bitarray_charmap.jsonl");

	var segments = terms
		.GroupBy(t => (first: t.Norm.First(), len: t.Norm.Length), (e, g) => BuildSegment(e, bitArrayCharMap, g))
		;

	//segments.Count.Dump("segments count");
	//segments.Take(1000).Dump("segments", 2);
	segments.SaveToJsonLines("segements.jsonl");

}

public IDictionary<string, int> Seed() => new Dictionary<string, int>();

public Segment BuildSegment((char first, int len) sid, IDictionary<string, int> bitArrayCharMap, IEnumerable<Term> grp)
{
	var termIds = new List<int>();
	var bitmaps = new List<ulong>();
	foreach(var e in grp)
	{
		var charList = e.Norm.ToCharList();
		var bitmask = bitArrayCharMap.ToBitArray64(charList).ToUInt64();
		termIds.Add(e.Id);
		bitmaps.Add(bitmask);
	}
	return new Segment(sid.first, sid.len, termIds, bitmaps);
}