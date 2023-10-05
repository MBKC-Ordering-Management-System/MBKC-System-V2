using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Utils
{
    public static class QueryableUtil
    {
        public static IEnumerable<TSource> If<TSource>(this IEnumerable<TSource> source,
            Func<IEnumerable<TSource>, bool> predicate,
            Func<IEnumerable<TSource>, IEnumerable<TSource>> thenPredicate,
            Func<IEnumerable<TSource>, IEnumerable<TSource>>? elsePredicate = null)
        {
            return IfInner(source, predicate(source), thenPredicate, elsePredicate);
        }
        public static IEnumerable<TSource> If<TSource>(this IEnumerable<TSource> source,
            bool predicate,
            Func<IEnumerable<TSource>, IEnumerable<TSource>> thenPredicate,
            Func<IEnumerable<TSource>, IEnumerable<TSource>>? elsePredicate = null)
        {
            return IfInner(source, predicate, thenPredicate, elsePredicate);
        }
        private static IEnumerable<TSource> IfInner<TSource>(IEnumerable<TSource> source,
            bool predicate,
            Func<IEnumerable<TSource>, IEnumerable<TSource>> thenPredicate,
            Func<IEnumerable<TSource>, IEnumerable<TSource>>? elsePredicate = null)
        {
            if (predicate)
                return thenPredicate(source);
            else if (elsePredicate == null)
                return source;
            else
                return elsePredicate(source);
        }
    }
}
