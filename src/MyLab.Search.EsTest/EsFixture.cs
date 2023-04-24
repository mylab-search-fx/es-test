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
    /// Provides services to work with remote ES instance
    /// </summary>
    public class EsFixture<TStrategy> : IAsyncLifetime
        where TStrategy : EsFixtureStrategy, new()
    {
        private readonly IConnectionPool _connection;

        /// <summary>
        /// Indexes the documents
        /// </summary>
        public IEsIndexer Indexer { get; }
        /// <summary>
        /// Tools to manage indexes
        /// </summary>
        public IEsIndexTools IndexTools { get; }
        /// <summary>
        /// Performs search requests
        /// </summary>
        public IEsSearcher Searcher { get; }
        /// <summary>
        /// NEST ES client
        /// </summary>
        public ElasticClient EsClient { get; }

        /// <summary>
        /// Test output. Set to get logs.
        /// </summary>
        public ITestOutputHelper Output { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="EsFixture{TConnectionProvider}"/>
        /// </summary>
        public EsFixture()
            :this(new TStrategy())
        {
            
        }

        /// <summary>
        /// Creates temp index 
        /// </summary>
        public async Task<TmpIndex<TDoc>> CreateTmpIndex<TDoc>(string tmpIndexName = null)
            where TDoc: class
        {
            return await TmpIndex<TDoc>.CreateAsync(EsClient, tmpIndexName);
        }

        /// <summary>
        /// Creates index for the duration of action performing
        /// </summary>
        public async Task UseTmpIndexASync<TDoc>(Func<TestServices<TDoc>, Task> action, string tmpIndexName = null)
            where TDoc : class
        {
            await using var indexLife = await TmpIndex<TDoc>.CreateAsync(EsClient, tmpIndexName);

            var clientProvider = new SingleEsClientProvider(EsClient);
            var indexNameProvider = new SingleIndexNameProvider(indexLife.IndexName);

            await action(new TestServices<TDoc>(
                indexLife.IndexName,
                new EsSpecialIndexTools<TDoc>(new EsIndexTools(clientProvider), indexNameProvider),
                new EsIndexer<TDoc>(new EsIndexer(clientProvider), indexNameProvider),
                new EsSearcher<TDoc>(new EsSearcher(clientProvider), indexNameProvider)
                ));
        }

        /// <summary>
        /// Creates index for the duration of function performing
        /// </summary>
        public async Task<TRes> UseTmpIndexAsync<TDoc, TRes>(Func<TestServices<TDoc>, Task<TRes>> func, string tmpIndexName = null)
            where TDoc : class
        {
            await using var indexLife = await TmpIndex<TDoc>.CreateAsync(EsClient, tmpIndexName);

            var clientProvider = new SingleEsClientProvider(EsClient);
            var indexNameProvider = new SingleIndexNameProvider(indexLife.IndexName);

            return await func(new TestServices<TDoc>(
                indexLife.IndexName,
                new EsSpecialIndexTools<TDoc>(new EsIndexTools(clientProvider), indexNameProvider),
                new EsIndexer<TDoc>(new EsIndexer(clientProvider), indexNameProvider),
                new EsSearcher<TDoc>(new EsSearcher(clientProvider), indexNameProvider)
            ));
        }

        /// <summary>
        /// Initializes a new instance of <see cref="EsFixture{TConnectionProvider}"/>
        /// </summary>
        protected EsFixture(TStrategy strategy)
        {
            _connection = strategy.ProvideConnection();
            
            var settings = strategy.CreateConnectionSettings(_connection);
            settings.DisableDirectStreaming();
            settings.OnRequestCompleted(details =>
            { 
                Output?.WriteLine(ApiCallDumper.ApiCallToDump(details));
            });

            EsClient = new ElasticClient(settings);

            var clientProvider = new SingleEsClientProvider(EsClient);

            Indexer = new EsIndexer(clientProvider);
            Searcher = new EsSearcher(clientProvider);
            IndexTools = new EsIndexTools(clientProvider);
        }

        /// <inheritdoc />
        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task DisposeAsync()
        {
            _connection?.Dispose();

            return Task.CompletedTask;
        }
    }
}
