using System;
using Nest;

namespace MyLab.Elastic.Test
{
    class TestEsManager : IEsManager
    {
        public ElasticClient Client { get; set; }

        public TestEsManager(ElasticClient client)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
        }
    }
}