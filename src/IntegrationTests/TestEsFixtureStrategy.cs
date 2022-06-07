using System;
using Elasticsearch.Net;
using MyLab.Search.EsTest;
using Nest;

namespace IntegrationTests
{
    public class TestEsFixtureStrategy : IEsFixtureStrategy
    {
        public IConnectionPool ProvideConnection()
        {
            return new SingleNodeConnectionPool(new Uri("http://localhost:9200"));
        }

        public void ApplyConnectionSettings(ConnectionSettings connectionSettings)
        {
            
        }
    }
}