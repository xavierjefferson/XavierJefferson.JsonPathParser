using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.Helpers;

namespace XavierJefferson.JsonPathParser;

public class CharacterIndex
{
    private const char OpenParenthesis = '(';
    private const char CloseParenthesis = ')';
    private const char CloseSquareBracket = ']';
    private const char Space = ' ';
    private const char Escape = '\\';
    private const char SingleQuote = '\'';
    private const char DoubleQuote = '"';
    private const char Minus = '-';
    private const char Period = '.';
    private const char Regex = '/';

    //workaround for issue: https://github.com/json-path/JsonPath/issues/590
    private const char SciUppercaseE = 'E';
    private const char SciLowercaseE = 'e';

    private readonly string _charSequence;

    public CharacterIndex(string charSequence)
    {
        _charSequence = charSequence;
        Position = 0;
        _endPosition = charSequence.Length - 1;
    }

    public int _endPosition { get; private set; }

    public int Position { get; private set; }

    public char this[int index] => _charSequence[index];

    public int Length => _endPosition + 1;

    public char CharAt(int idx)
    {
        return this[idx];
    }

    public char CurrentChar()
    {
        return _charSequence[Position];
    }

    public bool CurrentCharIs(char c)
    {
        return _charSequence[Position] == c;
    }

    public bool LastCharIs(char c)
    {
        return _charSequence[_endPosition] == c;
    }

    public bool NextCharIs(char c)
    {
        return InBounds(Position + 1) && _charSequence[Position + 1] == c;
    }

    public int IncrementPosition(int charCount)
    {
        return SetPosition(Position + charCount);
    }

    public int DecrementEndPosition(int charCount)
    {
        return SetEndPosition(_endPosition - charCount);
    }

    public int SetPosition(int newPosition)
    {
        //_position = Min(newPosition, charSequence.Length - 1);
        Position = newPosition;
        return Position;
    }

    private int SetEndPosition(int newPosition)
    {
        _endPosition = newPosition;
        return _endPosition;
    }


    public int IndexOfClosingSquareBracket(int startPosition)
    {
        var readPosition = startPosition;
        while (InBounds(readPosition))
        {
            if (CharAt(readPosition) == CloseSquareBracket) return readPosition;
            readPosition++;
        }

        return -1;
    }

    public int IndexOfMatchingCloseChar(int startPosition, char openChar, char closeChar, bool skipStrings,
        bool skipRegex)
    {
        if (CharAt(startPosition) != openChar)
            throw new InvalidPathException($"Expected {openChar} but found " + CharAt(startPosition));

        var opened = 1;
        var readPosition = startPosition + 1;
        while (InBounds(readPosition))
        {
            if (skipStrings)
            {
                var quoteChar = CharAt(readPosition);
                if (quoteChar == SingleQuote || quoteChar == DoubleQuote)
                {
                    readPosition = NextIndexOfUnescaped(readPosition, quoteChar);
                    if (readPosition == -1)
                        throw new InvalidPathException($"Could not find matching close quote for {quoteChar}" +
                                                       $" when parsing : {_charSequence}");
                    readPosition++;
                }
            }

            if (skipRegex)
                if (CharAt(readPosition) == Regex)
                {
                    readPosition = NextIndexOfUnescaped(readPosition, Regex);
                    if (readPosition == -1)
                        throw new InvalidPathException($"Could not find matching close for {Regex}" +
                                                       $" when parsing regex in : {_charSequence}");
                    readPosition++;
                }

            if (CharAt(readPosition) == openChar) opened++;
            if (CharAt(readPosition) == closeChar)
            {
                opened--;
                if (opened == 0) return readPosition;
            }

            readPosition++;
        }

        return -1;
    }

    public int IndexOfClosingBracket(int startPosition, bool skipStrings, bool skipRegex)
    {
        return IndexOfMatchingCloseChar(startPosition, OpenParenthesis, CloseParenthesis, skipStrings, skipRegex);
    }

    public int IndexOfNextSignificantChar(char c)
    {
        return IndexOfNextSignificantChar(Position, c);
    }

    public int IndexOfNextSignificantChar(int startPosition, char c)
    {
        var readPosition = startPosition + 1;
        while (!IsOutOfBounds(readPosition) && CharAt(readPosition) == Space) readPosition++;
        if (CharAt(readPosition) == c)
            return readPosition;
        return -1;
    }

    public int NextIndexOf(char c)
    {
        return NextIndexOf(Position + 1, c);
    }

    public int NextIndexOf(int startPosition, char c)
    {
        var readPosition = startPosition;
        while (!IsOutOfBounds(readPosition))
        {
            if (CharAt(readPosition) == c) return readPosition;
            readPosition++;
        }

        return -1;
    }

    public int NextIndexOfUnescaped(char c)
    {
        return NextIndexOfUnescaped(Position, c);
    }

    public int NextIndexOfUnescaped(int startPosition, char c)
    {
        var readPosition = startPosition + 1;
        var inEscape = false;
        while (!IsOutOfBounds(readPosition))
        {
            if (inEscape)
                inEscape = false;
            else if ('\\' == CharAt(readPosition))
                inEscape = true;
            else if (c == CharAt(readPosition)) return readPosition;
            readPosition++;
        }

        return -1;
    }

    public char CharAtOr(int postition, char defaultChar)
    {
        if (!InBounds(postition)) return defaultChar;
        return CharAt(postition);
    }

    public bool NextSignificantCharIs(int startPosition, char c)
    {
        var readPosition = startPosition + 1;
        while (!IsOutOfBounds(readPosition) && CharAt(readPosition) == Space) readPosition++;
        return !IsOutOfBounds(readPosition) && CharAt(readPosition) == c;
    }

    public bool NextSignificantCharIs(char c)
    {
        return NextSignificantCharIs(Position, c);
    }

    public char NextSignificantChar()
    {
        return NextSignificantChar(Position);
    }

    public char NextSignificantChar(int startPosition)
    {
        var readPosition = startPosition + 1;
        while (!IsOutOfBounds(readPosition) && CharAt(readPosition) == Space) readPosition++;
        if (!IsOutOfBounds(readPosition))
            return CharAt(readPosition);
        return ' ';
    }

    public void ReadSignificantChar(char c)
    {
        if (SkipBlanks().CurrentChar() != c) throw new InvalidPathException($"Expected character: {c}");
        IncrementPosition(1);
    }

    public bool HasSignificantSubSequence(string s)
    {
        SkipBlanks();
        if (!InBounds(Position + s.Length - 1)) return false;
        if (!Subsequence(Position, Position + s.Length).Equals(s)) return false;

        IncrementPosition(s.Length);
        return true;
    }

    public int IndexOfPreviousSignificantChar(int startPosition)
    {
        var readPosition = startPosition - 1;
        while (!IsOutOfBounds(readPosition) && CharAt(readPosition) == Space) readPosition--;
        if (!IsOutOfBounds(readPosition))
            return readPosition;
        return -1;
    }

    public int IndexOfPreviousSignificantChar()
    {
        return IndexOfPreviousSignificantChar(Position);
    }

    public char PreviousSignificantChar(int startPosition)
    {
        var previousSignificantCharIndex = IndexOfPreviousSignificantChar(startPosition);
        if (previousSignificantCharIndex == -1) return ' ';
        return CharAt(previousSignificantCharIndex);
    }

    public char PreviousSignificantChar()
    {
        return PreviousSignificantChar(Position);
    }

    public bool CurrentIsTail()
    {
        return Position >= _endPosition;
    }

    public bool HasMoreCharacters()
    {
        return InBounds(Position + 1);
    }

    public bool InBounds(int idx)
    {
        return idx >= 0 && idx <= _endPosition;
    }

    public bool InBounds()
    {
        return InBounds(Position);
    }

    public bool IsOutOfBounds(int idx)
    {
        return !InBounds(idx);
    }

    public string Subsequence(int start, int end)
    {
        return _charSequence.Subsequence(start, end);
    }

    public string CharSequence()
    {
        return _charSequence;
    }


    public override string ToString()
    {
        return _charSequence;
    }

    public bool IsNumberCharacter(int readPosition)
    {
        var c = CharAt(readPosition);
        //workaround for issue: https://github.com/json-path/JsonPath/issues/590
        return c.IsDigit() || c == Minus || c == Period || c == SciUppercaseE || c == SciLowercaseE;
    }

    public CharacterIndex SkipBlanks()
    {
        while (InBounds() && Position < _endPosition && CurrentChar() == Space) IncrementPosition(1);
        return this;
    }

    private CharacterIndex SkipBlanksAtEnd()
    {
        while (InBounds() && Position < _endPosition && LastCharIs(Space)) DecrementEndPosition(1);
        return this;
    }

    public CharacterIndex Trim()
    {
        SkipBlanks();
        SkipBlanksAtEnd();
        return this;
    }
}