using MyLab.Search.EsAdapter;
using MyLab.Search.EsAdapter.Indexing;
using MyLab.Search.EsAdapter.Search;
using MyLab.Search.EsAdapter.Tools;

namespace MyLab.Search.EsTest
{
    public class TestServices<TDoc>
        where TDoc : class
    {
        public string IndexName { get; }
        public IEsTools Tools { get; }
        public IEsSearcher<TDoc> Searcher { get;  }
        public IEsIndexer<TDoc> Indexer { get;  }

        public TestServices(string indexName, IEsTools tools, IEsIndexer<TDoc> indexer, IEsSearcher<TDoc> searcher)
        {
            IndexName = indexName;
            Tools = tools;
            Indexer = indexer;
            Searcher = searcher;
        }
    }
}