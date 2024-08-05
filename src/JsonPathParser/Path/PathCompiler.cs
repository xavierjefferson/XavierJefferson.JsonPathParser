using System.Text;
using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.Filtering;
using XavierJefferson.JsonPathParser.Function;
using XavierJefferson.JsonPathParser.Helpers;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Path;

public class PathCompiler
{
    private const char DocContext = '$';
    private const char EvalContext = '@';

    private const char OpenSquareBracket = '[';
    private const char CloseSquareBracket = ']';
    private const char OpenParenthesis = '(';
    private const char CloseParenthesis = ')';
    private const char OpenBrace = '{';
    private const char CloseBrace = '}';

    private const char Wildcard = '*';
    private const char Period = '.';
    private const char Space = ' ';
    private const char Tab = '\t';
    private const char Cr = '\r';
    private const char Lf = '\n';
    private const char BeginFilter = '?';
    private const char Comma = ',';
    private const char Split = ':';
    private const char Minus = '-';
    private const char SingleQuote = '\'';
    private const char DoubleQuote = '"';

    private readonly Stack<IPredicate> _filterStack;
    private readonly CharacterIndex _path;

    public PathCompiler(string path, Stack<IPredicate> filterStack) : this(new CharacterIndex(path), filterStack)
    {
    }

    public PathCompiler(CharacterIndex path, Stack<IPredicate> filterStack)
    {
        this._filterStack = filterStack;
        this._path = path;
    }

    private IPath Compile()
    {
        var root = ReadContextToken();
        return new CompiledPath(root, root.GetPathFragment().Equals("$"));
    }

    public static IPath Compile(string path, params IPredicate[]? filters)
    {
        try
        {
            var ci = new CharacterIndex(path);
            ci.Trim();

            if (!(ci[0] == DocContext) && !(ci[0] == EvalContext))
            {
                ci = new CharacterIndex($"$.{path}");
                ci.Trim();
            }

            if (ci.LastCharIs('.')) Fail("Path must not end with a '.' or '..'");
            var filterStack = new Stack<IPredicate>(filters);
            return new PathCompiler(ci, filterStack).Compile();
        }
        catch (Exception e)
        {
            if (e is InvalidPathException)
                throw;
            throw new InvalidPathException(e);
        }
    }

    private void ReadWhitespace()
    {
        while (_path.InBounds())
        {
            var c = _path.CurrentChar();
            if (!IsWhitespace(c)) break;
            _path.IncrementPosition(1);
        }
    }

    private bool IsPathContext(char c)
    {
        return c == DocContext || c == EvalContext;
    }

    //[$ | @]
    private RootPathToken ReadContextToken()
    {
        ReadWhitespace();

        if (!IsPathContext(_path.CurrentChar())) throw new InvalidPathException("Path must start with '$' or '@'");

        var pathToken = PathTokenFactory.CreateRootPathToken(_path.CurrentChar());

        if (_path.CurrentIsTail()) return pathToken;

        _path.IncrementPosition(1);

        if (_path.CurrentChar() != Period && _path.CurrentChar() != OpenSquareBracket)
            Fail($"Illegal character at position {_path.Position}{" expected '.' or '['"}");

        var appender = pathToken.GetPathTokenAppender();
        ReadNextToken(appender);

        return pathToken;
    }

    //
    //
    //
    private bool ReadNextToken(IPathTokenAppender appender)
    {
        var c = _path.CurrentChar();

        switch (c)
        {
            case OpenSquareBracket:
                if (!ReadBracketPropertyToken(appender) && !ReadArrayToken(appender) && !ReadWildCardToken(appender)
                    && !ReadFilterToken(appender) && !ReadPlaceholderToken(appender))
                    Fail($"Could not Parse token starting at position {_path.Position}. Expected ?, ', 0-9, *");
                return true;
            case Period:
                if (!ReadDotToken(appender)) Fail("Could not Parse token starting at position " + _path.Position);
                return true;
            case Wildcard:
                if (!ReadWildCardToken(appender)) Fail("Could not Parse token starting at position " + _path.Position);
                return true;
            default:
                if (!ReadPropertyOrFunctionToken(appender))
                    Fail("Could not Parse token starting at position " + _path.Position);
                return true;
        }
    }

    //
    // . and ..
    //
    private bool ReadDotToken(IPathTokenAppender appender)
    {
        if (_path.CurrentCharIs(Period) && _path.NextCharIs(Period))
        {
            appender.AppendPathToken(PathTokenFactory.CrateScanToken());
            _path.IncrementPosition(2);
        }
        else if (!_path.HasMoreCharacters())
        {
            throw new InvalidPathException("Path must not end with a '.");
        }
        else
        {
            _path.IncrementPosition(1);
        }

        if (_path.CurrentCharIs(Period))
            throw new InvalidPathException($"Character '.' on position {_path.Position}" + " is not valid.");
        return ReadNextToken(appender);
    }

    //
    // fooBar or fooBar()
    //
    private bool ReadPropertyOrFunctionToken(IPathTokenAppender appender)
    {
        if (_path.CurrentCharIs(OpenSquareBracket) || _path.CurrentCharIs(Wildcard) || _path.CurrentCharIs(Period) ||
            _path.CurrentCharIs(Space)) return false;
        var startPosition = _path.Position;
        var readPosition = startPosition;
        var endPosition = 0;

        var isFunction = false;

        while (_path.InBounds(readPosition))
        {
            var c = _path[readPosition];
            if (c == Space)
                throw new InvalidPathException(
                    $"Use bracket notion ['my prop'] if your property contains blank characters. position: {_path.Position}");

            if (c == Period || c == OpenSquareBracket)
            {
                endPosition = readPosition;
                break;
            }

            if (c == OpenParenthesis)
            {
                isFunction = true;
                endPosition = readPosition;
                break;
            }

            readPosition++;
        }

        if (endPosition == 0) endPosition = _path.Length;


        SerializingList<Parameter> functionParameters = null;
        if (isFunction)
        {
            var parenthesisCount = 1;

            for (var i = readPosition + 1; i < _path.Length; i++)
            {
                if (_path[i] == CloseParenthesis)
                    parenthesisCount--;
                else if (_path[i] == OpenParenthesis)
                    parenthesisCount++;
                if (parenthesisCount == 0)
                    break;
            }

            if (parenthesisCount != 0)
            {
                var functionName = _path.Subsequence(startPosition, endPosition);
                throw new InvalidPathException($"Arguments to function: '{functionName}" +
                                               "' are not closed properly.");
            }

            if (_path.InBounds(readPosition + 1))
            {
                // read the next token to determine if we have a simple no-args function call
                var c = _path[readPosition + 1];
                if (c != CloseParenthesis)
                {
                    _path.SetPosition(endPosition + 1);
                    // Parse the arguments of the function - arguments that are inner queries or JSON document(s)
                    var functionName = _path.Subsequence(startPosition, endPosition);
                    functionParameters = ParseFunctionParameters(functionName);
                }
                else
                {
                    _path.SetPosition(readPosition + 1);
                }
            }
            else
            {
                _path.SetPosition(readPosition);
            }
        }
        else
        {
            _path.SetPosition(endPosition);
        }

        var property = _path.Subsequence(startPosition, endPosition);
        if (isFunction)
            appender.AppendPathToken(PathTokenFactory.CreateFunctionPathToken(property, functionParameters));
        else
            appender.AppendPathToken(PathTokenFactory.CreateSinglePropertyPathToken(property, SingleQuote));

        return _path.CurrentIsTail() || ReadNextToken(appender);
    }

    /// <summary>
    ///     Parse the parameters of a function call, either the caller has supplied JSON data, or the caller has supplied
    ///     another path expression which must be evaluated and in turn invoked against the root document.  In this tokenizer
    ///     we're only concerned with parsing the path thus the output of this function is a list of parameters with the Path
    ///     set if the parameter is an expression.  If the parameter is a JSON document then the value of the cachedValue is
    ///     set on the object.
    ///     * Sequence for parsing out the parameters:
    ///     * This code has its own tokenizer - it does some rudimentary level of lexing in that it can distinguish between
    ///     JSON block parameters
    ///     and sub-JSON blocks - it effectively regex's out the parameters into string blocks that can then be passed along to
    ///     the appropriate parser.
    ///     Since sub-jsonpath expressions can themselves contain other function calls this routine needs to be sensitive to
    ///     token counting to
    ///     determine the boundaries.  Since the Path parser isn't aware of JSON processing this uber routine is needed.
    ///     * Parameters are separated by COMMAs ','
    ///     *
    ///     <code>
    ///  doc = {"numbers": [1,2,3,4,5,6,7,8,9,10]}
    ///  * $.sum({10}, $.numbers.avg())
    ///  </code>
    ///     The above is a valid function call, we're first summing 10 + avg of params 1[] 10 (5.5) so the total should be 15.5
    /// </summary>
    /// <returns>
    ///     An ordered list of parameters that are to processed via the function.  Typically functions either process
    ///     an array of values and/or can consume parameters in addition to the values provided from the consumption of
    ///     an array.
    /// </returns>
    private SerializingList<Parameter> ParseFunctionParameters(string funcName)
    {
        ParamType? type = null;

        // Parenthesis starts at 1 since we're marking the start of a function call, the close paren will denote the
        // last parameter boundary
        int groupParen = 1, groupBracket = 0, groupBrace = 0, groupQuote = 0;
        var endOfStream = false;
        var priorChar = (char)0;
        var parameters = new SerializingList<Parameter>();
        var parameter = new StringBuilder();
        while (_path.InBounds() && !endOfStream)
        {
            var c = _path.CurrentChar();
            _path.IncrementPosition(1);

            // we're at the start of the stream, and don't know what type of parameter we have
            if (type == null)
            {
                if (IsWhitespace(c)) continue;

                if (c == OpenBrace || c.IsDigit() || DoubleQuote == c || Minus == c)
                    type = ParamType.Json;
                else if
                    (IsPathContext(c))
                    type = ParamType.Path; // read until we reach a terminating comma and we've reset grouping to zero
            }

            switch (c)
            {
                case DoubleQuote:
                    if (priorChar != '\\' && groupQuote > 0)
                        groupQuote--;
                    else
                        groupQuote++;
                    break;
                case OpenParenthesis:
                    groupParen++;
                    break;
                case OpenBrace:
                    groupBrace++;
                    break;
                case OpenSquareBracket:
                    groupBracket++;
                    break;

                case CloseBrace:
                    if (0 == groupBrace)
                        throw new InvalidPathException($"Unexpected close brace '}}' at character position: {_path.Position}");
                    groupBrace--;
                    break;
                case CloseSquareBracket:
                    if (0 == groupBracket)
                        throw new InvalidPathException($"Unexpected close bracket ']' at character position: {_path.Position}");
                    groupBracket--;
                    break;

                // In either the close paren case where we have zero paren groups left, capture the parameter, or where
                // we've encountered a COMMA do the same
                case CloseParenthesis:
                    groupParen--;
                    //CS304 Issue link: https://github.com/json-path/JsonPath/issues/620
                    if (0 > groupParen || priorChar == '(') parameter.Append(c);
                    ProcessComma(ref type, groupParen, groupBracket, groupBrace, groupQuote, ref endOfStream,
                        parameters, parameter, c);
                    break;
                case Comma:
                    ProcessComma(ref type, groupParen, groupBracket, groupBrace, groupQuote, ref endOfStream,
                        parameters, parameter, c);
                    break;
            }

            if (type != null && !(c == Comma && 0 == groupBrace && 0 == groupBracket && 1 == groupParen))
                parameter.Append(c);
            priorChar = c;
        }

        if (0 != groupBrace || 0 != groupParen || 0 != groupBracket)
            throw new InvalidPathException($"Arguments to function: '{funcName}" + "' are not closed properly.");
        return parameters;
    }

    private static void ProcessComma(ref ParamType? type, int groupParen, int groupBracket, int groupBrace,
        int groupQuote, ref bool endOfStream, SerializingList<Parameter> parameters, StringBuilder parameter, char c)
    {
        // In this state we've reach the end of a function parameter and we can pass along the parameter string
        // to the parser
        if (0 == groupQuote && 0 == groupBrace && 0 == groupBracket
            && ((0 == groupParen && CloseParenthesis == c) || 1 == groupParen))
        {
            endOfStream = 0 == groupParen;

            if (null != type)
            {
                Parameter? param = null;
                switch (type)
                {
                    case ParamType.Json:
                        // Parse the json and set the value
                        param = new Parameter(parameter.ToString());
                        break;
                    case ParamType.Path:
                        var predicates = new Stack<IPredicate>();
                        var compiler = new PathCompiler(parameter.ToString(), predicates);
                        param = new Parameter(compiler.Compile());
                        break;
                }

                if (null != param) parameters.Add(param);
                parameter.Clear();
                type = null;
            }
        }
    }

    private bool IsWhitespace(char c)
    {
        return c == Space || c == Tab || c == Lf || c == Cr;
    }

    //
    // [?], [?,?, ..]
    //
    private bool ReadPlaceholderToken(IPathTokenAppender appender)
    {
        if (!_path.CurrentCharIs(OpenSquareBracket)) return false;
        var questionmarkIndex = _path.IndexOfNextSignificantChar(BeginFilter);
        if (questionmarkIndex == -1) return false;
        var nextSignificantChar = _path.NextSignificantChar(questionmarkIndex);
        if (nextSignificantChar != CloseSquareBracket && nextSignificantChar != Comma) return false;

        var expressionBeginIndex = _path.Position + 1;
        var expressionEndIndex = _path.NextIndexOf(expressionBeginIndex, CloseSquareBracket);

        if (expressionEndIndex == -1) return false;

        var expression = _path.Subsequence(expressionBeginIndex, expressionEndIndex);

        var tokens = expression.Split(",");

        if (_filterStack.Count() < tokens.Length)
            throw new InvalidPathException($"Not enough predicates supplied for filter [{expression}" +
                                           "] at position " + _path.Position);

        ICollection<IPredicate> predicates = new SerializingList<IPredicate>();
        foreach (var token1 in tokens)
        {
            var token = token1;
            token = token != null ? token.Trim() : null;
            if (!"?".Equals(token == null ? "" : token))
                throw new InvalidPathException("Expected '?' but found " + token);
            predicates.Add(_filterStack.Pop());
        }

        appender.AppendPathToken(PathTokenFactory.CreatePredicatePathToken(predicates));

        _path.SetPosition(expressionEndIndex + 1);

        return _path.CurrentIsTail() || ReadNextToken(appender);
    }

    //
    // [?(...)]
    //
    private bool ReadFilterToken(IPathTokenAppender appender)
    {
        if (!_path.CurrentCharIs(OpenSquareBracket) && !_path.NextSignificantCharIs(BeginFilter)) return false;

        var openStatementBracketIndex = _path.Position;
        var questionMarkIndex = _path.IndexOfNextSignificantChar(BeginFilter);
        if (questionMarkIndex == -1) return false;
        var openBracketIndex = _path.IndexOfNextSignificantChar(questionMarkIndex, OpenParenthesis);
        if (openBracketIndex == -1) return false;
        var closeBracketIndex = _path.IndexOfClosingBracket(openBracketIndex, true, true);
        if (closeBracketIndex == -1) return false;
        if (!_path.NextSignificantCharIs(closeBracketIndex, CloseSquareBracket)) return false;
        var closeStatementBracketIndex = _path.IndexOfNextSignificantChar(closeBracketIndex, CloseSquareBracket);

        var criteria = _path.Subsequence(openStatementBracketIndex, closeStatementBracketIndex + 1);


        IPredicate predicate = FilterCompiler.Compile(criteria);
        appender.AppendPathToken(PathTokenFactory.CreatePredicatePathToken(predicate));

        _path.SetPosition(closeStatementBracketIndex + 1);

        return _path.CurrentIsTail() || ReadNextToken(appender);
    }

    //
    // [*]
    // *
    //
    private bool ReadWildCardToken(IPathTokenAppender appender)
    {
        var inBracket = _path.CurrentCharIs(OpenSquareBracket);

        if (inBracket && !_path.NextSignificantCharIs(Wildcard)) return false;
        if (!_path.CurrentCharIs(Wildcard) && _path.IsOutOfBounds(_path.Position + 1)) return false;
        if (inBracket)
        {
            var wildCardIndex = _path.IndexOfNextSignificantChar(Wildcard);
            if (!_path.NextSignificantCharIs(wildCardIndex, CloseSquareBracket))
            {
                var offset = wildCardIndex + 1;
                throw new InvalidPathException($"Expected wildcard token to end with ']' on position {offset}");
            }

            var bracketCloseIndex = _path.IndexOfNextSignificantChar(wildCardIndex, CloseSquareBracket);
            _path.SetPosition(bracketCloseIndex + 1);
        }
        else
        {
            _path.IncrementPosition(1);
        }

        appender.AppendPathToken(PathTokenFactory.CreateWildCardPathToken());

        return _path.CurrentIsTail() || ReadNextToken(appender);
    }

    //
    // [1], [1,2, n], [1:], [1:2], [:2]
    //
    private bool ReadArrayToken(IPathTokenAppender appender)
    {
        if (!_path.CurrentCharIs(OpenSquareBracket)) return false;
        var nextSignificantChar = _path.NextSignificantChar();
        if (!nextSignificantChar.IsDigit() && nextSignificantChar != Minus && nextSignificantChar != Split)
            return false;

        var expressionBeginIndex = _path.Position + 1;
        var expressionEndIndex = _path.NextIndexOf(expressionBeginIndex, CloseSquareBracket);

        if (expressionEndIndex == -1) return false;

        var expression = _path.Subsequence(expressionBeginIndex, expressionEndIndex).Trim();

        if ("*".Equals(expression)) return false;

        //check valid chars
        for (var i = 0; i < expression.Length; i++)
        {
            var c = expression[i];
            if (!c.IsDigit() && c != Comma && c != Minus && c != Split && c != Space) return false;
        }

        var isSliceOperation = expression.Contains(":");

        if (isSliceOperation)
        {
            var arraySliceOperation = ArraySliceOperation.Parse(expression);
            appender.AppendPathToken(PathTokenFactory.CreateSliceArrayPathToken(arraySliceOperation));
        }
        else
        {
            var arrayIndexOperation = ArrayIndexOperation.Parse(expression);
            appender.AppendPathToken(PathTokenFactory.CreateIndexArrayPathToken(arrayIndexOperation));
        }

        _path.SetPosition(expressionEndIndex + 1);

        return _path.CurrentIsTail() || ReadNextToken(appender);
    }

    //
    // ['foo']
    //
    private bool ReadBracketPropertyToken(IPathTokenAppender appender)
    {
        if (!_path.CurrentCharIs(OpenSquareBracket)) return false;
        var potentialStringDelimiter = _path.NextSignificantChar();
        if (potentialStringDelimiter != SingleQuote && potentialStringDelimiter != DoubleQuote) return false;

        var properties = new SerializingList<string>();

        var startPosition = _path.Position + 1;
        var readPosition = startPosition;
        var endPosition = 0;
        var inProperty = false;
        var inEscape = false;
        var lastSignificantWasComma = false;

        while (_path.InBounds(readPosition))
        {
            var c = _path[readPosition];

            if (inEscape)
            {
                inEscape = false;
            }
            else if ('\\' == c)
            {
                inEscape = true;
            }
            else if (c == CloseSquareBracket && !inProperty)
            {
                if (lastSignificantWasComma) Fail($"Found empty property at index {readPosition}");
                break;
            }
            else if (c == potentialStringDelimiter)
            {
                if (inProperty)
                {
                    var nextSignificantChar = _path.NextSignificantChar(readPosition);
                    if (nextSignificantChar != CloseSquareBracket && nextSignificantChar != Comma)
                        Fail(
                            $"Property must be separated by comma or Property must be terminated close square bracket at index {readPosition}");
                    endPosition = readPosition;
                    var prop = _path.Subsequence(startPosition, endPosition);
                    properties.Add(StringHelper.Unescape(prop));
                    inProperty = false;
                }
                else
                {
                    startPosition = readPosition + 1;
                    inProperty = true;
                    lastSignificantWasComma = false;
                }
            }
            else if (c == Comma && !inProperty)
            {
                if (lastSignificantWasComma) Fail($"Found empty property at index {readPosition}");
                lastSignificantWasComma = true;
            }

            readPosition++;
        }

        if (inProperty) Fail($"Property has not been closed - missing closing {potentialStringDelimiter}");

        var endBracketIndex = _path.IndexOfNextSignificantChar(endPosition, CloseSquareBracket);
        if (endBracketIndex == -1) Fail("Property has not been closed - missing closing ]");
        endBracketIndex++;

        _path.SetPosition(endBracketIndex);

        appender.AppendPathToken(PathTokenFactory.CreatePropertyPathToken(properties, potentialStringDelimiter));

        return _path.CurrentIsTail() || ReadNextToken(appender);
    }

    public static bool Fail(string message)
    {
        throw new InvalidPathException(message);
    }
}