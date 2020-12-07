using Elasticsearch.Net;

namespace MyLab.Elastic.Test
{
    public interface IConnectionProvider
    {
        IConnectionPool Provide();
    }
}