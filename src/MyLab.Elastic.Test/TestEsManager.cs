using System;
using Nest;

namespace MyLab.Elastic.Test
{
    /// <summary>
    /// Implemeents <see cref="IEsManager"/> with specified <see cref="ElasticClient"/>
    /// </summary>
    public class TestEsManager : IEsManager
    {
        public ElasticClient Client { get; set; }

        public TestEsManager(ElasticClient client)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
        }
    }
}