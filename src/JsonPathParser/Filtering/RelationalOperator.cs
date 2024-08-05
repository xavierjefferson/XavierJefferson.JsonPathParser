using System.Reflection;
using XavierJefferson.JsonPathParser.Exceptions;

namespace XavierJefferson.JsonPathParser.Filtering;

public class RelationalOperator
{
    public static RelationalOperator Gte = new(">=");
    public static RelationalOperator Lte = new("<=");
    public static RelationalOperator Eq = new("==");

    /// <summary>
    ///     Type safe equals
    /// </summary>
    public static RelationalOperator Tseq = new("===");

    public static RelationalOperator Ne = new("!=");

    /// <summary>
    ///     Type safe not equals
    /// </summary>
    public static RelationalOperator Tsne = new("!==");

    public static RelationalOperator Lt = new("<");
    public static RelationalOperator Gt = new(">");
    public static RelationalOperator Regex = new("=~");
    public static RelationalOperator Nin = new("NIN");
    public static RelationalOperator In = new("IN");
    public static RelationalOperator Contains = new("CONTAINS");
    public static RelationalOperator All = new("ALL");
    public static RelationalOperator Size = new("SIZE");
    public static RelationalOperator Exists = new("EXISTS");
    public static RelationalOperator Type = new("TYPE");
    public static RelationalOperator Matches = new("MATCHES");
    public static RelationalOperator Empty = new("EMPTY");
    public static RelationalOperator SubsetOf = new("SUBSETOF");
    public static RelationalOperator AnyOf = new("ANYOF");
    public static RelationalOperator NoneOf = new("NONEOF");

    private static readonly SerializingList<RelationalOperator> Values = new()
    {
        Gte, Lte, Eq, Tseq, Ne, Tsne, Lt, Gt, Regex, Nin, In, Contains, All, Size, Exists, Type, Matches, Empty,
        SubsetOf, AnyOf, NoneOf
    };

    private static readonly Dictionary<string, RelationalOperator?> OperatorDictionaryByName = typeof(RelationalOperator)
        .GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        .Where(i => i.FieldType == typeof(RelationalOperator))
        .Select(i => new { i.Name, Value = i.GetValue(null) as RelationalOperator })
        .ToDictionary(i => i.Name, i => i.Value, StringComparer.InvariantCultureIgnoreCase);

    private static readonly Dictionary<string, RelationalOperator> OperatorDictionary = Values.ToDictionary(
        i => i._operatorString, i => i,
        StringComparer.InvariantCultureIgnoreCase);

    private readonly string _operatorString;

    public static RelationalOperator FromName(string name)
    {
        if (!OperatorDictionaryByName.ContainsKey(name))
            throw new InvalidPathException("Filter operator with name " + name + " is not supported!");
        return OperatorDictionaryByName[name];
    }
    private RelationalOperator(string operatorString)
    {
        _operatorString = operatorString;
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