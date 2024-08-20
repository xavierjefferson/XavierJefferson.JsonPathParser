using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Filtering
{
    public class SimplePredicate : IPredicate
    {
        private Func<IPredicateContext, bool> _func;

        private SimplePredicate()
        {

        }
        public static SimplePredicate Create(Func<IPredicateContext, Boolean> func)
        {
            return new SimplePredicate() { _func = func };
        }
        public bool Apply(IPredicateContext context)
        {
            return _func(context);
        }

        public string ToUnenclosedString()
        {
            return _func.ToString();
        }
    }
}
