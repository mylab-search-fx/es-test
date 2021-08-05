using System;
using System.Threading.Tasks;
using Elasticsearch.Net;
using MyLab.Search.EsAdapter;
using Nest;
using Xunit.Abstractions;

namespace MyLab.Search.EsTest
{
    /// <summary>
    /// CreateAsync tmp index factory with specified model mapping
    /// </summary>
    public class EsIndexFactoryFixture<TDoc, TConnectionProvider> : IDisposable
        where TDoc : class
        where TConnectionProvider : IConnectionProvider, new()
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
            : this(new TConnectionProvider())
        {

        }

        /// <summary>
        /// Initializes a new instance of <see cref="EsIndexFactoryFixture{TDoc, TConnectionProvider}"/>
        /// </summary>
        protected EsIndexFactoryFixture(TConnectionProvider connectionProvider)
        {
            _connection = connectionProvider.Provide();
            
            var settings = new ConnectionSettings(_connection);
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
        public async Task UseTmpIndex(Func<TestServices<TDoc>, Task> action, string tmpIndexName = null)
        {
            await using var indexLife = await TmpIndexLife<TDoc>.CreateAsync(_client, tmpIndexName);

            var searcher = new EsSearcher<TDoc>(new SingleEsClientProvider(_client), null, (ElasticsearchOptions)null).ForIndex(indexLife.IndexName);
            var manager = new EsManager(new SingleEsClientProvider(_client), (ElasticsearchOptions)null);
            var indexer = new EsIndexer<TDoc>(new SingleEsClientProvider(_client), null, (ElasticsearchOptions)null).ForIndex(indexLife.IndexName);

            await action(new TestServices<TDoc>
            {
                IndexName = indexLife.IndexName,
                Indexer = indexer,
                Manager = manager,
                Searcher = searcher
            });
        }

        /// <summary>
        /// Creates index for the duration of function performing
        /// </summary>
        public async Task<TRes> UseTmpIndex<TRes>(Func<TestServices<TDoc>, Task<TRes>> func, string tmpIndexName = null)
        {
            await using var indexLife = await TmpIndexLife<TDoc>.CreateAsync(_client, tmpIndexName);

            var searcher = new EsSearcher<TDoc>(new SingleEsClientProvider(_client), null, (ElasticsearchOptions)null).ForIndex(indexLife.IndexName);
            var manager = new EsManager(new SingleEsClientProvider(_client), (ElasticsearchOptions)null);
            var indexer = new EsIndexer<TDoc>(new SingleEsClientProvider(_client), null, (ElasticsearchOptions)null).ForIndex(indexLife.IndexName);

            return await func(new TestServices<TDoc>
            {
                IndexName = indexLife.IndexName,
                Indexer = indexer,
                Manager = manager,
                Searcher = searcher
            });
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}