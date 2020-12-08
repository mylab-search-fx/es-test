using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;
using Xunit;
using Xunit.Abstractions;

namespace MyLab.Elastic.Test
{
    /// <summary>
    /// Provides manager to work with remote ES instance
    /// </summary>
    public class EsFixture<TConnectionProvider> : IAsyncLifetime
        where TConnectionProvider : IConnectionProvider, new()
    {
        private readonly IConnectionPool _connection;
        private readonly ElasticClient _client;

        public IEsManager Manager { get; private set; }

        /// <summary>
        /// Test output. Set to get logs.
        /// </summary>
        public ITestOutputHelper Output { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="EsFixture{TConnectionProvider}"/>
        /// </summary>
        public EsFixture()
            :this(new TConnectionProvider())
        {
            
        }

        /// <summary>
        /// Initializes a new instance of <see cref="EsFixture{TConnectionProvider}"/>
        /// </summary>
        protected EsFixture(TConnectionProvider connectionProvider)
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

        public Task InitializeAsync()
        {
            Manager = new EsManager(new SingleEsClientProvider(_client), (ElasticsearchOptions)null);

            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            _connection?.Dispose();

            return Task.CompletedTask;
        }
    }
}
