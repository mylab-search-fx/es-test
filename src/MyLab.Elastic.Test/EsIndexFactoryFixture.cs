using System;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;
using Xunit.Abstractions;

namespace MyLab.Elastic.Test
{
    /// <summary>
    /// CreateAsync tmp index factory with specified model mapping
    /// </summary>
    public class EsIndexFactoryFixture<TDoc> : IDisposable
        where TDoc : class
    {
        private readonly IConnectionPool _connection;
        private readonly ElasticClient _client;

        /// <summary>
        /// Test output. Set to get logs.
        /// </summary>
        public ITestOutputHelper Output { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="EsIndexFixture{TDoc}"/>
        /// </summary>
        public EsIndexFactoryFixture()
            : this(new EnvironmentConnectionProvider())
        {

        }

        /// <summary>
        /// Initializes a new instance of <see cref="EsIndexFixture{TDoc}"/>
        /// </summary>
        protected EsIndexFactoryFixture(IConnectionProvider connectionProvider)
        {
            _connection = connectionProvider.Provide();
            
            var settings = new ConnectionSettings(_connection);
            settings.DisableDirectStreaming();
            settings.OnRequestCompleted(details =>
            {
                if (Output != null)
                    TestEsLogger.Log(Output, details);
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
        public async Task UseTmpIndex(Func<IIndexSpecificEsManager, Task> action, string tmpIndexName = null)
        {
            await using var indexLife = await TmpIndexLife<TDoc>.CreateAsync(_client, tmpIndexName);

            var mgr = new TestEsManager(_client).ForIndex(indexLife.IndexName);

            await action(mgr);
        }

        /// <summary>
        /// Creates index for the duration of function performing
        /// </summary>
        public async Task<TRes> UseTmpIndex<TRes>(Func<IIndexSpecificEsManager, Task<TRes>> func, string tmpIndexName = null)
        {
            await using var indexLife = await TmpIndexLife<TDoc>.CreateAsync(_client, tmpIndexName);

            var mgr = new TestEsManager(_client).ForIndex(indexLife.IndexName);

            return await func(mgr);
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}