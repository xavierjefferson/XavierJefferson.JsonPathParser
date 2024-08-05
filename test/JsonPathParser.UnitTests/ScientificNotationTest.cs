using XavierJefferson.JsonPathParser.UnitTests.Extensions;
using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests;

//test for issue: https://github.com/json-path/JsonPath/issues/590
public class ScientificNotationTest : TestUtils
{
    private readonly string _sciRepArray = "{\"num_array\": [" +
                                           "{\"num\":1}," +
                                           "{\"num\":-1e-10}," +
                                           "{\"num\":0.1e10}," +
                                           "{\"num\":2E-20}," +
                                           "{\"num\":-0.2E20}" +
                                           " ]}";

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void TestScientificNotation(IProviderTypeTestCase testCase)
    {
        var result = JsonPath.Using(testCase.Configuration)
            .Parse(_sciRepArray)
            .Read(
                "$.num_array[?(@.num == 1 || @.num == -1e-10 || @.num == 0.1e10 || @.num == 2E-20 || @.num == -0.2E20)]")
            .AsListOfMap();

        var numbers = GetNumbers(result);
        MyAssert.ContainsOnly(numbers, 1d, -1e-10d, .1e10d, 2e-20d, -.2E20d);
    }

    private List<object> GetNumbers(object? result)
    {
        var numbers = result.AsListOfMap().Where(i => i.ContainsKey("num")).Select(i => i["num"]).ToList();
        return numbers;
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void testScientificNotation_lt_gt(IProviderTypeTestCase testCase)
    {
        var result = JsonPath.Using(testCase.Configuration)
            .Parse(_sciRepArray)
            .Read("$.num_array[?(@.num > -0.0E0)]").AsListOfMap();
        var numbers = GetNumbers(result);
        MyAssert.ContainsOnly(numbers, 1d, 1e9, 2e-20);
        //Assert.Equal("[{\"num\":1},{\"num\":1.0E9},{\"num\":2.0E-20}]", result.ToString());

        result = JsonPath.Using(testCase.Configuration)
            .Parse(_sciRepArray)
            .Read("$.num_array[?(@.num < -0.0E0)]").AsListOfMap();
        numbers = GetNumbers(result);
        MyAssert.ContainsOnly(numbers, -1.0E-10, -2.0E19);
        //Assert.Equal("[{\"num\":-1.0E-10},{\"num\":-2.0E19}]", result.ToString());
    }
}