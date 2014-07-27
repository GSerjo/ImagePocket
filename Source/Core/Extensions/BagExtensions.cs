using System;

namespace Core
{
	public static class BagExtensions
	{
		public static Bag<T> ToBag<T>(this T value)
		{
			if (typeof(T).IsValueType == false && ReferenceEquals(value, null))
			{
				return Bag<T>.Empty;
			}
			return new Bag<T>(value);
		}
	}
}

