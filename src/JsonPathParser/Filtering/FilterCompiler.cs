using System.Diagnostics;
using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.Filtering.ValueNodes;
using XavierJefferson.JsonPathParser.Interfaces;
using XavierJefferson.JsonPathParser.Logging;

namespace XavierJefferson.JsonPathParser.Filtering;

public class FilterCompiler
{
    private const char DocContext = '$';
    private const char EvalContext = '@';

    private const char OpenSquareBracket = '[';
    private const char CloseSquareBracket = ']';
    private const char OpenParenthesis = '(';
    private const char CloseParenthesis = ')';
    private const char OpenObject = '{';
    private const char CloseObject = '}';
    private const char OpenArray = '[';
    private const char CloseArray = ']';

    private const char SingleQuote = '\'';
    private const char DoubleQuote = '"';

    private const char Space = ' ';
    private const char Period = '.';

    private const char And = '&';
    private const char Or = '|';

    private const char Minus = '-';
    private const char Lt = '<';
    private const char Gt = '>';
    private const char Eq = '=';
    private const char Tilde = '~';
    private const char True = 't';
    private const char False = 'f';
    private const char Null = 'n';
    private const char Not = '!';
    private const char Pattern = '/';
    private const char IgnoreCase = 'i';
    private static readonly ILog Logger = LoggerFactory.GetLogger(typeof(FilterCompiler));

    private readonly CharacterIndex _filter;

    private FilterCompiler(string filterString)
    {
        _filter = new CharacterIndex(filterString);
        _filter.Trim();
        if (!_filter.CurrentCharIs('[') || !_filter.LastCharIs(']'))
            throw new InvalidPathException($"Filter must start with '[' and end with ']'. {filterString}");
        _filter.IncrementPosition(1);
        _filter.DecrementEndPosition(1);
        _filter.Trim();
        if (!_filter.CurrentCharIs('?'))
            throw new InvalidPathException($"Filter must start with '[?' and end with ']'. {filterString}");
        _filter.IncrementPosition(1);
        _filter.Trim();
        if (!_filter.CurrentCharIs('(') || !_filter.LastCharIs(')'))
            throw new InvalidPathException($"Filter must start with '[?(' and end with ')]'. {filterString}");
    }

    public static Filter Compile(string filterString)
    {
        var compiler = new FilterCompiler(filterString);
        return new CompiledFilter(compiler.Compile());
    }

    public IPredicate Compile()
    {
        try
        {
            var result = ReadLogicalOr();
            _filter.SkipBlanks();
            if (_filter.InBounds())
                throw new InvalidPathException(
                    $"Expected end of filter expression instead of: {_filter.Subsequence(_filter.Position, _filter.Length)}");

            return result;
        }
        catch (InvalidPathException e)
        {
            throw;
        }
        catch (Exception e)
        {
            throw new InvalidPathException($"Failed to Parse filter: {_filter}" + ", error on position: " +
                                           _filter.Position + ", char: " + _filter.CurrentChar(), e);
        }
    }

    private ValueNode ReadValueNode()
    {
        switch (_filter.SkipBlanks().CurrentChar())
        {
            case DocContext: return ReadPath();
            case EvalContext: return ReadPath();
            case Not:
                _filter.IncrementPosition(1);
                switch (_filter.SkipBlanks().CurrentChar())
                {
                    case DocContext: return ReadPath();
                    case EvalContext: return ReadPath();
                    default: throw new InvalidPathException($"Unexpected character: {Not}");
                }
            default: return ReadLiteral();
        }
    }

    private ValueNode ReadLiteral()
    {
        switch (_filter.SkipBlanks().CurrentChar())
        {
            case SingleQuote: return ReadStringLiteral(SingleQuote);
            case DoubleQuote: return ReadStringLiteral(DoubleQuote);
            case True: return ReadBooleanLiteral();
            case False: return ReadBooleanLiteral();
            case Minus: return ReadNumberLiteral();
            case Null: return ReadNullLiteral();
            case OpenObject: return ReadJsonLiteral();
            case OpenArray: return ReadJsonLiteral();
            case Pattern: return ReadPattern();
            default: return ReadNumberLiteral();
        }
    }

    /// <summary>
    ///     LogicalOR               = LogicalAND { '||' LogicalAND }
    ///     LogicalAND              = LogicalANDOperand { '&&' LogicalANDOperand }
    ///     LogicalANDOperand       = RelationalExpression | '(' LogicalOR ')' | '!' LogicalANDOperand
    ///     RelationalExpression    = Value [ RelationalOperator Value ]
    /// </summary>
    private ExpressionNode ReadLogicalOr()
    {
        var ops = new SerializingList<ExpressionNode>();
        ops.Add(ReadLogicalAnd());

        while (true)
        {
            var savepoint = _filter.Position;
            if (_filter.HasSignificantSubSequence(LogicalOperator.Or.OperatorString))
            {
                ops.Add(ReadLogicalAnd());
            }
            else
            {
                _filter.SetPosition(savepoint);
                break;
            }
        }

        return 1 == ops.Count() ? ops[0] : LogicalExpressionNode.CreateLogicalOr(ops);
    }

    private ExpressionNode ReadLogicalAnd()
    {
        /// @fixme copy-pasted
        var ops = new SerializingList<ExpressionNode>();
        ops.Add(ReadLogicalAndOperand());

        while (true)
        {
            var savepoint = _filter.Position;
            if (_filter.HasSignificantSubSequence(LogicalOperator.And.OperatorString))
            {
                ops.Add(ReadLogicalAndOperand());
            }
            else
            {
                _filter.SetPosition(savepoint);
                break;
            }
        }

        return 1 == ops.Count() ? ops[0] : LogicalExpressionNode.CreateLogicalAnd(ops);
    }

    private ExpressionNode ReadLogicalAndOperand()
    {
        var savepoint = _filter.SkipBlanks().Position;
        if (_filter.SkipBlanks().CurrentCharIs(Not))
        {
            _filter.ReadSignificantChar(Not);
            switch (_filter.SkipBlanks().CurrentChar())
            {
                case DocContext:
                case EvalContext:
                    _filter.SetPosition(savepoint);
                    break;
                default:
                    var op = ReadLogicalAndOperand();
                    return LogicalExpressionNode.CreateLogicalNot(op);
            }
        }

        if (_filter.SkipBlanks().CurrentCharIs(OpenParenthesis))
        {
            _filter.ReadSignificantChar(OpenParenthesis);
            var op = ReadLogicalOr();
            _filter.ReadSignificantChar(CloseParenthesis);
            return op;
        }

        return ReadExpression();
    }

    private RelationalExpressionNode ReadExpression()
    {
        var left = ReadValueNode();
        var savepoint = _filter.Position;
        try
        {
            var operator1 = ReadRelationalOperator();
            var right1 = ReadValueNode();
            return new RelationalExpressionNode(left, operator1, right1);
        }
        catch (InvalidPathException)
        {
            _filter.SetPosition(savepoint);
        }

        var pathNode = left.AsPathNode();
        left = pathNode.AsExistsCheck(pathNode.ShouldExists());
        var @operator = RelationalOperator.Exists;
        ValueNode right = left.AsPathNode().ShouldExists() ? ValueNodeConstants.True : ValueNodeConstants.False;
        return new RelationalExpressionNode(left, @operator, right);
    }

    private LogicalOperator ReadLogicalOperator()
    {
        var begin = _filter.SkipBlanks().Position;
        var end = begin + 1;

        if (!_filter.InBounds(end)) throw new InvalidPathException("Expected bool literal");
        var logicalOperator = _filter.Subsequence(begin, end + 1);
        if (!logicalOperator.Equals("||") && !logicalOperator.Equals("&&"))
            throw new InvalidPathException("Expected logical operator");
        _filter.IncrementPosition(logicalOperator.Length);
        Logger.Trace($"LogicalOperator from {begin} to {end} -> [{logicalOperator}]");

        return LogicalOperator.FromString(logicalOperator);
    }

    private RelationalOperator ReadRelationalOperator()
    {
        var begin = _filter.SkipBlanks().Position;

        if (IsRelationalOperatorChar(_filter.CurrentChar()))
            while (_filter.InBounds() && IsRelationalOperatorChar(_filter.CurrentChar()))
                _filter.IncrementPosition(1);
        else
            while (_filter.InBounds() && _filter.CurrentChar() != Space)
                _filter.IncrementPosition(1);

        var @operator = _filter.Subsequence(begin, _filter.Position);
        Logger.Trace($"Operator from {begin} to {_filter.Position - 1} -> [{@operator}]");
        return RelationalOperator.FromString(@operator);
    }

    private NullNode ReadNullLiteral()
    {
        var begin = _filter.Position;
        if (_filter.CurrentChar() == Null && _filter.InBounds(_filter.Position + 3))
        {
            var nullValue = _filter.Subsequence(_filter.Position, _filter.Position + 4);
            if ("null".Equals(nullValue))
            {
                Logger.Trace($"NullLiteral from {begin} to {_filter.Position + 3} -> [{nullValue}]");
                _filter.IncrementPosition(nullValue.Length);
                return ValueNode.CreateNullNode();
            }
        }

        throw new InvalidPathException("Expected <null> value");
    }

    private JsonNode ReadJsonLiteral()
    {
        var begin = _filter.Position;

        var openChar = _filter.CurrentChar();

        Debug.Assert(openChar == OpenArray || openChar == OpenObject);

        var closeChar = openChar == OpenArray ? CloseArray : CloseObject;

        var closingIndex = _filter.IndexOfMatchingCloseChar(_filter.Position, openChar, closeChar, true, false);
        if (closingIndex == -1)
            throw new InvalidPathException($"string not closed. Expected {SingleQuote}" + $" in {_filter}");
        _filter.SetPosition(closingIndex + 1);
        var json = _filter.Subsequence(begin, _filter.Position);
        Logger.Trace($"JsonLiteral from {begin} to {_filter.Position} -> [{json}]");
        return ValueNode.CreateJsonNode(json);
    }

    private int EndOfFlags(int position)
    {
        var endIndex = position;
        var currentChar = new char[1];
        while (_filter.InBounds(endIndex))
        {
            currentChar[0] = _filter[endIndex];
            if (PatternFlag.ParseFlags(currentChar) > 0)
            {
                endIndex++;
                continue;
            }

            break;
        }

        return endIndex;
    }

    private PatternNode ReadPattern()
    {
        var begin = _filter.Position;
        var closingIndex = _filter.NextIndexOfUnescaped(Pattern);
        if (closingIndex == -1)
            throw new InvalidPathException($"Regex not closed. Expected {Pattern}" + $" in {_filter}");

        if (_filter.InBounds(closingIndex + 1))
        {
            var endFlagsIndex = EndOfFlags(closingIndex + 1);
            if (endFlagsIndex > closingIndex)
            {
                var flags = _filter.Subsequence(closingIndex + 1, endFlagsIndex);
                closingIndex += flags.Length;
            }
        }

        _filter.SetPosition(closingIndex + 1);
        var pattern = _filter.Subsequence(begin, _filter.Position);
        Logger.Trace($"PatternNode from {begin} to {_filter.Position} -> [{pattern}]");
        return ValueNode.CreatePatternNode(pattern);
    }

    private StringNode ReadStringLiteral(char endChar)
    {
        var begin = _filter.Position;

        var closingSingleQuoteIndex = _filter.NextIndexOfUnescaped(endChar);
        if (closingSingleQuoteIndex == -1)
            throw new InvalidPathException($"string literal does not have matching quotes. Expected {endChar}" +
                                           $" in {_filter}");
        _filter.SetPosition(closingSingleQuoteIndex + 1);
        var stringLiteral = _filter.Subsequence(begin, _filter.Position);
        Logger.Trace($"StringLiteral from {begin} to {_filter.Position} -> [{stringLiteral}]");
        return ValueNode.CreateStringNode(stringLiteral, true);
    }

    private NumberNode ReadNumberLiteral()
    {
        var begin = _filter.Position;

        while (_filter.InBounds() && _filter.IsNumberCharacter(_filter.Position)) _filter.IncrementPosition(1);
        var numberLiteral = _filter.Subsequence(begin, _filter.Position);
        Logger.Trace($"NumberLiteral from {begin} to {_filter.Position} -> [{numberLiteral}]");
        return ValueNode.CreateNumberNode(numberLiteral);
    }

    private BooleanNode ReadBooleanLiteral()
    {
        var begin = _filter.Position;
        var end = _filter.CurrentChar() == True ? _filter.Position + 3 : _filter.Position + 4;

        if (!_filter.InBounds(end)) throw new InvalidPathException("Expected bool literal");
        var boolValue = _filter.Subsequence(begin, end + 1);
        if (!boolValue.Equals("true") && !boolValue.Equals("false"))
            throw new InvalidPathException("Expected bool literal");
        _filter.IncrementPosition(boolValue.Length);
        Logger.Trace($"BooleanLiteral from {begin} to {end} -> [{boolValue}]");

        return ValueNode.CreateBooleanNode(boolValue);
    }

    private PathNode ReadPath()
    {
        var previousSignificantChar = _filter.PreviousSignificantChar();
        var begin = _filter.Position;

        _filter.IncrementPosition(1); //skip $ and @
        while (_filter.InBounds())
        {
            if (_filter.CurrentChar() == OpenSquareBracket)
            {
                var closingSquareBracketIndex = _filter.IndexOfMatchingCloseChar(_filter.Position, OpenSquareBracket,
                    CloseSquareBracket, true, false);
                if (closingSquareBracketIndex == -1)
                    throw new InvalidPathException($"Square brackets does not match in filter {_filter}");
                _filter.SetPosition(closingSquareBracketIndex + 1);
            }

            var closingFunctionBracket =
                _filter.CurrentChar() == CloseParenthesis && CurrentCharIsClosingFunctionBracket(begin);
            var closingLogicalBracket = _filter.CurrentChar() == CloseParenthesis && !closingFunctionBracket;

            if (!_filter.InBounds() || IsRelationalOperatorChar(_filter.CurrentChar()) ||
                _filter.CurrentChar() == Space ||
                closingLogicalBracket)
                break;
            _filter.IncrementPosition(1);
        }

        var shouldExists = previousSignificantChar != Not;
        var path = _filter.Subsequence(begin, _filter.Position);
        return ValueNode.CreatePathNode(path, false, shouldExists);
    }

    private bool ExpressionIsTerminated()
    {
        var c = _filter.CurrentChar();
        if (c == CloseParenthesis || IsLogicalOperatorChar(c)) return true;
        c = _filter.NextSignificantChar();
        return c == CloseParenthesis || IsLogicalOperatorChar(c);
    }

    private bool CurrentCharIsClosingFunctionBracket(int lowerBound)
    {
        if (_filter.CurrentChar() != CloseParenthesis) return false;
        var idx = _filter.IndexOfPreviousSignificantChar();
        if (idx == -1 || _filter[idx] != OpenParenthesis) return false;
        idx--;
        while (_filter.InBounds(idx) && idx > lowerBound)
        {
            if (_filter[idx] == Period) return true;
            idx--;
        }

        return false;
    }

    private bool IsLogicalOperatorChar(char c)
    {
        return c == And || c == Or;
    }

    private bool IsRelationalOperatorChar(char c)
    {
        return c == Lt || c == Gt || c == Eq || c == Tilde || c == Not;
    }

    public class CompiledFilter : Filter
    {
        private readonly IPredicate _predicate;

        public CompiledFilter(IPredicate predicate)
        {
            _predicate = predicate;
        }


        public override bool Apply(IPredicateContext ctx)
        {
            return _predicate.Apply(ctx);
        }


        public override string ToString()
        {
            var predicateString = _predicate.ToString();
            if (predicateString.StartsWith("("))
                return $"[?{predicateString}" + "]";
            return $"[?({predicateString}" + ")]";
        }
    }
}