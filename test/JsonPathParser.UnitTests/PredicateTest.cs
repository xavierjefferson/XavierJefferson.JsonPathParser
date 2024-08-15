using Moq;
using XavierJefferson.JsonPathParser.Interfaces;
using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests;

public class PredicateTest : TestUtils
{



    [Theory]

    [ClassData(typeof(ProviderTypeTestCases))]
    public void predicates_filters_can_be_applied(IProviderTypeTestCase testCase)
    {
        IReadContext Reader = JsonPath.Using(testCase.Configuration)
        .Parse((object)JsonTestData.JsonDocument);
        var m = new Mock<IPredicate>();

        m.Setup(i => i.ToString()).Returns(() => "Test123");
        m.Setup(i => i.Apply(It.IsAny<IPredicateContext>())).Returns((IPredicateContext context) =>
        {
            return context.GetItem<IDictionary<string, object?>>().ContainsKey("isbn");
        });

        var nn = Reader.Read<List<object?>>("$.store.book[?].isbn", m.Object);
        MyAssert.ContainsOnly(nn, "0-395-19395-8", "0-553-21311-3");
    }
}