using System;

namespace Core
{
    public static class OptionExtensions
    {
        public static Option<T> ToOption<T>(this T value)
        {
            if (typeof(T).IsValueType == false && ReferenceEquals(value, null))
            {
                return Option<T>.Empty;
            }
            return new Option<T>(value);
        }
    }
}
