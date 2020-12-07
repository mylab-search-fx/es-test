using System;
using System.Linq;
using System.Threading.Tasks;
using MyLab.Elastic;
using MyLab.Elastic.Test;
using Nest;
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
            var indexResp = await _fixture.Manager.Client.Indices.GetAsync(_fixture.Manager.IndexName);
            var found = indexResp.Indices.Values.FirstOrDefault();

            //Assert
            Assert.NotNull(found);
        }
    }
}
