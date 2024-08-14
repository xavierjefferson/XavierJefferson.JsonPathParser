using System.Text;
using System.Text.RegularExpressions;

namespace XavierJefferson.JsonPathParser.Filtering;

public class PatternFlag
{
    //static readonly PatternFlag UNIX_LINES = new(Regex.UNIX_LINES, 'd');
    private static readonly PatternFlag CaseInsensitive = new(RegexOptions.IgnoreCase, 'i');
    private static readonly PatternFlag Comments = new(RegexOptions.IgnorePatternWhitespace, 'x');
    private static readonly PatternFlag Multiline = new(RegexOptions.Multiline, 'm');
    private static readonly PatternFlag DotAll = new(RegexOptions.Singleline, 's');
    private static readonly PatternFlag CultureInvariant = new(RegexOptions.CultureInvariant, 'U');

    private static readonly SerializingList<PatternFlag> Values = new()
    {
        //UNIX_LINES, 
        CaseInsensitive, Comments, Multiline, DotAll, CultureInvariant
        //UNICODE_CASE, 
        //UNICODE_CHARACTER_CLASS
    };
    //static readonly PatternFlag UNICODE_CASE = new(Regex.UNICODE_CASE, 'u');
    // static readonly PatternFlag UNICODE_CHARACTER_CLASS = new(Regex.UNICODE_CHARACTER_CLASS, 'U');

    private readonly RegexOptions _code;
    private readonly char _flag;

    private PatternFlag(RegexOptions code, char flag)
    {
        _code = code;
        _flag = flag;
    }

    public static RegexOptions ParseFlags(string? flags)
    {
        return ParseFlags(flags.Select(i => i).ToArray());
    }

    public static RegexOptions ParseFlags(char[] flags)
    {
        RegexOptions flagsValue = 0;
        foreach (var flag in flags) flagsValue |= GetCodeByFlag(flag);
        return flagsValue;
    }

    public static string ParseFlags(RegexOptions flags)
    {
        var builder = new StringBuilder();
        foreach (var patternFlag in Values)
            if ((patternFlag._code & flags) == patternFlag._code)
                builder.Append(patternFlag._flag);
        return builder.ToString();
    }

    private static RegexOptions GetCodeByFlag(char flag)
    {
        foreach (var patternFlag in Values)
            if (patternFlag._flag == flag)
                return patternFlag._code;
        return 0;
    }
}