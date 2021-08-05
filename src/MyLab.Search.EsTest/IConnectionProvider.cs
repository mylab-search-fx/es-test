using Elasticsearch.Net;

namespace MyLab.Search.EsTest
{
    public interface IConnectionProvider
    {
        IConnectionPool Provide();
    }
}