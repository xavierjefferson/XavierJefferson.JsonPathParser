using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

namespace XavierJefferson.JsonPathParser.UnitTests.TestData;

public class PatternFlagTestCases : TheoryData<PatternFlagTestCase>
{
    public static Lazy<List<PatternFlagTestCase>> LazyTestEntries { get; } = new(() =>
    {
        var options = new List<Tuple<RegexOptions, string>>
        {
            new(RegexOptions.IgnoreCase, "i"),
            new(RegexOptions.IgnorePatternWhitespace, "x"),
            new(RegexOptions.Multiline, "m"),
            new(RegexOptions.Singleline, "s")
        };


        var vals = Enumerable.Range(0, options.Count).Select(i => Convert.ToInt64(Math.Pow(2, i))).ToArray();
        var testEntries = new List<PatternFlagTestCase>();
        for (long i = 0; i < Math.Pow(2, options.Count); i++)
        {
            var sb = new StringBuilder();
            var regexOptions = RegexOptions.None;
            foreach (var m in options.Select((i, j) => new { Index = j, RegexOptions = i.Item1, Pattern = i.Item2 }))
                if ((i & vals[m.Index]) != 0)
                {
                    sb.Append(m.Pattern);
                    regexOptions = regexOptions | m.RegexOptions;
                }

            testEntries.Add(new PatternFlagTestCase { Pattern = sb.ToString(), RegexOptions = regexOptions });
        }

        return testEntries;
    });

    public PatternFlagTestCases()
    {
        foreach (var m in LazyTestEntries.Value)
        {
            this.Add(m);
        }
    }
}