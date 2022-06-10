using System;
using System.Threading.Tasks;
using Nest;

namespace MyLab.Search.EsTest
{
    /// <summary>
    /// Manage tmp index life
    /// </summary>
    public class TmpIndex<TDoc> : IAsyncDisposable
        where TDoc : class
    {
        private readonly ElasticClient _client;

        /// <summary>
        /// Index name
        /// </summary>
        public string IndexName { get; }

        /// <summary>
        /// Index creation response
        /// </summary>
        public IResponse CreationResponse { get; private set; }

        /// <summary>
        /// Initializes a new instance of <see cref="TmpIndex{TDoc}"/>
        /// </summary>
        TmpIndex(ElasticClient client, string indexName)
        {
            _client = client;
            IndexName = indexName;
        }
        
        /// <summary>
        /// Creates an index 
        /// </summary>
        public static async Task<TmpIndex<TDoc>> CreateAsync(ElasticClient client, string indexName = null)
        {
            var resultIndexName = indexName ?? "test-" + Guid.NewGuid().ToString("N");

            var res = await client.Indices.CreateAsync(
                resultIndexName, cd => cd.Map<TDoc>(md => md.AutoMap()));

            if (!res.ShardsAcknowledged)
                throw new InvalidOperationException("Unable to create index");

            return new TmpIndex<TDoc>(client, resultIndexName)
            {
                CreationResponse = res
            };
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            return new ValueTask(_client.Indices.DeleteAsync(IndexName));
        }
    }
}