using System.Collections.Generic;
using System.Diagnostics.Contracts;
using JetBrains.Annotations;

namespace JaroWinklerRecordSearch.Model
{
    [PublicAPI]
    public class TermIndex
    {
        public IDictionary<(string orig, string norm), Term> TermByString =
            new Dictionary<(string orig, string norm), Term>();

        public IDictionary<int, Term> TermById = new Dictionary<int, Term>();

        public Term AddOrGetTerm((string orig, string norm) tkey)
        {
            if (TermByString.TryGetValue(tkey, out var term))
                return term;
            Contract.Assert(TermByString.Count == TermById.Count);
            var tid = TermById.Count;
            var newTerm = new Term(tid, tkey.orig, tkey.norm);
            Add(newTerm);
            return newTerm;
        }

        private void Add(Term term)
        {
            TermById.Add(term.Id, term);
            TermByString.Add((term.Orig, term.Norm), term);
        }

        public ICollection<Term> AllTerms => TermById.Values;
    }
}