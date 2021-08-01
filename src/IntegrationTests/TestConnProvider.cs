using System;
using Elasticsearch.Net;
using MyLab.Search.EsTest;

namespace IntegrationTests
{
    public class TestConnProvider : IConnectionProvider
    {
        public IConnectionPool Provide()
        {
            return new SingleNodeConnectionPool(new Uri("http://localhost:10115"));
        }
    }
}