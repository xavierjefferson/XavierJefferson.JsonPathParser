using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Path;

public class FoundResultImpl : IFoundResult
{
    public FoundResultImpl(int index, string path, object? result)
    {
        Index = index;
        Path = path;
        Result = result;
    }


    public int Index { get; }


    public string Path { get; }


    public object? Result { get; }
}