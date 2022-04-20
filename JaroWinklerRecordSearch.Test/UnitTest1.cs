using System;
using System.Collections.Generic;
using System.Linq;
using JaroWinklerRecordSearch.Helper;
using Munkres;
using Xunit;
using Xunit.Abstractions;

namespace JaroWinklerRecordSearch.Test
{
    public class UnitTest1
    {

        private readonly ITestOutputHelper _log;
        public UnitTest1(ITestOutputHelper log)
        {
            _log = log;
        }

        [Fact]
        public void TestMethod1()
        {
        }

        [Fact]
        public void TestMergeUtils()
        {
            var aa1 = new int[][]
                { new[] { 2, 5, 7, 9, 12 }, new[] { 0, 1, 3, 4, 6, 8, 11 }, new[] { 0, 4, 5, 9, 12, 13 } };
            var r1 = aa1.MergeBy(e => e, (k,xs) => (k, xs.Count()));
            //r1.Dump("r1", 1);

            var aa2 = new List<IEnumerable<TestInput>>
        {
            new List<TestInput>
            {
                new(1, 3, 0.9), new(3, 7, 0.1), new(5, 4, 0.2), new(6, 3, 0.3),
                new(7, 9, 0.3), new(9, 9, 0.5), new(13, 13, 0.6), new(15, 23, 0.7)
            },
            new List<TestInput>
            {
                new(2, 5, 0.8), new(4, 23, 0.7), new(6, 13, 0.6), new(8, 32, 0.5),
                new(10, 9, 0.4), new(13, 4, 0.2)
            }
        };

            TestResult merge(int k, IEnumerable<TestInput> ts) => new(k, ts.Select(a => (a.Tid, a.Score)).ToList());

            var r2 = aa2.MergeBy(e => e.Id, merge);
            r2.Dump(_log, "r2");
        }


        [Fact]
        public void TestJaroWinkler()
        {
            var term = "je";
            var test = "jens";

            term.Dump(_log, "term");
            test.Dump(_log, "test");
            var (totalScore, debug1, debug2) = JarooWinklerUtils.DebugJaroWinkler(term.ToLowerInvariant(), test.ToLowerInvariant(), 0.2);
            //totalScore += 1 - Math.Tanh(8 - 8 * jaroWinkler1);
            totalScore.Dump(_log, "totalScore");
            debug1.Dump(_log);
            debug2.Dump(_log);
        }

        [Fact]
        public void TestJaroWinklerSearch()
        {
            var search = ("chis", "rin");
            var tests = new[] { ("josef", "Josef"), ("josef", "József") };
            var tests1 = new[] { ("chr", "rin"), ("Christoph", "Rinn") };
            var tests2 = new[] { ("chr", "rin"), ("Chris", "Rinnert") };
            var tests3 = new[] { ("Chairul", "Rini") };
            var tests4 = new[] { ("Chandirasekaran", ""), ("Christian", "Riu") };
            var tests5 = new[] { ("Christoph", "Rinn"), ("Christian", "Riu"), ("Chandirasekaran", "") };

            search.Dump(_log, "search");
            foreach (var t in tests5)
            {
                t.Dump(_log, "test");
                var totalScore = 0.0;
                var (jaroWinkler1, debug1, debug2) = JarooWinklerUtils.DebugJaroWinkler(search.Item1.ToLowerInvariant(), t.Item1.ToLowerInvariant(), 0.2);
                totalScore += 1 - Math.Tanh(8 - 8 * jaroWinkler1);
                totalScore.Dump(_log, "totalScore");
                debug1.Dump(_log);
                debug2.Dump(_log);
                "------------------------------------".Dump(_log);
            }
        }

        [Fact]
        public void TestMunkres()
        {
            // This is the same example that is found in Wikipedia:
            // https://en.wikipedia.org/wiki/Hungarian_algorithm

            double[,] costMatrix =
            {
                //               Clean bathroom, Sweep floors,  Wash windows
                /* Armond   */  {    -2,           -3,           -3       },
                /* Francine */  {    -3,           -2,           -3       },
                /* Herbert  */  {    -3,           -3,           -2       }
            };

            var m = new MunkresProgram();
            m.RunMunkres(costMatrix);
            _log.WriteLine(m.Log.ToString());
        }

        [Fact]
        public void TestMunkres2()
        {
            double[,] costMatrix =
            {
                // pos                        FERA|fera (46580).0  FERIKA|ferika (46581).0  FERA|fera (46580).1   FERIKA|ferika (46581).1 
                /* FirstName.0 */        {    -0.991666667,         0.0,                     -0.991666667,	      0.0         },
                /* LastName.0  */        {    0.0,	               -0.983333333,             0,                    -0.983333333 }
            };
            var m = new MunkresProgram();
            m.RunMunkres(costMatrix);
            _log.WriteLine(m.Log.ToString());
        }

        [Fact]
        public void TestMunkres3()
        {
            double[,] costMatrix =
            {
                // pos                        FERA|fera (46580).0  FERIKA|ferika (46581).0  
                /* FirstName.0 */        {    -0.991666667,         -0.99               },
                /* LastName.0  */        {    -0.99,	            -0.983333333        },
                /* LastName.1  */        {    -0.991666667,	        -0.983333334        }
            };
            var m = new MunkresProgram();
            m.RunMunkres(costMatrix);
            _log.WriteLine(m.Log.ToString());
        }

        /*
         *
         *
         */
        public struct TestInput
        {
            public TestInput(int Id, int Tid, double Score)
            {
                this.Id = Id;
                this.Tid = Tid;
                this.Score = Score;
            }

            public int Id { get; set; }
            public int Tid { get; set; }
            public double Score { get; set; }

            public void Deconstruct(out int id, out int tid, out double score)
            {
                id = Id;
                tid = Tid;
                score = Score;
            }
        }

        public struct TestResult
        {
            public TestResult(int id, IList<(int tid, double score)> tidScores)
            {
                Id = id;
                TidScores = tidScores;
            }

            public int Id { get; set; }
            public IList<(int tid, double score)> TidScores { get; set; }

            public void Deconstruct(out int id, out IList<(int tid, double score)> tidScores)
            {
                id = Id;
                tidScores = TidScores;
            }
        }

    }

    public static class DumpExtensions
    {
        public static T Dump<T>(this T o, ITestOutputHelper output)
        {
            output.WriteLine($"{o}");
            return o;
        }
        public static T Dump<T>(this T o, ITestOutputHelper output, string description)
        {
            output.WriteLine($"{description}: {o}");
            return o;
        }

    }

}
