using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JaroWinklerRecordSearch.Model;
using JetBrains.Annotations;

namespace JaroWinklerRecordSearch.Helper
{
    [PublicAPI]
    public static class NormalizationExtensions
    {
        public static Regex NumRegex = new("[0-9]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static Regex AlphaNumRegex = new("[0-9a-z]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static Regex NotAlphaNumRegex = new("[^0-9a-z]", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static Regex NotAlphaNumExtRegex =
            new("[^0-9a-z\\-_\\.'+]", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static bool UseSpecialChars(this string name) => NotAlphaNumExtRegex.IsMatch(name);
        public static bool UseSpecialChars(this IEnumerable<string> parts) => parts.Any(UseSpecialChars);

        public static string MyNormalize(this string name)
        {
            var rd = name.RemoveDiacritics();
            return rd.ToLowerInvariant();
        }

        public static (string[] origs, string[] norms) ToNameParts(string nameNat, string name)
        {
            var origs = nameNat.MySplit();
            var norms = origs.Select(n => n.MyNormalize()).ToArray();
            if (!norms.Any() || norms.UseSpecialChars())
            {
                origs = name.MySplit();
                norms = origs.Select(n => n.MyNormalize()).ToArray();
            }

            if (origs.Length != norms.Length) throw new InvalidOperationException();
            return (origs, norms);
        }

        public static (string[] origs, string[] norms) ToOrgCodeParts(string name)
        {
            var origs = name.MySplit();
            var norms = origs.Select(n => n.MyNormalize()).ToArray();
            if (origs.Length != norms.Length) throw new InvalidOperationException();
            return (origs, norms);
        }

        public static IndexDoc ToScdIndexDoc(this ScdPerson scdPerson, Func<(string orig, string norm), Term> termResolver, Func<string,int> orgCodeResolver)
        {
            var (ofns, nfns) = ToNameParts(scdPerson.FirstNameNat, scdPerson.FirstName);
            var (olns, nlns) = ToNameParts(scdPerson.LastNameNat, scdPerson.LastName);
            var tplf = ofns.Zip(nfns, (a,b)=>(a,b))
                .Select((p,i) => new TidFieldPos(termResolver(p).Id, FieldEnum.FirstName, i));
            var tpll = olns.Zip(nlns, (a,b)=>(a,b))
                .Select((p, i) => new TidFieldPos(termResolver(p).Id, FieldEnum.LastName, i));
            var tplo = new []{new TidFieldPos(orgCodeResolver(scdPerson.OrgCode), FieldEnum.OrgCode, 0)};
            var tpl = tplf.Concat(tpll).Concat(tplo).ToArray();
            var scdIndexDoc = new IndexDoc(scdPerson.Id, tpl);
            return scdIndexDoc;
        }

        // ReSharper disable once UseDeconstructionOnParameter
        public static IndexDoc ToOrgCodeIndexDoc(this (int id, string name) orgCode, Func<(string orig, string norm), Term> termResolver)
        {
            var (oocs, nocs) = ToOrgCodeParts(orgCode.name);
            var tpl = oocs.Zip(nocs, (a,b)=>(a,b))
                .Select((p, i) => new TidFieldPos(termResolver(p).Id, FieldEnum.OrgCode, i));
            var orgCodeIndexDoc = new IndexDoc(orgCode.id, tpl.ToArray());
            return orgCodeIndexDoc;
        }

        public static string[] MySplit(this string text)
        {
            var ns = text.Split(new[] { " ", "(", ")", "/", "-", "_", ".", "+" }, StringSplitOptions.RemoveEmptyEntries);
            return ns;
        }

        public static IEnumerable<SearchTerm> SearchTermSplit(this string text)
        {
            var nsts = text.MySplit().Select((t,i) => new SearchTerm(i, t, t.MyNormalize()));
            return nsts;
        }

    }
}