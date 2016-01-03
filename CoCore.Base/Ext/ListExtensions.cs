using System;
using System.Collections.Generic;
using System.Linq;

namespace CoCore.Base
{
    internal static class ListExtensions
    {

        // Convenience method on IEnumerable<T> to allow passing of a
        // Comparison<T> delegate to the OrderBy method.
        internal static IEnumerable<T> OrderBy<T>(this IEnumerable<T> list, Comparison<T> comparison)
        {
            return list.OrderBy(t => t, new ComparisonComparer<T>(comparison));
        }

        // Convenience method on IEnumerable<T> to allow passing of a
        // Comparison<T> delegate to the OrderBy method.
        internal static IEnumerable<T> OrderBy<T>(this IEnumerable<T> list, Comparer<T> comparer)
        {
            return list.OrderBy(t => t, comparer);
        }

    }
}
