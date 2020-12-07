using System;
using Elasticsearch.Net;
using Nest;
using Xunit.Abstractions;

namespace MyLab.Elastic.Test
{
    /// <summary>
    /// Provides EsManager with test connection
    /// </summary>
    public class EsManagerFixture<TConnectionProvider> : IDisposable
        where TConnectionProvider : IConnectionProvider, new()
    {
        private readonly IConnectionPool _connection;

        /// <summary>
        /// ES manager
        /// </summary>
        public IEsManager Manager { get; }

        /// <summary>
        /// Test output. Set to get logs.
        /// </summary>
        public ITestOutputHelper Output { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="EsManagerFixture{TConnectionProvider}"/>
        /// </summary>
        public EsManagerFixture()
            : this(new TConnectionProvider())
        {

        }

        /// <summary>
        /// Initializes a new instance of <see cref="EsManagerFixture{TConnectionProvider}"/>
        /// </summary>
        protected EsManagerFixture(TConnectionProvider connectionProvider)
        {
            _connection = connectionProvider.Provide();

            var settings = new ConnectionSettings(_connection);
            settings.DisableDirectStreaming();
            settings.OnRequestCompleted(details =>
            {
                Output?.WriteLine(ApiCallDumper.ApiCallToDump(details));
            });

            var client = new ElasticClient(settings);
            Manager = new TestEsManager(client);
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
