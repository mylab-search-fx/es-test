using System;
using System.Threading.Tasks;
using Elasticsearch.Net;
using MyLab.Search.EsAdapter;
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

        public IIndexSpecificEsSearcher<TDoc> Searcher { get; private set; }
        public IIndexSpecificEsIndexer<TDoc> Indexer { get; private set; }
        public IEsManager Manager{ get; private set; }

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
            
            Searcher = new EsSearcher<TDoc>(new SingleEsClientProvider(EsClient), null, (ElasticsearchOptions)null).ForIndex(_index.IndexName);
            Manager = new EsManager(new SingleEsClientProvider(EsClient), (ElasticsearchOptions)null);
            Indexer = new EsIndexer<TDoc>(new SingleEsClientProvider(EsClient), null, (ElasticsearchOptions)null).ForIndex(_index.IndexName);
        }

        public async Task DisposeAsync()
        {
            await _index.DisposeAsync();
            _connection?.Dispose();
        }
    }
}
