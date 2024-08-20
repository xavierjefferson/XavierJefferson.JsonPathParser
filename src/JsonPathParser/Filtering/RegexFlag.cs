using System.Text;
using System.Text.RegularExpressions;

namespace XavierJefferson.JsonPathParser.Filtering;

public class RegexFlag
{
    //static readonly PatternFlag UNIX_LINES = new(Regex.UNIX_LINES, 'd');
    private static readonly RegexFlag CaseInsensitive = new(RegexOptions.IgnoreCase, 'i');
    private static readonly RegexFlag Comments = new(RegexOptions.IgnorePatternWhitespace, 'x');
    private static readonly RegexFlag Multiline = new(RegexOptions.Multiline, 'm');
    private static readonly RegexFlag DotAll = new(RegexOptions.Singleline, 's');
    private static readonly RegexFlag CultureInvariant = new(RegexOptions.CultureInvariant, 'U');

    private static readonly List<RegexFlag> Values = new()
    {
        //UNIX_LINES, 
        CaseInsensitive, Comments, Multiline, DotAll, CultureInvariant
        //UNICODE_CASE, 
        //UNICODE_CHARACTER_CLASS
    };

    private RegexFlag(RegexOptions regexOptions, char flag)
    {
        RegexOptions = regexOptions;
        Flag = flag;
    }
    //static readonly PatternFlag UNICODE_CASE = new(Regex.UNICODE_CASE, 'u');
    // static readonly PatternFlag UNICODE_CHARACTER_CLASS = new(Regex.UNICODE_CHARACTER_CLASS, 'U');

    public RegexOptions RegexOptions { get; }
    public char Flag { get; }

    public static RegexOptions ParseFlags(string? flags)
    {
        return ParseFlags(flags.ToArray());
    }

    public static RegexOptions ParseFlags(char[] flags)
    {
        RegexOptions flagsValue = 0;
        foreach (var flag in flags) flagsValue |= GetRegexOptionByFlag(flag);
        return flagsValue;
    }

    public static string ParseFlags(RegexOptions flags)
    {
        var builder = new StringBuilder();
        foreach (var patternFlag in Values.Where(patternFlag => (patternFlag.RegexOptions & flags) != 0))
            builder.Append(patternFlag.Flag);
        return builder.ToString();
    }

    private static RegexOptions GetRegexOptionByFlag(char flag)
    {
        return Values.Where(i => i.Flag == flag).Select(i => i.RegexOptions).FirstOrDefault();
    }
}