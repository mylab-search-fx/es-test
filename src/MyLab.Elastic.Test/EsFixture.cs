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
    public class EsFixture : IAsyncLifetime
    {
        private readonly IConnectionPool _connection;
        private readonly ElasticClient _client;

        public IEsManager Manager { get; private set; }

        /// <summary>
        /// Test output. Set to get logs.
        /// </summary>
        public ITestOutputHelper Output { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="EsIndexFixture{TDoc}"/>
        /// </summary>
        public EsFixture()
            :this(new EnvironmentConnectionProvider())
        {
            
        }

        /// <summary>
        /// Initializes a new instance of <see cref="EsIndexFixture{TDoc}"/>
        /// </summary>
        protected EsFixture(IConnectionProvider connectionProvider)
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
            Manager = new TestEsManager(_client);

            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            _connection?.Dispose();

            return Task.CompletedTask;
        }
    }
}
