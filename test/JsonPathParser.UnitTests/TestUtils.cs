using XavierJefferson.JsonPathParser.Interfaces;
using XavierJefferson.JsonPathParser.Path;
using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests;

public class TestUtils
{
    public static List<object?> AsList(params object?[] items)
    {
        return new List<object?>(items);
    }


    public static Stream GetResourceAsStream(string path)
    {
        var name = typeof(TestUtils).Assembly.GetManifestResourceNames()
            .FirstOrDefault(i => i.EndsWith(path, StringComparison.InvariantCultureIgnoreCase));
        return typeof(TestUtils).Assembly.GetManifestResourceStream(name);
    }

    public IDictionary<string, object?> GetSingletonMap(string key, object? value)
    {
        return new Dictionary<string, object?> { { key, value } };
    }

    public IPredicateContext CreatePredicateContext(object check, IProviderTypeTestCase testCase)
    {
        return new PredicateContextImpl(check, check, testCase.Configuration,
            new Dictionary<IPath, object?>());
    }
}