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
        public IList<IList<DocTidFieldPos>> Index { get; }

        public OrgCodeSearchIndex(IDictionary<string,Term> termDict, IDictionary<int, Term> terms, IDictionary<int, IList<TidFieldPos>> docs, IList<IList<DocTidFieldPos>> index)
        {
            TermDict = termDict;
            Terms = terms;
            Docs = docs;
            Index = index;
        }

        public static OrgCodeSearchIndex Build()
        {
            var terms = LoadOrgCodeTerms();
            var termDict = terms.ToDictionaryFirst(e => e.Value.Norm, e => e.Value);
            var docs = LoadOrgCodeDocs();
            var index = docs.InvertedIndex(terms.Count);
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

        public IList<StIdTidScore> SearchTermScore(SearchTerm searchTerm)
        {
            var (stId, _, stNorm) = searchTerm;
            if (stNorm.Length == 0)
                return Array.Empty<StIdTidScore>();

            if (!TermDict.TryGetValue(stNorm, out var result))
                return Array.Empty<StIdTidScore>();

            return new[] { new StIdTidScore(stId, result.Id, 1) };
        }


        public IEnumerable<DocFieldPosTidStIdScores> Find(IEnumerable<StIdTidScore> stTidScores)
        {
            static DocFieldPosTidStIdScores Aggregate(int docId, IEnumerable<DocFieldPosTidStIdScore> xs) =>
                new(docId, xs.Select(x => x.FieldPosTidStIdScore).ToArray());

            var docLists = stTidScores.SelectMany(t => Index[t.Tid].Select(d => new DocFieldPosTidStIdScore(d, t)));
            var byDocId = docLists.GroupBy(d => d.DocId, Aggregate);
            return byDocId;
        }
    }
}