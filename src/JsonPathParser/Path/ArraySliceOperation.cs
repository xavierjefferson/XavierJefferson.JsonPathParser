using System.Text;
using XavierJefferson.JsonPathParser.Enums;
using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.Helpers;

namespace XavierJefferson.JsonPathParser.Path;

public class ArraySliceOperation
{
    private readonly int? _from;
    private readonly ArraySliceOperationEnum _operation;
    private readonly int? _to;

    private ArraySliceOperation(int? from, int? to, ArraySliceOperationEnum operation)
    {
        _from = from;
        _to = to;
        _operation = operation;
    }

    public int? From()
    {
        return _from;
    }

    public int? To()
    {
        return _to;
    }

    public ArraySliceOperationEnum Operation()
    {
        return _operation;
    }


    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("[");
        sb.Append(From == null ? "" : _from.ToString());
        sb.Append(":");
        sb.Append(To == null ? "" : _to.ToString());
        sb.Append("]");

        return sb.ToString();
    }

    public static ArraySliceOperation Parse(string operation)
    {
        //check valid chars
        for (var i = 0; i < operation.Length; i++)
        {
            var c = operation[i];
            if (!c.IsDigit() && c != '-' && c != ':')
                throw new InvalidPathException($"Failed to Parse SliceOperation: {operation}");
        }

        var tokens = operation.Split(":");

        var tempFrom = TryRead(tokens, 0);
        var tempTo = TryRead(tokens, 1);
        ArraySliceOperationEnum tempOperation;

        if (tempFrom != null && tempTo == null)
            tempOperation = ArraySliceOperationEnum.SliceFrom;
        else if (tempFrom != null)
            tempOperation = ArraySliceOperationEnum.SliceBetween;
        else if (tempTo != null)
            tempOperation = ArraySliceOperationEnum.SliceTo;
        else
            throw new InvalidPathException($"Failed to Parse SliceOperation: {operation}");

        return new ArraySliceOperation(tempFrom, tempTo, tempOperation);
    }

    private static int? TryRead(string[] tokens, int idx)
    {
        if (tokens.Length > idx)
        {
            if (tokens[idx].Equals("")) return null;
            return int.Parse(tokens[idx]);
        }

        return null;
    }
}