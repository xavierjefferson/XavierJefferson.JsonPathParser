using System.Text.RegularExpressions;
using XavierJefferson.JsonPathParser.Helpers;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Filtering.ValueNodes;

public class PatternNode : TypedValueNode<Regex>
{
    private readonly Regex _compiledPattern;
    private readonly string _flags;
    private readonly string _pattern;

    public override Regex Value => _compiledPattern;

    public PatternNode(string charSequence)
    {
        var begin = charSequence.IndexOf('/');
        var end = charSequence.LastIndexOf('/');
        _pattern = charSequence.Subsequence(begin + 1, end);
        var flagsIndex = end + 1;
        _flags = charSequence.Length > flagsIndex ? charSequence.Substring(flagsIndex) : "";
        _compiledPattern = new Regex(_pattern,
            PatternFlag.ParseFlags(_flags.Select(i => i).ToArray()) | RegexOptions.Compiled);
    }

    public PatternNode(Regex pattern)
    {
        _pattern = pattern.ToString();
        _compiledPattern = pattern;
        _flags = PatternFlag.ParseFlags(pattern.Options);
    }


    public override Type Type(IPredicateContext ctx)
    {
        return typeof(void);
    }

    public override PatternNode AsPatternNode()
    {
        return this;
    }

    public override int GetHashCode()
    {
        return _pattern.GetHashCode();
    }

    public override string ToString()
    {
        if (_pattern.StartsWith("/"))
            return _pattern;
        return $"/{_pattern}/{_flags}";
    }


    public override bool Equals(object? o)
    {
        if (this == o) return true;
        if (!(o is PatternNode)) return false;

        var that = (PatternNode)o;

        return !(_compiledPattern != null
            ? !_compiledPattern.Equals(that._compiledPattern)
            : that._compiledPattern != null);
    }
}