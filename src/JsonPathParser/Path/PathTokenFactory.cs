using XavierJefferson.JsonPathParser.Function;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Path;

public class PathTokenFactory
{
    public static RootPathToken CreateRootPathToken(char token)
    {
        return new RootPathToken(token);
    }

    public static PathToken CreateSinglePropertyPathToken(string property, char stringDelimiter)
    {
        return new PropertyPathToken(new List<string?> { property }, stringDelimiter);
    }

    public static PathToken CreatePropertyPathToken(IList<string?> properties, char stringDelimiter)
    {
        return new PropertyPathToken(properties, stringDelimiter);
    }

    public static PathToken CreateSliceArrayPathToken(ArraySliceOperation arraySliceOperation)
    {
        return new ArraySliceToken(arraySliceOperation);
    }

    public static PathToken CreateIndexArrayPathToken(ArrayIndexOperation arrayIndexOperation)
    {
        return new ArrayIndexToken(arrayIndexOperation);
    }

    public static PathToken CreateWildCardPathToken()
    {
        return new WildcardPathToken();
    }

    public static PathToken CrateScanToken()
    {
        return new ScanPathToken();
    }

    public static PathToken CreatePredicatePathToken(ICollection<IPredicate> predicates)
    {
        return new PredicatePathToken(predicates);
    }

    public static PathToken CreatePredicatePathToken(IPredicate predicate)
    {
        return new PredicatePathToken(predicate);
    }

    public static PathToken CreateFunctionPathToken(string function, IList<Parameter>? parameters)
    {
        return new FunctionPathToken(function, parameters);
    }
}