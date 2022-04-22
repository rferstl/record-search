using System;
using System.Collections.Generic;
using System.Linq;
using JaroWinklerRecordSearch.Helper;
using JaroWinklerRecordSearch.Model;
using JetBrains.Annotations;

namespace JaroWinklerRecordSearch
{
    [PublicAPI]
    public class OrgCodeSearchIndex
    {
        public IDictionary<string, Term> TermDict { get; }
        public IDictionary<int, Term> Terms { get; }
        public IDictionary<int, IList<TidFieldPos>> Docs { get; }
        public IList<IList<int>> Index { get; }

        public OrgCodeSearchIndex(IDictionary<string,Term> termDict, IDictionary<int, Term> terms, IDictionary<int, IList<TidFieldPos>> docs, IList<IList<int>> index)
        {
            TermDict = termDict;
            Terms = terms;
            Docs = docs;
            Index = index;
        }

        public static OrgCodeSearchIndex Build()
        {
            var terms = LoadOrgCodeTerms();
            var termDict = terms.ToDictionary(e => e.Value.Norm, e => e.Value);
            var docs = LoadOrgCodeDocs();
            var index = InvertedIndex(docs, terms.Count);
            var searchIndex = new OrgCodeSearchIndex(
                termDict: termDict,
                terms: terms,
                docs: docs,
                index: index
            );
            return searchIndex;
        }

        public static IDictionary<int,Term> LoadOrgCodeTerms()
            => JsonlExtensions.LoadFromJsonLines<Term>("orgcode_terms.jsonl").ToDictionary(e => e.Id);

        public static IDictionary<int,IList<TidFieldPos>> LoadOrgCodeDocs()
            => JsonlExtensions.LoadFromJsonLines<IndexDoc>("orgcode_docs.jsonl").ToDictionary(e => e.Id, e => e.Tpl);

        public static IList<IList<int>> InvertedIndex(IDictionary<int, IList<TidFieldPos>> docs, int tidCount)
        {
            var gs = docs
                .SelectMany(d => d.Value.Select(tp => new KeyValuePair<int, int>(tp.Tid, d.Key)))
                .GroupBy(e => e.Key);
            var array = new IList<int>[tidCount];	
            foreach(var g in gs)
                array[g.Key] = g.Select(kv => kv.Value).ToArray();
            return array;
        }
        public IList<StIdTidScore> SearchTermScore(SearchTerm searchTerm)
        {
            var (stId, _, stNorm) = searchTerm;
            if (stNorm.Length == 0)
                return Array.Empty<StIdTidScore>();

            var term = TermDict.GetValueOrDefault(stNorm, Term.None);
            return new []{new StIdTidScore(stId, term.Id, 0.5) };
        }

    }
}