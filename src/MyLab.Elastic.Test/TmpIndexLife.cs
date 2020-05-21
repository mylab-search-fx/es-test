using System;
using System.Threading.Tasks;
using Nest;

namespace MyLab.Elastic.Test
{
    class TmpIndexLife<TDoc> : IAsyncDisposable
        where TDoc : class
    {
        private readonly ElasticClient _client;

        public string IndexName { get; }

        TmpIndexLife(ElasticClient client, string indexName)
        {
            _client = client;
            IndexName = indexName;
        }

        public static Task<TmpIndexLife<TDoc>> CreateAsync(ElasticClient client)
        {
            return CreateAsync(client, Guid.NewGuid().ToString("N"));
        }
        public static async Task<TmpIndexLife<TDoc>> CreateAsync(ElasticClient client, string key)
        {
            var indexName = "test-" + key;

            var res = await client.Indices.CreateAsync(
                indexName, cd => cd.Map<TDoc>(md => md.AutoMap()));

            if (!res.ShardsAcknowledged)
                throw new InvalidOperationException("Could not create index");

            return new TmpIndexLife<TDoc>(client, indexName);
        }

        public ValueTask DisposeAsync()
        {
            return new ValueTask(_client.Indices.DeleteAsync(IndexName));
        }
    }
}