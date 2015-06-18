using System;
using System.Collections.Generic;

namespace Core
{
    public static class EnumerableExtensions
    {
        public static void Iter<T>(this IEnumerable<T> value, Action<T> action)
        {
            foreach (T item in value)
            {
                action(item);
            }
        }

        public static void IterI<T>(this IEnumerable<T> value, Action<int, T> action)
        {
            int i = 0;
            foreach (T item in value)
            {
                action(i++, item);
            }
        }
    }
}
