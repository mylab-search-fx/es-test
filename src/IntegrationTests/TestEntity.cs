using Nest;

namespace IntegrationTests
{
    [ElasticsearchType(IdProperty = nameof(Id))]
    public class TestEntity
    {
        [Number(NumberType.Integer, Name = "uid")]
        public int Id { get; set; }
        [Text(Name = "val")]
        public string Value { get; set; }
    }
}