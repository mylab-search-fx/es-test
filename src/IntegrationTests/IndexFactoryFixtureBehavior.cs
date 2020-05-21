using System.Linq;
using System.Threading.Tasks;
using MyLab.Elastic.Test;
using Xunit;

namespace IntegrationTests
{
    public class IndexFactoryFixtureBehavior : IClassFixture<EsIndexFactoryFixture<TestEntity>>
    {
        private readonly EsIndexFactoryFixture<TestEntity> _indexFactory;

        public IndexFactoryFixtureBehavior(EsIndexFactoryFixture<TestEntity> indexFactory)
        {
            _indexFactory = indexFactory;
        }

        [Fact]
        public async Task ShouldCreateIndex()
        {
            //Act
            var indexResp = await _indexFactory.UseTmpIndex(mgr => mgr.Client.Indices.GetAsync(mgr.IndexName));
            var found = indexResp.Indices.Values.FirstOrDefault();

            //Assert
            Assert.NotNull(found);
        }
    }
}
