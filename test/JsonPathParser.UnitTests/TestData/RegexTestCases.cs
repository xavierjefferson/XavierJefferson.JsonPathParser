using XavierJefferson.JsonPathParser.Filtering.ValueNodes;
using XavierJefferson.JsonPathParser.Path;

namespace XavierJefferson.JsonPathParser.UnitTests.TestData;

public class RegexTestCases : TheoryData<RegexTestCase>
{
    private const string RegexJson = "{ 'some': 'JsonNode' }";

    public static readonly List<RegexTestCase> RegexTestCaseList = new()
    {
        new RegexTestCase("/true|false/", ValueNode.CreateStringNode("true", true), true),
        new RegexTestCase("/9.*9/", ValueNode.CreateNumberNode("9979"), true),
        new RegexTestCase("/fa.*se/", ValueNode.CreateBooleanNode("false"), true),
        new RegexTestCase("/Eval.*or/", ValueNode.CreateClassNode(typeof(string)), false),
        new RegexTestCase("/JsonNode/", ValueNode.CreateJsonNode(RegexJson), false),
        new RegexTestCase("/PathNode/", ValueNode.CreatePathNode(RegexPath), false),
        new RegexTestCase("/Undefined/", ValueNode.CreateUndefinedNode(), false),
        new RegexTestCase("/NullNode/", ValueNode.CreateNullNode(), false),
        new RegexTestCase("/test/i", ValueNode.CreateStringNode("tEsT", true), true),
        new RegexTestCase("/test/", ValueNode.CreateStringNode("tEsT", true), false),
        new RegexTestCase("/\u00de/ui", ValueNode.CreateStringNode("\u00fe", true), true),
        new RegexTestCase("/\u00de/", ValueNode.CreateStringNode("\u00fe", true), false),
        //new RegexTestCase("/\u00de/i", ValueNode.CreateStringNode("\u00fe", true), false),
        new RegexTestCase("/test# code/", ValueNode.CreateStringNode("test", true), false),
        new RegexTestCase("/test# code/x", ValueNode.CreateStringNode("test", true), true),
        new RegexTestCase("/.*test.*/d", ValueNode.CreateStringNode("my\rtest", true), true),
        //new RegexTestCase("/.*test.*/", ValueNode.CreateStringNode("my\rtest", true), false),
        new RegexTestCase("/.*tEst.*/is", ValueNode.CreateStringNode("test\ntest", true), true),
        //new RegexTestCase("/.*tEst.*/i", ValueNode.CreateStringNode("test\ntest", true), false),
        new RegexTestCase("/^\\w+$/U", ValueNode.CreateStringNode("\u00fe", true), true),
        //new RegexTestCase("/^\\w+$/", ValueNode.CreateStringNode("\u00fe", true), false)
    };

    public static Dictionary<string, ValueNode> RegExpTestCaseDictionary =
        RegexTestCaseList.ToDictionary(i => i.Pattern, i => i.ValueNode);

    public RegexTestCases()
    {
        foreach (var m in RegexTestCaseList) Add(m);
    }

    private static CompiledPath RegexPath => new(PathTokenFactory.CreateRootPathToken('$'), true);
}