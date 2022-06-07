using System.Threading.Tasks;
using Elasticsearch.Net;
using MyLab.Search.EsAdapter;
using Nest;
using Xunit;
using Xunit.Abstractions;

namespace MyLab.Search.EsTest
{
    /// <summary>
    /// Provides manager to work with remote ES instance
    /// </summary>
    public class EsFixture<TStrategy> : IAsyncLifetime
        where TStrategy : EsFixtureStrategy, new()
    {
        private readonly IConnectionPool _connection;

        public IEsManager Manager { get; private set; }

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
        }

        public IEsSearcher<TDoc> CreateSearcher<TDoc>() where TDoc : class
        {
            return new EsSearcher<TDoc>(new SingleEsClientProvider(EsClient), null, (ElasticsearchOptions) null);
        }

        public IEsIndexer<TDoc> CreateIndexer<TDoc>() where TDoc : class
        {
            return new EsIndexer<TDoc>(new SingleEsClientProvider(EsClient), null, (ElasticsearchOptions)null);
        }

        public Task InitializeAsync()
        {
            Manager = new EsManager(new SingleEsClientProvider(EsClient), (ElasticsearchOptions)null);

            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            _connection?.Dispose();

            return Task.CompletedTask;
        }
    }
}
