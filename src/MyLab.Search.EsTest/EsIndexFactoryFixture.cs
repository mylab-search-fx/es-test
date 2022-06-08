using System;
using System.Threading.Tasks;
using Elasticsearch.Net;
using MyLab.Search.EsAdapter;
using MyLab.Search.EsAdapter.Indexing;
using MyLab.Search.EsAdapter.Inter;
using MyLab.Search.EsAdapter.Search;
using Nest;
using Xunit.Abstractions;

namespace MyLab.Search.EsTest
{
    /// <summary>
    /// Create tmp index factory with specified model mapping
    /// </summary>
    public class EsIndexFactoryFixture<TDoc, TStrategy> : IDisposable
        where TDoc : class
        where TStrategy : EsFixtureStrategy, new()
    {
        private readonly IConnectionPool _connection;
        private readonly ElasticClient _client;

        /// <summary>
        /// Test output. Set to get logs.
        /// </summary>
        public ITestOutputHelper Output { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="EsIndexFactoryFixture{TDoc, TConnectionProvider}"/>
        /// </summary>
        public EsIndexFactoryFixture()
            : this(new TStrategy())
        {

        }

        /// <summary>
        /// Initializes a new instance of <see cref="EsIndexFactoryFixture{TDoc, TConnectionProvider}"/>
        /// </summary>
        protected EsIndexFactoryFixture(TStrategy strategy)
        {
            _connection = strategy.ProvideConnection();
            
            var settings = strategy.CreateConnectionSettings(_connection);
            settings.DisableDirectStreaming();
            settings.OnRequestCompleted(details =>
            {
                Output?.WriteLine(ApiCallDumper.ApiCallToDump(details));
            });
            
            _client = new ElasticClient(settings);
        }

        /// <summary>
        /// Creates temp index 
        /// </summary>
        public async Task<IAsyncDisposable> CreateTmpIndex(string tmpIndexName = null)
        {
            return await TmpIndexLife<TDoc>.CreateAsync(_client, tmpIndexName);
        }

        /// <summary>
        /// Creates index for the duration of action performing
        /// </summary>
        public async Task UseTmpIndexASync(Func<TestServices<TDoc>, Task> action, string tmpIndexName = null)
        {
            await using var indexLife = await TmpIndexLife<TDoc>.CreateAsync(_client, tmpIndexName);
            
            var clientProvider = new SingleEsClientProvider(_client);
            var indexNameProvider = new SingleIndexNameProvider(indexLife.IndexName);

            await action(new TestServices<TDoc>(
                indexLife.IndexName,
                new EsIndexTools<TDoc>(new EsIndexTools(clientProvider), indexNameProvider),
                new EsIndexer<TDoc>(new EsIndexer(clientProvider), indexNameProvider),
                new EsSearcher<TDoc>(new EsSearcher(clientProvider), indexNameProvider)
                ));
        }

        /// <summary>
        /// Creates index for the duration of function performing
        /// </summary>
        public async Task<TRes> UseTmpIndexAsync<TRes>(Func<TestServices<TDoc>, Task<TRes>> func, string tmpIndexName = null)
        {
            await using var indexLife = await TmpIndexLife<TDoc>.CreateAsync(_client, tmpIndexName);

            var clientProvider = new SingleEsClientProvider(_client);
            var indexNameProvider = new SingleIndexNameProvider(indexLife.IndexName);

            return await func(new TestServices<TDoc>(
                indexLife.IndexName,
                new EsIndexTools<TDoc>(new EsIndexTools(clientProvider), indexNameProvider),
                new EsIndexer<TDoc>(new EsIndexer(clientProvider), indexNameProvider),
                new EsSearcher<TDoc>(new EsSearcher(clientProvider), indexNameProvider)
            ));
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}