using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.Helpers;

namespace XavierJefferson.JsonPathParser.Path;

public class ArrayIndexOperation
{
    private static readonly Regex Comma = new("\\s*,\\s*");

    private readonly ReadOnlyCollection<int> _indexes;

    private ArrayIndexOperation(SerializingList<int> indexes)
    {
        _indexes = new ReadOnlyCollection<int>(indexes);
    }

    public ReadOnlyCollection<int> Indexes()
    {
        return _indexes;
    }

    public bool IsSingleIndexOperation()
    {
        return _indexes.Count() == 1;
    }


    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.Append("[");
        sb.Append(string.Join(",",  _indexes.Select(i => i.ToString())));
        sb.Append("]");

        return sb.ToString();
    }

    public static ArrayIndexOperation Parse(string operation)
    {
        //check valid chars
        for (var i = 0; i < operation.Length; i++)
        {
            var c = operation[i];
            if (!c.IsDigit() && c != ',' && c != ' ' && c != '-')
                throw new InvalidPathException($"Failed to Parse ArrayIndexOperation: {operation}");
        }

        var tokens = Comma.Split(operation);

        var tempIndexes = new SerializingList<int>(tokens.Length);
        foreach (var token in tokens) tempIndexes.Add(ParseInteger(token));

        return new ArrayIndexOperation(tempIndexes);
    }

    private static int ParseInteger(string token)
    {
        try
        {
            return int.Parse(token);
        }
        catch (Exception e)
        {
            throw new InvalidPathException($"Failed to Parse token in ArrayIndexOperation: {token}", e);
        }
    }
}