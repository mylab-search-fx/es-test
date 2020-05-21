using System;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;

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
            _client = new ElasticClient(settings);
        }

        /// <summary>
        /// Creates index for the duration of action performing
        /// </summary>
        public async Task UseTmpIndex(Func<IIndexSpecificEsManager, Task> action)
        {
            await using var indexLife = await TmpIndexLife<TDoc>.CreateAsync(_client);

            var mgr = new TestEsManager(_client).ForIndex(indexLife.IndexName);

            await action(mgr);
        }

        /// <summary>
        /// Creates index for the duration of function performing
        /// </summary>
        public async Task<TRes> UseTmpIndex<TRes>(Func<IIndexSpecificEsManager, Task<TRes>> func)
        {
            await using var indexLife = await TmpIndexLife<TDoc>.CreateAsync(_client);

            var mgr = new TestEsManager(_client).ForIndex(indexLife.IndexName);

            return await func(mgr);
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}