using System;
using System.Collections;
using System.Collections.Generic;

namespace CJG.Core.Interfaces
{
	/// <summary>
	/// <typeparamref name="EnumerableExtensions"/> static class, provides extension methods for <typeparamref name="Enumerable"/> types.
	/// </summary>
	public static class EnumerableExtensions
	{
		public static void ForEach(this IEnumerable items, Action<object> action)
		{
			foreach (var item in items)
			{
				action(item);
			}
		}

		public static void ForEach<TItem>(this IEnumerable<TItem> items, Action<TItem> action)
		{
			foreach (var item in items)
			{
				action(item);
			}
		}

		/// <summary>
		/// Get the generic item type of the enumerable.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="items"></param>
		public static Type GetItemType<T>(this IEnumerable<T> items)
		{
			return typeof(T);
		}
	}
}
