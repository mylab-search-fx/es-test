using System;
using Elasticsearch.Net;
using Nest;

namespace MyLab.Elastic.Test
{
    /// <summary>
    /// Provides EsManager with test connection
    /// </summary>
    public class EsManagerFixture : IDisposable
    {
        private readonly IConnectionPool _connection;

        /// <summary>
        /// ES manager
        /// </summary>
        public IEsManager Manager { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="EsManagerFixture"/>
        /// </summary>
        public EsManagerFixture()
            : this(new EnvironmentConnectionProvider())
        {

        }

        /// <summary>
        /// Initializes a new instance of <see cref="EsIndexFixture{TDoc}"/>
        /// </summary>
        protected EsManagerFixture(IConnectionProvider connectionProvider)
        {
            _connection = connectionProvider.Provide();

            var settings = new ConnectionSettings(_connection);
            settings.DisableDirectStreaming();

            var client = new ElasticClient(settings);
            Manager = new TestEsManager(client);
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
