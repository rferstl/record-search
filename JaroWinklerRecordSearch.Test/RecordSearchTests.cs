using System.CodeDom.Compiler;
using System.Linq;
using System.Runtime.Remoting.Channels;
using Xunit;
using Xunit.Abstractions;

namespace JaroWinklerRecordSearch.Test
{
    public class RecordSearchTests  : IClassFixture<RecordSearchFixture>
    {
        private readonly ITestOutputHelper _log;
        private readonly RecordSearch _recordSearch;
        public RecordSearchTests(RecordSearchFixture fixture, ITestOutputHelper log)
        {
            _log = log;
            _recordSearch = fixture.RecordSearch;
        }

        [Fact]
        public void TestPerfRecordSearch1()
        {
            var testSearchQueries = new []{ "fer fer", "fer rol", "rin chr", "fer rol fer", "chr chr chr"};
            //var testSearchQueries = new []{ "fer rol"};
            foreach(var sq in testSearchQueries){
                var topResults = _recordSearch.Search(sq).Take(10);
                _log.WriteLine($"search '{sq}'");
                foreach (var sr in topResults)
                {
                    var doc = _recordSearch.ScdNameDict[sr.DocId];
                    var text = $"{doc.LastName}, {doc.FirstName} ({doc.Gid} {sr.DocId}) {doc.OrgCode}";
                    _log.WriteLine($"{text} {sr.Score}");
                }
                _log.WriteLine("");
            }
        }
    }
}