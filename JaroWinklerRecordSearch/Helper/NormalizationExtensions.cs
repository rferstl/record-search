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

        public static IEnumerable<(string orig, string norm)> ToTermPairs(this NameParts<string> nps)
        {
            var ots = nps.OriginalFirstNames.Concat(nps.OriginalLastNames);
            var nts = nps.NormalizedFirstNames.Concat(nps.NormalizedLastNames);
            return ots.Zip(nts, (a,b)=>(a,b));
        }

        public static NameParts<string> ToNameParts(this ScdPerson scdPerson)
        {
            var (ofns, nfns) = ToNameParts(scdPerson.FirstNameNat, scdPerson.FirstName);
            var (olns, nlns) = ToNameParts(scdPerson.LastNameNat, scdPerson.LastName);
            return new NameParts<string>(scdPerson.Id, nfns, ofns, nlns, olns);
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

        public static ScdDoc ToScdDoc(this NameParts<string> nps, Func<(string orig, string norm), Term> resolver)
        {
            var tplf = nps.OriginalFirstNames.Zip(nps.NormalizedFirstNames, (a,b)=>(a,b))
                .Select((p,i) => new TidFieldPos(resolver(p).Id, FieldEnum.FirstName, i));
            var tpll = nps.OriginalLastNames.Zip(nps.NormalizedLastNames, (a,b)=>(a,b))
                .Select((p, i) => new TidFieldPos(resolver(p).Id, FieldEnum.LastName, i));
            var tpl = tplf.Concat(tpll).ToArray();
            return new ScdDoc(nps.Id, tpl);
        }

        public static ScdDoc ToScdDoc(this ScdPerson scdPerson, Func<(string orig, string norm), Term> resolver)
            => scdPerson.ToNameParts().ToScdDoc(resolver);

        public static string[] MySplit(this string text)
        {
            var ns = text.Split(new[] { " ", "(", ")", "/", "-", "_", "." }, StringSplitOptions.RemoveEmptyEntries);
            return ns;
        }

        public static SearchTerm[] SearchTermSplit(this string text)
        {
            var nsts = text.MySplit().Select((t,i) => new SearchTerm(i, t, t.MyNormalize())).ToArray();
            return nsts;
        }
    }
}