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

        public IResponse CreationResponse { get; private set; }

        TmpIndexLife(ElasticClient client, string indexName)
        {
            _client = client;
            IndexName = indexName;
        }
        
        public static async Task<TmpIndexLife<TDoc>> CreateAsync(ElasticClient client, string indexName = null)
        {
            var resultIndexName = indexName ?? "test-" + Guid.NewGuid().ToString("N");

            var res = await client.Indices.CreateAsync(
                resultIndexName, cd => cd.Map<TDoc>(md => md.AutoMap()));

            if (!res.ShardsAcknowledged)
                throw new ResponseException<CreateIndexResponse>("Could not create index", res);

            return new TmpIndexLife<TDoc>(client, resultIndexName)
            {
                CreationResponse = res
            };
        }

        public ValueTask DisposeAsync()
        {
            return new ValueTask(_client.Indices.DeleteAsync(IndexName));
        }
    }
}