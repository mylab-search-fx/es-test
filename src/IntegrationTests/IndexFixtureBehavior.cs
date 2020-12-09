using System.Threading.Tasks;
using MyLab.Elastic.Test;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public class IndexFixtureBehavior : IClassFixture<EsIndexFixture<TestEntity, TestConnProvider>>
    {
        private readonly EsIndexFixture<TestEntity, TestConnProvider> _fixture;

        public IndexFixtureBehavior(EsIndexFixture<TestEntity, TestConnProvider> fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            fixture.Output = output;
        }

        [Fact]
        public async Task ShouldCreateIndex()
        {
            //Act
            var indexFound = await _fixture.Manager.IsIndexExistsAsync(_fixture.IndexName);

            //Assert
            Assert.True(indexFound);
        }
    }
}
