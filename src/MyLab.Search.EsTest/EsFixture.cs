﻿using System.Threading.Tasks;
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
    public class EsFixture<TConnectionProvider> : IAsyncLifetime
        where TConnectionProvider : IConnectionProvider, new()
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

            EsClient = new ElasticClient(settings);
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