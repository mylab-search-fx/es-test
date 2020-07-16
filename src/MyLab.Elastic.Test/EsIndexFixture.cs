using System;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;
using Xunit;
using Xunit.Abstractions;

namespace MyLab.Elastic.Test
{
    /// <summary>
    /// CreateAsync tmp index with specified model mapping
    /// </summary>
    public class EsIndexFixture<TDoc> : IAsyncLifetime
        where TDoc : class
    {
        private readonly IConnectionPool _connection;
        private TmpIndexLife<TDoc> _index;
        private readonly ElasticClient _client;

        public IIndexSpecificEsManager Manager { get; private set; }

        /// <summary>
        /// Test output. Set to get logs.
        /// </summary>
        public ITestOutputHelper Output { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="EsIndexFixture{TDoc}"/>
        /// </summary>
        public EsIndexFixture()
            :this(new EnvironmentConnectionProvider())
        {
            
        }

        /// <summary>
        /// Initializes a new instance of <see cref="EsIndexFixture{TDoc}"/>
        /// </summary>
        protected EsIndexFixture(IConnectionProvider connectionProvider)
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

        public async Task InitializeAsync()
        {
            _index = await TmpIndexLife<TDoc>.CreateAsync(_client);
            Manager = new TestEsManager(_client).ForIndex(_index.IndexName);
        }

        public async Task DisposeAsync()
        {
            await _index.DisposeAsync();
            _connection?.Dispose();
        }
    }
}
