using System;
using System.Threading.Tasks;
using MyLab.Elastic;
using MyLab.Elastic.Test;
using Xunit;

namespace IntegrationTests
{
    public class IndexFixtureBehavior : IClassFixture<EsIndexFixture<TestEntity>>
    {
        [Fact]
        public void ShouldCreateIndex()
        {
            //Assert is exception control when fixture initialized
        }
    }
}
