using System.CodeDom.Compiler;
using System.Runtime.Remoting.Channels;
using Xunit;
using Xunit.Abstractions;

namespace JaroWinklerRecordSearch.Test
{
    public class RecordSearchPerfTests
    {
        private readonly ITestOutputHelper _log;
        private readonly RecordSearch _recordSearch;
        public RecordSearchPerfTests(ITestOutputHelper log)
        {
            _log = log;
            _recordSearch = new RecordSearch();
        }


        [Fact]
        public void TestRecordSearch1()
        {
            var testSearchQueries = new []{ "fer fer", "fer rol", "rin chr", "fer rol fer", "chr chr chr"};
            //var testSearchQueries = new []{ "fer rol"};
            foreach(var sq in testSearchQueries){
                var (resultCount, topResults) = _recordSearch.Search(sq);
                _log.WriteLine($"search '{sq}' -> {resultCount} found");
                foreach (var sr in topResults)
                    _log.WriteLine($"{sr.Text} {sr.Score}");
                _log.WriteLine("");
            }
        }
    }
}