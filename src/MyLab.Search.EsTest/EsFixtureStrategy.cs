using Elasticsearch.Net;
using Nest;

namespace MyLab.Search.EsTest
{
    public abstract class EsFixtureStrategy
    {
        public abstract IConnectionPool ProvideConnection();

        public virtual ConnectionSettings CreateConnectionSettings(IConnectionPool connection)
        {
            return new ConnectionSettings(connection);
        }
    }
}