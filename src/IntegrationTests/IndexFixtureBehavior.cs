using System.Threading.Tasks;
using MyLab.Search.EsTest;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public class IndexFixtureBehavior : IClassFixture<EsIndexFixture<TestEntity, TestEsFixtureStrategy>>
    {
        private readonly EsIndexFixture<TestEntity, TestEsFixtureStrategy> _fixture;

        public IndexFixtureBehavior(EsIndexFixture<TestEntity, TestEsFixtureStrategy> fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            fixture.Output = output;
        }

        [Fact]
        public async Task ShouldCreateIndex()
        {
            //Act
            var indexFound = await _fixture.Tools.Index(_fixture.IndexName).ExistsAsync();

            //Assert
            Assert.True(indexFound);
        }
    }
}
