using System.Reflection;
using XavierJefferson.JsonPathParser.Exceptions;

namespace XavierJefferson.JsonPathParser.Filtering;

public class RelationalOperator
{
    public static readonly RelationalOperator Gte = new(">=");
    public static readonly RelationalOperator Lte = new("<=");
    public static readonly RelationalOperator Eq = new("==");

    /// <summary>
    ///     Type safe equals
    /// </summary>
    public static readonly RelationalOperator Tseq = new("===");

    public static readonly RelationalOperator Ne = new("!=");

    /// <summary>
    ///     Type safe not equals
    /// </summary>
    public static readonly RelationalOperator Tsne = new("!==");

    public static readonly RelationalOperator Lt = new("<");
    public static readonly RelationalOperator Gt = new(">");
    public static readonly RelationalOperator Regex = new("=~");
    public static readonly RelationalOperator Nin = new("NIN");
    public static readonly RelationalOperator In = new("IN");
    public static readonly RelationalOperator Contains = new("CONTAINS");
    public static readonly RelationalOperator All = new("ALL");
    public static readonly RelationalOperator Size = new("SIZE");
    public static readonly RelationalOperator Exists = new("EXISTS");
    public static readonly RelationalOperator Type = new("TYPE");
    public static readonly RelationalOperator Matches = new("MATCHES");
    public static readonly RelationalOperator Empty = new("EMPTY");
    public static readonly RelationalOperator SubsetOf = new("SUBSETOF");
    public static readonly RelationalOperator AnyOf = new("ANYOF");
    public static readonly RelationalOperator NoneOf = new("NONEOF");

    private static readonly List<RelationalOperator> Values = new()
    {
        Gte, Lte, Eq, Tseq, Ne, Tsne, Lt, Gt, Regex, Nin, In, Contains, All, Size, Exists, Type, Matches, Empty,
        SubsetOf, AnyOf, NoneOf
    };

    private static readonly Dictionary<string, RelationalOperator?> OperatorDictionaryByName =
        typeof(RelationalOperator)
            .GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(i => i.FieldType == typeof(RelationalOperator))
            .Select(i => new { i.Name, Value = i.GetValue(null) as RelationalOperator })
            .ToDictionary(i => i.Name, i => i.Value, StringComparer.InvariantCultureIgnoreCase);

    private static readonly Dictionary<string, RelationalOperator> OperatorDictionary = Values.ToDictionary(
        i => i._operatorString, i => i,
        StringComparer.InvariantCultureIgnoreCase);

    private readonly string _operatorString;

    private RelationalOperator(string operatorString)
    {
        _operatorString = operatorString;
    }

    public static RelationalOperator FromName(string name)
    {
        if (!OperatorDictionaryByName.ContainsKey(name))
            throw new InvalidPathException("Filter operator with name " + name + " is not supported!");
        return OperatorDictionaryByName[name];
    }

    public static RelationalOperator FromString(string operatorString)
    {
        if (!OperatorDictionary.ContainsKey(operatorString))
            throw new InvalidPathException("Filter operator with syntax " + operatorString + " is not supported!");
        return OperatorDictionary[operatorString];
    }


    public override string ToString()
    {
        return _operatorString;
    }
}