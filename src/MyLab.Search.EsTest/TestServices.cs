using MyLab.Search.EsAdapter;
using MyLab.Search.EsAdapter.Indexing;
using MyLab.Search.EsAdapter.Search;

namespace MyLab.Search.EsTest
{
    public class TestServices<TDoc>
        where TDoc : class
    {
        public string IndexName { get; }
        public IEsIndexTools<TDoc> IndexTools { get; }
        public IEsSearcher<TDoc> Searcher { get;  }
        public IEsIndexer<TDoc> Indexer { get;  }

        public TestServices(string indexName, IEsIndexTools<TDoc> indexTools, IEsIndexer<TDoc> indexer, IEsSearcher<TDoc> searcher)
        {
            IndexName = indexName;
            IndexTools = indexTools;
            Indexer = indexer;
            Searcher = searcher;
        }
    }
}