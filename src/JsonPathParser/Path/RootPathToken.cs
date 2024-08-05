using XavierJefferson.JsonPathParser.PathRefs;

namespace XavierJefferson.JsonPathParser.Path;

/// <summary>
///     */
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

    public IPathTokenAppender GetPathTokenAppender()
    {
        return new L1(this);
    }


    public override void Evaluate(string currentPath, PathRef pathRef, object? model, EvaluationContextImpl ctx)
    {
        if (IsLeaf())
        {
            var op = ctx.ForUpdate() ? pathRef : PathRef.NoOp;
            ctx.AddResult(_rootToken, op, model);
        }
        else
        {
            Next().Evaluate(_rootToken, pathRef, model, ctx);
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

    private class L1 : IPathTokenAppender
    {
        private readonly RootPathToken _rpt;

        public L1(RootPathToken rpt)
        {
            this._rpt = rpt;
        }

        public IPathTokenAppender AppendPathToken(PathToken next)
        {
            _rpt.Append(next);
            return this;
        }
    }
}