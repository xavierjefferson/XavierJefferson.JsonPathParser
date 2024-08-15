using XavierJefferson.JsonPathParser.PathRefs;

namespace XavierJefferson.JsonPathParser.Path;

public class RootPathToken : PathToken
{
    private readonly string _rootToken;

    private PathToken _tail;
    private int _tokenCount;


    public RootPathToken(char rootToken)
    {
        _rootToken = rootToken.ToString();
        _tail = this;
        _tokenCount = 1;
    }

    public PathToken GetTail()
    {
        return _tail;
    }


    public override int GetTokenCount()
    {
        return _tokenCount;
    }

    public RootPathToken Append(PathToken next)
    {
        _tail = _tail.AppendTailToken(next);
        _tokenCount++;
        return this;
    }

    public PathTokenAppenderDelegate GetPathTokenAppender()
    {
        return i => Append(i);
    }


    public override void Evaluate(string currentPath, PathRef pathRef, object? model, EvaluationContextImpl context)
    {
        if (IsLeaf())
        {
            var op = context.ForUpdate() ? pathRef : PathRef.NoOp;
            context.AddResult(_rootToken, op, model);
        }
        else
        {
            Next().Evaluate(_rootToken, pathRef, model, context);
        }
    }


    public override string GetPathFragment()
    {
        return _rootToken;
    }


    public override bool IsTokenDefinite()
    {
        return true;
    }

    public bool IsFunctionPath()
    {
        return _tail is FunctionPathToken;
    }

    public void SetTail(PathToken token)
    {
        _tail = token;
    }
}