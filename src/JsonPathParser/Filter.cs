using XavierJefferson.JsonPathParser.Filtering;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser;

/// <summary>
///     */
public abstract class Filter : IPredicate
{
    public abstract bool Apply(IPredicateContext ctx);

    /// <summary>
    ///     Creates a new Filter based on given criteria
    /// </summary>
    /// <param name="predicate">criteria</param>
    /// <returns> a new Filter</returns>
    public static Filter Create(IPredicate predicate)
    {
        return new SingleFilter(predicate);
    }

    /// <summary>
    ///     Create a new Filter based on given list of criteria.
    /// </summary>
    /// <param name="predicates">list of criteria all needs to evaluate to true</param>
    /// <returns> the filter</returns>
    public static Filter Create(ICollection<IPredicate> predicates)
    {
        return new AndFilter(predicates);
    }


    public virtual Filter Or(IPredicate other)
    {
        return new OrFilter(this, other);
    }

    public virtual Filter And(IPredicate other)
    {
        return new AndFilter(this, other);
    }

    /// <summary>
    ///     Parses a filter. The filter must match <code>[?(<filter>)]</code>, white spaces are ignored.
    /// </summary>
    /// <param name="filter">filter string to Parse</param>
    /// <returns> the filter</returns>
    public static Filter Parse(string filter)
    {
        return FilterCompiler.Compile(filter);
    }
}