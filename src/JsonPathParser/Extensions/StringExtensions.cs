using XavierJefferson.JsonPathParser.Jsbeautifier;

namespace XavierJefferson.JsonPathParser.Extensions;

internal static class StringExtensions
{
    public static string? Beautify(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var b = new Beautifier(new BeautifierOptions
            { IndentWithTabs = false, KeepArrayIndentation = false, PreserveNewlines = true });
        return b.Beautify(value);
    }
}