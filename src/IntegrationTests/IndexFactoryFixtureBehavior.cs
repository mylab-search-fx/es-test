using System.Linq;
using System.Threading.Tasks;
using MyLab.Search.EsTest;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public class IndexFactoryFixtureBehavior : IClassFixture<EsIndexFactoryFixture<TestEntity, TestEsFixtureStrategy>>
    {
        private readonly EsIndexFactoryFixture<TestEntity, TestEsFixtureStrategy> _indexFactory;

        public IndexFactoryFixtureBehavior(EsIndexFactoryFixture<TestEntity, TestEsFixtureStrategy> indexFactory, ITestOutputHelper output)
        {
            _indexFactory = indexFactory;
            indexFactory.Output = output;
        }

        [Fact]
        public async Task ShouldCreateIndex()
        {
            //Act
            var indexExist = await _indexFactory.UseTmpIndexAsync(srv => srv.IndexTools.IsIndexExistsAsync());

            //Assert
            Assert.True(indexExist);
        }
    }
}
