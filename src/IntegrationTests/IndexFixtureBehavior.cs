using System;
using System.Linq;
using System.Threading.Tasks;
using MyLab.Elastic;
using MyLab.Elastic.Test;
using Xunit;

namespace IntegrationTests
{
    public class IndexFixtureBehavior : IClassFixture<EsIndexFixture<TestEntity>>
    {
        private readonly EsIndexFixture<TestEntity> _fixture;

        public IndexFixtureBehavior(EsIndexFixture<TestEntity> fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task ShouldCreateIndex()
        {
            //Act
            var indexResp = await _fixture.Manager.Client.Indices.GetAsync(_fixture.Manager.IndexName);
            var found = indexResp.Indices.Values.FirstOrDefault();

            //Assert
            Assert.NotNull(found);
        }
    }
}
