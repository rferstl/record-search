<Query Kind="Program">
  <Reference Relative="..\JaroWinklerRecordSearch\bin\Debug\JaroWinklerRecordSearch.dll">C:\Users\z00017ol\source\repos\JaroWinklerRecordSearch\JaroWinklerRecordSearch\bin\Debug\JaroWinklerRecordSearch.dll</Reference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>JaroWinklerRecordSearch</Namespace>
  <Namespace>JaroWinklerRecordSearch.Helper</Namespace>
  <Namespace>JaroWinklerRecordSearch.Model</Namespace>
  <Namespace>System.Diagnostics.Contracts</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
  <DisableMyExtensions>true</DisableMyExtensions>
</Query>

void Main()
{
	//ScdNamesToJsonLines();
	//IndexScdDocs();
	IndexOrgCodeDocs();
}

public static TermIndex NameTermRegistry = new TermIndex();
public static TermIndex OrgCodeTermRegistry = new TermIndex();
public static Dictionary<string,OrgCode> OrgCodeDict = new Dictionary<string,OrgCode>();

void ScdNamesToJsonLines()
{
	var scdNames = JsonlExtensions.LoadFromJson<ScdPerson[]>("scd_all.json");
	scdNames.SaveToJsonLines("scd_all.jsonl");
}

public void IndexScdDocs()
{
	var scdNames = JsonlExtensions.LoadFromJsonLines<ScdPerson>("scd_all.jsonl");
	
	var scdIndexDocs = scdNames
		.Select(e => e.ToScdIndexDoc(NameTermRegistry.AddOrGetTerm, orgCodeName => AddOrGetOrgCode(orgCodeName).Id + 1_000_000))
		.ToArray();
	//scdIndexDocs.Take(20).Dump("scdIndexDocs");

	scdIndexDocs.SaveToJsonLines("scd_docs.jsonl");

	NameTermRegistry.AllTerms.SaveToJsonLines("scd_terms.jsonl");
	OrgCodeDict.Values.SaveToJsonLines("orgcodes.jsonl");
}

public void IndexOrgCodeDocs()
{
	var orgCodes = JsonlExtensions.LoadFromJsonLines<OrgCode>("orgcodes.jsonl");
	var orgCodeIndexDocs = orgCodes
		.Select(e => e.ToTuple().ToOrgCodeIndexDoc(OrgCodeTermRegistry.AddOrGetTerm))
		.ToArray();

	orgCodeIndexDocs.SaveToJsonLines("orgcode_docs.jsonl");

	OrgCodeTermRegistry.AllTerms.SaveToJsonLines("orgcode_terms.jsonl");
}

public OrgCode AddOrGetOrgCode(string orgCodeName)
{
	orgCodeName = orgCodeName.ToUpper();
	if (OrgCodeDict.TryGetValue(orgCodeName, out var orgCode))
		return orgCode;
	var id = OrgCodeDict.Count;
	var newOrgCode = new OrgCode(id, orgCodeName);
	OrgCodeDict.Add(orgCodeName, newOrgCode);
	return newOrgCode;
}

public readonly struct OrgCode
{
	public int Id { get; }
	public string Name { get; }

	[JsonConstructor]
	public OrgCode(int id, string name)
	{
		Id = id;
		Name = name;
	}
	public (int id, string name) ToTuple() => (Id, Name);
}