using System.Text;
using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.Extensions;

namespace XavierJefferson.JsonPathParser.Helpers;

public static class Assertions
{

    /// <summary>

    ///         Validate that the argument condition is <code>true</code>; otherwise
    ///         throwing an exception with the specified message. This method is useful when
    ///         validating according to an arbitrary bool expression, such as validating a
    ///         primitive number or using your own custom validation expression.
    ///     <code>Validate.isTrue(i > 0.0, "The value must be greater than zero: %d", i);</code>
    ///         For performance reasons, the long value is passed as a separate parameter and
    ///         appended to the exception message only in the case of an error.
    /// </summary>
    /// <param name="expression">the bool expression to check</param>
    /// <param name="message">* @ if expression is {@code false}</param>
    public static void IsTrue(bool expression, string message)
    {
        if (expression == false) throw new ArgumentException(message);
    }

    /// <summary>
    ///     Check if one and only one condition is true; otherwise
    ///     throw an exception with the specified message.
    /// </summary>
    /// <param name="message">error describing message</param>
    /// <param name="expressions">the bool expressions to check if zero or more than one expressions are true</param>
    public static void onlyOneIsTrue(string message, params bool[] expressions)
    {
        if (!OnlyOneIsTrueNonThrow(expressions)) throw new ArgumentException(message);
    }

    public static bool OnlyOneIsTrueNonThrow(params bool[] expressions)
    {
        var count = 0;
        foreach (var expression in expressions)
            if (expression && ++count > 1)
                return false;
        return 1 == count;
    }

    /// <summary>
    ///     <p>
    ///         Validate that the specified argument character sequence is
    ///         neither {@code null} nor a length of zero (no characters);
    ///         otherwise throwing an exception with the specified message.
    ///         <p />
    ///         <code>Validate.notEmpty(myString, "The string must not be empty");</code>
    /// </summary>
    /// <param name="T">the character sequence type</param>
    /// <param name="chars">the character sequence to check, validated not null by this method</param>
    /// <param name="message">the {@link string#format(string, params object[] )} exception message if invalid, not null</param>
    /// <returns> the validated character sequence (never {@code null} method for chaining)</returns>
    /// @     if the character sequence is {@code null}
    /// @ if the character sequence is empty
    public static string? NotEmpty(string? chars, string message)
    {
        if (string.IsNullOrEmpty(chars)) throw new InvalidOperationException(message);
        return chars;
    }

    /// <summary>
    ///         Validate that the specified argument character sequence is
    ///         neither {@code null} nor a length of zero (no characters);
    ///         otherwise throwing an exception with the specified message.
    ///         <code>Validate.notEmpty(myString, "The string must not be empty");</code>
    /// </summary>
    /// <param name="bytes">the bytes to check, validated not null by this method</param>
    /// <param name="message">the {@link string#format(string, params object[] )} exception message if invalid, not null</param>
    /// <returns> the validated character sequence (never {@code null} method for chaining)</returns>
    public static byte[] NotEmpty(byte[] bytes, string message)
    {
        if (bytes == null || bytes.Length == 0) throw new InvalidOperationException(message);
        return bytes;
    }
}