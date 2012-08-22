using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StereoPlanner
{
    static class EnumerableExtensions
    {
        public static T ArgMin<T>(this IEnumerable<T> source, Func<T, double> selector)
        {
            double? min = null;
            T result = default(T);
            foreach (var xs in source)
            {
                var value = selector(xs);
                if (!min.HasValue || value < min.Value)
                {
                    min = value;
                    result = xs;
                }
            }

            return result;
        }

        public static IEnumerable<T> Do<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var xs in source)
            {
                action(xs);
                yield return xs;
            }
        }
    }
}
