using Elasticsearch.Net;
using Nest;

namespace MyLab.Search.EsTest
{
    public interface IEsFixtureStrategy
    {
        IConnectionPool ProvideConnection();

        void ApplyConnectionSettings(ConnectionSettings connectionSettings);
    }
}