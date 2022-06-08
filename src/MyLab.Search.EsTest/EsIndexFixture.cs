using System;
using System.Threading.Tasks;
using Elasticsearch.Net;
using MyLab.Search.EsAdapter;
using MyLab.Search.EsAdapter.Indexing;
using MyLab.Search.EsAdapter.Inter;
using MyLab.Search.EsAdapter.Search;
using Nest;
using Xunit;
using Xunit.Abstractions;

namespace MyLab.Search.EsTest
{
    /// <summary>
    /// CreateAsync tmp index with specified model mapping
    /// </summary>
    public class EsIndexFixture<TDoc, TStrategy> : IAsyncLifetime
        where TDoc : class
        where TStrategy : EsFixtureStrategy, new()
    {
        private readonly IConnectionPool _connection;
        private TmpIndexLife<TDoc> _index;

        public IEsIndexTools<TDoc> IndexTools { get; private set; }
        public IEsSearcher<TDoc> Searcher { get; private set; }
        public IEsIndexer<TDoc> Indexer { get; private set; }

        public ElasticClient EsClient { get; }

        public string IndexName => _index?.IndexName;

        /// <summary>
        /// Test output. Set to get logs.
        /// </summary>
        public ITestOutputHelper Output { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="EsIndexFixture{TDoc, TConnectionProvider}"/>
        /// </summary>
        public EsIndexFixture()
            :this(new TStrategy())
        {
            
        }

        /// <summary>
        /// Initializes a new instance of <see cref="EsIndexFixture{TDoc, TConnectionProvider}"/>
        /// </summary>
        protected EsIndexFixture(TStrategy strategy)
        {
            _connection = strategy.ProvideConnection();
            
            var settings = strategy.CreateConnectionSettings(_connection);
            settings.DisableDirectStreaming();
            settings.OnRequestCompleted(details =>
            {
                try
                {
                    Output?.WriteLine(ApiCallDumper.ApiCallToDump(details));
                }
                catch (InvalidOperationException e)  when (e.Message == "There is no currently active test.")
                {
                    //Do nothing
                }
            });
            
            EsClient = new ElasticClient(settings);
        }

        public async Task InitializeAsync()
        {
            _index = await TmpIndexLife<TDoc>.CreateAsync(EsClient);

            var clientProvider = new SingleEsClientProvider(EsClient);
            IIndexNameProvider indexNameProvider = new SingleIndexNameProvider(_index.IndexName);

            IndexTools= new EsIndexTools<TDoc>(new EsIndexTools(clientProvider), indexNameProvider);
            Searcher = new EsSearcher<TDoc>(new EsSearcher(clientProvider), indexNameProvider);
            Indexer = new EsIndexer<TDoc>(new EsIndexer(clientProvider), indexNameProvider);
        }

        public async Task DisposeAsync()
        {
            await _index.DisposeAsync();
            _connection?.Dispose();
        }
    }
}
