<Query Kind="Program">
  <Reference Relative="..\JaroWinklerRecordSearch\bin\Debug\JaroWinklerRecordSearch.dll">C:\Users\z00017ol\source\repos\JaroWinklerRecordSearch\JaroWinklerRecordSearch\bin\Debug\JaroWinklerRecordSearch.dll</Reference>
  <Namespace>JaroWinklerRecordSearch.Helper</Namespace>
  <Namespace>JaroWinklerRecordSearch</Namespace>
</Query>

void Main()
{
	var UnmatchTolerance = 0.75;
	var nst = "rann".Dump();
	var s = "rander".Dump();
	var unmatchCharsMax1 = nst.Length * (nst.Length + s.Length * (2 - 3 * UnmatchTolerance)) / (nst.Length + s.Length);
	unmatchCharsMax1.Dump("unmatchCharsMax1");
	var unmatchCharsMax2 = s.Length * (s.Length + nst.Length * (2 - 3 * UnmatchTolerance)) / (s.Length + nst.Length);
	unmatchCharsMax2.Dump("unmatchCharsMax2");

	var bitArrayCharMap = SearchIndex.LoadBitArrayCharMap();
	var nstBitmask = bitArrayCharMap.StringToBitmask(nst);
	var sBitmask = bitArrayCharMap.StringToBitmask(s);

	var b1 = nstBitmask;
	var b2 = sBitmask;
	var v = b1 & ~b2;
	bitArrayCharMap.ToCharString().DumpFixed();
	Convert.ToString((long)b1, 2).PadLeft(64, '0').DumpFixed();
	Convert.ToString((long)b2, 2).PadLeft(64, '0').DumpFixed();
	Convert.ToString((long)~b2, 2).PadLeft(64, '0').DumpFixed();
	Convert.ToString((long)v, 2).PadLeft(64, '0').DumpFixed();

	var unmatchChars = BitOperations.BitCount64(v);
	unmatchChars.Dump("unmatchChars");
}

public static class BitArrayCharMapExtensions
{
	public static string ToCharString(this IDictionary<string, int> bitArrayCharMap)
	{
		return string.Concat(bitArrayCharMap.Keys.Select(k => $"{k[0]}".ToUpper()).Reverse());
	}

	public static T DumpFixed<T>(this T toDump, string heading = null, int? depth = null)
	{
		Util.WithStyle(toDump, "font-family:consolas; font-size: 120%;").Dump(heading, depth);
		return toDump;
	}
}