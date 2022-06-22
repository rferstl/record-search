using System;
using System.Collections.Generic;
using System.Linq;
using JaroWinklerRecordSearch.Helper;
using JaroWinklerRecordSearch.Model;
using JetBrains.Annotations;

namespace JaroWinklerRecordSearch
{
    [PublicAPI]
    public class SearchIndex
    {
        public double UnmatchTolerance { get; } // 0.8;
        public double TresholdScore { get; } // 0.9;

        public IDictionary<string, int> BitArrayCharMap { get; }
        public IList<Segment> Segments { get; }
        public IDictionary<int, Term> Terms { get; }
        public IDictionary<int, IList<TidFieldPos>> Docs { get; }
        public IList<IList<DocTidFieldPos>> Index { get; }

        public SearchIndex(IDictionary<string, int> bitArrayCharMap, IList<Segment> segments,
            IDictionary<int, Term> terms, IDictionary<int, IList<TidFieldPos>> docs, IList<IList<DocTidFieldPos>> index, 
            double unmatchTolerance = 0.8, double tresholdScore = 0.9)
        {
            BitArrayCharMap = bitArrayCharMap;
            Segments = segments;
            Terms = terms;
            Docs = docs;
            Index = index;
            UnmatchTolerance = unmatchTolerance;
            TresholdScore = tresholdScore;
        }

        public static SearchIndex Build(double unmatchTolerance = 0.8, double tresholdScore = 0.9)
        {
            var terms = LoadTerms();
            var docs = LoadScdDocs();
            var index = docs.InvertedIndex(terms.Count);
            var searchIndex = new SearchIndex(
                bitArrayCharMap: LoadBitArrayCharMap(),
                segments: LoadSegments().ToArray(),
                terms: terms,
                docs: docs,
                index: index,
                unmatchTolerance: unmatchTolerance,
                tresholdScore: tresholdScore
            );
            return searchIndex;
        }

        public static IDictionary<string,int> LoadBitArrayCharMap()
            => JsonlExtensions.LoadFromJsonLines<KeyValuePair<string, int>>("bitarray_charmap.jsonl").ToDictionary(e => e.Key, e => e.Value);

        public static IEnumerable<Segment> LoadSegments()
            => JsonlExtensions.LoadFromJsonLines<Segment>("segements.jsonl");

        public static IDictionary<int,Term> LoadTerms()
            => JsonlExtensions.LoadFromJsonLines<Term>("scd_terms.jsonl").ToDictionary(e => e.Id);

        public static IDictionary<int,IList<TidFieldPos>> LoadScdDocs()
            => JsonlExtensions.LoadFromJsonLines<IndexDoc>("scd_docs.jsonl").ToDictionary(e => e.Id, e => e.Tpl);

        public IEnumerable<StIdTidScore> SearchTermScore(SearchTerm searchTerm)
        {
            var (stId, stOrig, stNorm) = searchTerm;
            if (stNorm.Length == 0)
                return Array.Empty<StIdTidScore>();

            var tids = PreSelectTermIds(stNorm).ToArray();

            var origTerms = tids.Select(tid => (orig: Terms[tid.tid].Orig, tid.tid, tid.u, tid.umax));

            var pw = stNorm.Length >= 4 ? 0.2
                : stNorm.Length == 3 ? 0.3
                : stNorm.Length == 2 ? 0.4
                : stNorm.Length == 1 ? 0.5
                : throw new InvalidOperationException();
            
            var result = origTerms //.AsParallel().WithDegreeOfParallelism(4).WithMergeOptions(ParallelMergeOptions.FullyBuffered)
                .Select(term => new StIdTidScore(stId, term.tid, JaroWinkler(stOrig, term.orig, pw)))
                .Where(r => r.Score >= TresholdScore);

            return result.OrderByDescending(r => r.Score);
        }

        public IEnumerable<(int tid, long u, long umax)> PreSelectTermIds(string nst)
        {
            if (string.IsNullOrEmpty(nst))
                yield break;

            var nstBitmask = BitArrayCharMap.StringToBitmask(nst);
            foreach (var s in Segments.Where(x => x.First == nst.First()))
            {
                var unmatchCharsMax = nst.Length * (nst.Length + s.Len * (2 - 3 * UnmatchTolerance)) /
                                      (nst.Length + s.Len);
                var unmatchCharsMaxLong = (int)Math.Floor(Math.Max(unmatchCharsMax, 0.0d));
                for (var i = 0; i < s.Bitmaps.Count; i++)
                {
                    var v = nstBitmask & ~s.Bitmaps[i];
                    var unmatchChars = BitOperations.BitCount64(v);
                    //var u = (long)Popcnt.X64.PopCount(v);
                    if (unmatchChars <= unmatchCharsMaxLong)
                        yield return (s.TermIds[i], unmatchChars, unmatchCharsMaxLong);
                }
            }
        }

        public static double JaroWinkler(string s1, string s2, double pw = 0.1)
        {
            var jaroWinklerSim = JaroWinklerUtils.JaroWinklerSim(s1.ToLowerInvariant(), s2.ToLowerInvariant(),
                JaroWinklerUtils.DiacriticCharSim, pw);
            return jaroWinklerSim;
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