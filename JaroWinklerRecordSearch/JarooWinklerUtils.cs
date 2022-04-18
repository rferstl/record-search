using System;
using System.Linq;
using JetBrains.Annotations;

namespace JaroWinklerRecordSearch
{
    [PublicAPI]
    public static class JarooWinklerUtils
    {

        public static (double, string, string) DebugJaroWinkler(string s1, string s2, double pw = 0.1)
        {
            var (commonChars, transCount /*, matches*/) = MatchingCharacters(s1, s2, DiacriticCharSim);
            var prefixLen = PrefixLen(s1, s2, DiacriticCharSim);
            var debug1 = $"common_chars: {commonChars}, PrefixLen: {prefixLen}, trans_count: {transCount}";

            var jaroSim = JaroSim(s1, s2, DiacriticCharSim);
            var jaroWinklerSim = JaroWinklerSim(s1, s2, DiacriticCharSim, pw);
            var debug2 = $"j_sim: {jaroSim}, jw_sim: {jaroWinklerSim}";

            return (jaroWinklerSim, debug1, debug2);
        }

        public static bool Ceq(char c1, char c2, int kind) => c1 == c2;

        public static int CharSimAll(char c1, char c2, Func<char, char, int, bool> charSim)
        {
            return Enumerable.Range(1, 3).FirstOrDefault(a => charSim(c1, c2, a));
        }

        private static double TransCount(string s1, string s2, byte[] fs1, byte[] fs2, Func<char, char, int, bool> charSim)
        {
            var transCount = 0.0;
            var k = 0;
            for (var i = 0; i < fs1.Length; i++)
            {
                if (fs1[i] > 0)
                {
                    while (fs2[k] == 0)
                        k += 1;
                    var a = CharSimAll(s1[i], s2[k], charSim);
                    if (a == 0)
                        transCount += 1;
                    k += 1;
                }
            }
            return transCount / 2;
        }

        //public static (double common_chars, double t_count, IList<(int i, int j, int a)> matches ) MatchingCharacters(string s1, string s2, Func<char, char, int, bool>? charSim = default)
        public static (double commonChars, double transCount) MatchingCharacters(string s1, string s2, Func<char, char, int, bool> charSim)
        {
            var len1 = s1.Length;
            var len2 = s2.Length;

            var maxDist = Math.Max(Math.Max(len1, len2) / 2 - 1, 0);
            var fs1 = new byte[len1];
            var fs2 = new byte[len2];

            var commonChars = 0.0;
            //var matches = new List<(int i, int j, int a)>();

            for (var a = 1; a <= 3; a++)
            {
                for (var i = 0; i < s1.Length; i++)
                {
                    var start = Math.Max(0, i - maxDist);
                    var end = Math.Min(len2, i + maxDist + 1);
                    if (end <= start)
                        continue;
                    for (var j = start; j < end; j++)
                    {
                        if (fs1[i] > 0 || fs2[j] > 0)
                            continue;

                        if (charSim(s2[j], s1[i], a))
                        {
                            fs1[i] = fs2[j] = (byte)a;
                            //matches.Add((i, j, a));
                            commonChars += 1.0 / a;
                            break;
                        }
                    }
                }
            }

            if (commonChars == 0.0)
                return (commonChars, 0.0 /*, matches*/);

            var transCount = TransCount(s1, s2, fs1, fs2, charSim);

            return (commonChars, transCount /*, matches*/);
        }

        public static double JaroSim(string s1, string s2, Func<char, char, int, bool> charSim)
        {
            var (commonChars, tCount /*, matches*/) = MatchingCharacters(s1, s2, charSim);
            if (commonChars == 0.0)
                return 0.0;
            var jSim =
                (commonChars / s1.Length + commonChars / s2.Length + (commonChars - tCount) / commonChars) / 3;
            return jSim;
        }

        public static double PrefixLen(string s1, string s2, Func<char, char, int, bool> charSim)
        {
            return Enumerable.Range(0, new[] { s1.Length, s2.Length, 4 }.Min())
                .Select(i => CharSimAll(s1[i], s2[i], charSim))
                .TakeWhile(a => a != 0)
                .Aggregate(0.0, (current, a) => current + 1.0 / a);
        }

        public static double JaroWinklerSim(string s1, string s2, Func<char, char, int, bool> charSim, double pw = 0.1)
        {
            var jaroSim = JaroSim(s1, s2, charSim);
            if (jaroSim == 0.0)
                return 0.0;
            var p = PrefixLen(s1, s2, charSim);

            double jaroWinklerSim;
            if (p > 0.0)
                jaroWinklerSim = jaroSim + (1.0 - jaroSim) * p * pw;
            else
                jaroWinklerSim = jaroSim;
            return jaroWinklerSim;
        }

        public static bool DiacriticCharSim(char c1, char c2, int a)
        {
            return a switch
            {
                1 => c1 == c2,
                2 => c1.RemoveDiacritics() == c2.RemoveDiacritics(),
                3 => c1 == 'c' && c2 == 'k' || c1 == 'k' && c2 == 'c' || c1 == 'p' && c2 == 'b',
                _ => throw new ArgumentOutOfRangeException(nameof(a))
            };
        }
    }
}