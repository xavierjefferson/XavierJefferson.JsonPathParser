using System.Text;
using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.Extensions;

namespace XavierJefferson.JsonPathParser.Helpers;

public static class StringHelper
{
    public static string? Subsequence(this string input, int start, int end)
    {
        if (start > input.Length) return null;
        var count = end - start;
        return input.Substring(start, count);
    }

    public static string Join(string? delimiter, string wrap, params object[] objs)
    {
        var vx = objs.Select(i => $"{wrap}{i}{wrap}").ToList();
        return string.Join(delimiter, vx);
    }

    public static string Concat(params string[]? strings)
    {
        if (strings == null || strings.Length == 0) return "";
        if (strings.Length == 1) return strings[0];
        var length = 0;
        // -1 = no result, -2 = multiple results
        var indexOfSingleNonEmptyString = -1;
        foreach (var tuple in strings.ToIndexedEnumerable())
            //for (int i = 0; i < strings.Length; i++)
        {
            var i = tuple.Index;
            var charSequence = tuple.Value;
            var len = charSequence.Length;
            length += len;
            if (indexOfSingleNonEmptyString != -2 && len > 0)
            {
                if (indexOfSingleNonEmptyString == -1)
                    indexOfSingleNonEmptyString = i;
                else
                    indexOfSingleNonEmptyString = -2;
            }
        }

        if (length == 0) return "";
        if (indexOfSingleNonEmptyString > 0) return strings[indexOfSingleNonEmptyString];
        var sb = new StringBuilder(length);
        foreach (var charSequence in strings) sb.Append(charSequence);
        return sb.ToString();
    }

    public static string? Escape(string? str, bool escapeSingleQuote)
    {
        if (str == null) return null;
        var len = str.Length;
        var writer = new StringWriter();

        for (var i = 0; i < len; i++)
        {
            var ch = str[i];

            // handle unicode
            if (ch > 0x7f)
            {
                writer.Write("\\u");
                writer.Write(((int)ch).ToString("X4"));
            }
            //if (ch > 0xfff )
            //{
            //    writer.Write("\\u" + hex(ch));
            //}
            //else if (ch > 0xff)
            //{
            //    writer.Write("\\u0" + hex(ch));
            //}
            //else if (ch > 0x7f)
            //{
            //    writer.Write("\\u00" + hex(ch));
            //}
            else if (ch < 32)
            {
                switch (ch)
                {
                    case '\b':
                        writer.Write('\\');
                        writer.Write('b');
                        break;
                    case '\n':
                        writer.Write('\\');
                        writer.Write('n');
                        break;
                    case '\t':
                        writer.Write('\\');
                        writer.Write('t');
                        break;
                    case '\f':
                        writer.Write('\\');
                        writer.Write('f');
                        break;
                    case '\r':
                        writer.Write('\\');
                        writer.Write('r');
                        break;
                    default:
                        writer.Write("\\u");
                        writer.Write(((int)ch).ToString("X4"));
                        break;
                }
            }
            else
            {
                switch (ch)
                {
                    case '\'':
                        if (escapeSingleQuote) writer.Write('\\');
                        writer.Write('\'');
                        break;
                    case '"':
                        writer.Write('\\');
                        writer.Write('"');
                        break;
                    case '\\':
                        writer.Write('\\');
                        writer.Write('\\');
                        break;
                    case '/':
                        writer.Write('\\');
                        writer.Write('/');
                        break;
                    default:
                        writer.Write(ch);
                        break;
                }
            }
        }

        return writer.ToString();
    }

    public static bool IsDigit(this char c)
    {
        return c >= '0' && c <= '9';
    }

    public static string? Unescape(string? str)
    {
        if (str == null) return null;
        var len = str.Length;
        var writer = new StringWriter();
        var unicode = new StringBuilder(4);
        var hadSlash = false;
        var inUnicode = false;
        for (var i = 0; i < len; i++)
        {
            var ch = str[i];
            if (inUnicode)
            {
                unicode.Append(ch);
                if (unicode.Length == 4)
                    try
                    {
                        var value = Convert.ToInt32($"0x{unicode}", 16);
                        writer.Write((char)value);
                        unicode.Clear();
                        inUnicode = false;
                        hadSlash = false;
                    }
                    catch (Exception nfe)
                    {
                        throw new JsonPathException("Unable to Parse unicode value: " + unicode, nfe);
                    }

                continue;
            }

            if (hadSlash)
            {
                hadSlash = false;
                switch (ch)
                {
                    case '\\':
                        writer.Write('\\');
                        break;
                    case '\'':
                        writer.Write('\'');
                        break;
                    case '\"':
                        writer.Write('"');
                        break;
                    case 'r':
                        writer.Write('\r');
                        break;
                    case 'f':
                        writer.Write('\f');
                        break;
                    case 't':
                        writer.Write('\t');
                        break;
                    case 'n':
                        writer.Write('\n');
                        break;
                    case 'b':
                        writer.Write('\b');
                        break;
                    case 'u':
                    {
                        inUnicode = true;
                        break;
                    }
                    default:
                        writer.Write(ch);
                        break;
                }

                continue;
            }

            if (ch == '\\')
            {
                hadSlash = true;
                continue;
            }

            writer.Write(ch);
        }

        if (hadSlash) writer.Write('\\');
        return writer.ToString();
    }

    /// <summary>
    ///     <p>Checks if a string is empty ("") or null.</p>
    ///     <p />
    ///     <pre>
    ///         StringUtils.isEmpty(null)      = true
    ///         StringUtils.isEmpty("")        = true
    ///         StringUtils.isEmpty(" ")       = false
    ///         StringUtils.isEmpty("bob")     = false
    ///         StringUtils.isEmpty("  bob  ") = false
    ///     </pre>
    ///     <p />
    ///     <p>
    ///         NOTE: This method changed in Lang version 2.0.
    ///         It no longer trims the string.
    ///         That functionality is available in isBlank().
    ///     </p>
    ///     @since 3.0 Changed signature from isEmpty(string) to isEmpty(string)
    /// </summary>
    /// <param name="cs">the string to check, may be null</param>
    /// <returns> {@code true} if the string is empty or null</returns>
    public static bool IsEmpty(string? cs)
    {
        return string.IsNullOrEmpty(cs);
    }

    /// <summary>
    ///     Used by the IndexOf(string methods) as a green implementation of IndexOf.
    /// </summary>
    /// <param name="cs">the {@code string} to be processed</param>
    /// <param name="searchChar">the {@code string} to be searched for</param>
    /// <param name="start">the start index</param>
    /// <returns> the index where the search sequence was found</returns>
    public static int IndexOf(string cs, string searchChar, int start)
    {
        return cs.IndexOf(searchChar, start);
    }
}