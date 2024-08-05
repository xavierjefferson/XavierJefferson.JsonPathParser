using Moq;
using XavierJefferson.JsonPathParser.Interfaces;
using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests;

public class PredicateTest : TestUtils
{
    private static readonly IReadContext Reader = JsonPath.Using(ConfigurationData.SystemTextJsonConfiguration)
        .Parse((object)JsonTestData.JsonDocument);

    [Fact]
    public void predicates_filters_can_be_applied()
    {
        var m = new Mock<IPredicate>();

        m.Setup(i => i.ToString()).Returns(() => "Test123");
        m.Setup(i => i.Apply(It.IsAny<IPredicateContext>())).Returns((IPredicateContext ctx) =>
        {
            return ctx.GetItem<JpDictionary>().ContainsKey("isbn");
        });

        var nn = Reader.Read<JpObjectList>("$.store.book[?].isbn", m.Object);
        MyAssert.ContainsOnly(nn, "0-395-19395-8", "0-553-21311-3");
    }
}