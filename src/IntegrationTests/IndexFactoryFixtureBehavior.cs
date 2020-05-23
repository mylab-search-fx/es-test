using System.Linq;
using System.Threading.Tasks;
using MyLab.Elastic.Test;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public class IndexFactoryFixtureBehavior : IClassFixture<EsIndexFactoryFixture<TestEntity>>
    {
        private readonly EsIndexFactoryFixture<TestEntity> _indexFactory;
        private readonly TestEsLogger _log;

        public IndexFactoryFixtureBehavior(EsIndexFactoryFixture<TestEntity> indexFactory, ITestOutputHelper output)
        {
            _indexFactory = indexFactory;
            _log = new TestEsLogger(output);
        }

        [Fact]
        public async Task ShouldCreateIndex()
        {
            //Act
            var indexResp = await _indexFactory.UseTmpIndex(mgr => mgr.Client.Indices.GetAsync(mgr.IndexName));

            _log.LogResponse(indexResp);

            var found = indexResp.Indices.Values.FirstOrDefault();

            //Assert
            Assert.NotNull(found);
        }
    }
}
