using System.Linq;
using System.Threading.Tasks;
using MyLab.Search.EsTest;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public class IndexFactoryFixtureBehavior : IClassFixture<EsIndexFactoryFixture<TestEntity, TestConnProvider>>
    {
        private readonly EsIndexFactoryFixture<TestEntity, TestConnProvider> _indexFactory;

        public IndexFactoryFixtureBehavior(EsIndexFactoryFixture<TestEntity, TestConnProvider> indexFactory, ITestOutputHelper output)
        {
            _indexFactory = indexFactory;
            indexFactory.Output = output;
        }

        [Fact]
        public async Task ShouldCreateIndex()
        {
            //Act
            var indexExist = await _indexFactory.UseTmpIndex(srv => srv.Manager.IsIndexExistsAsync(srv.IndexName));

            //Assert
            Assert.True(indexExist);
        }
    }
}
