namespace XavierJefferson.JsonPathParser.Path;

public interface IPathTokenAppender
{
    IPathTokenAppender AppendPathToken(PathToken next);
}