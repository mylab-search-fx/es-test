using Elasticsearch.Net;
using Nest;

namespace MyLab.Search.EsTest
{
    /// <summary>
    /// Tunes <see cref="EsFixture{TStrategy}"/>
    /// </summary>
    public abstract class EsFixtureStrategy
    {
        /// <summary>
        /// Provides Elasticsearch connection pool
        /// </summary>
        public abstract IConnectionPool ProvideConnection();

        /// <summary>
        /// Creates settings for connection
        /// </summary>
        public virtual ConnectionSettings CreateConnectionSettings(IConnectionPool connection)
        {
            return new ConnectionSettings(connection);
        }
    }
}