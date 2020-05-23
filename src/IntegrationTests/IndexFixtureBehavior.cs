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
    public class IndexFixtureBehavior : IClassFixture<EsIndexFixture<TestEntity>>
    {
        private readonly EsIndexFixture<TestEntity> _fixture;
        private readonly TestEsLogger _log;

        public IndexFixtureBehavior(EsIndexFixture<TestEntity> fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _log = new TestEsLogger(output);
        }

        [Fact]
        public async Task ShouldCreateIndex()
        {
            //Act
            var indexResp = await _fixture.Manager.Client.Indices.GetAsync(_fixture.Manager.IndexName);

            _log.LogResponse(indexResp);

            var found = indexResp.Indices.Values.FirstOrDefault();

            //Assert
            Assert.NotNull(found);
        }
    }
}
