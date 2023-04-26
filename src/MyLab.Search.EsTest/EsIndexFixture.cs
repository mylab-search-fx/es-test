using System;
using System.Threading.Tasks;
using Elasticsearch.Net;
using MyLab.Search.EsAdapter;
using MyLab.Search.EsAdapter.Indexing;
using MyLab.Search.EsAdapter.Inter;
using MyLab.Search.EsAdapter.Search;
using MyLab.Search.EsAdapter.Tools;
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
        private TmpIndex<TDoc> _index;

        /// <summary>
        /// Tools to manage ES
        /// </summary>
        public IEsTools Tools { get; private set; }
        /// <summary>
        /// Specific index searcher <see cref="IEsSearcher{TDoc}"/>
        /// </summary>
        public IEsSearcher<TDoc> Searcher { get; private set; }
        /// <summary>
        /// Specific index indexer <see cref="IEsIndexer{TDoc}"/>
        /// </summary>
        public IEsIndexer<TDoc> Indexer { get; private set; }

        /// <summary>
        /// Underline Elasticsearch client
        /// </summary>
        public ElasticClient EsClient { get; }

        /// <summary>
        /// Index name
        /// </summary>
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

        /// <inheritdoc />
        public async Task InitializeAsync()
        {
            _index = await TmpIndex<TDoc>.CreateAsync(EsClient);

            var clientProvider = new SingleEsClientProvider(EsClient);
            IIndexNameProvider indexNameProvider = new SingleIndexNameProvider(_index.IndexName);

            Tools= new EsTools(clientProvider);
            Searcher = new EsSearcher<TDoc>(new EsSearcher(clientProvider), indexNameProvider);
            Indexer = new EsIndexer<TDoc>(new EsIndexer(clientProvider), indexNameProvider);
        }

        /// <inheritdoc />
        public async Task DisposeAsync()
        {
            await _index.DisposeAsync();
            _connection?.Dispose();
        }
    }
}
