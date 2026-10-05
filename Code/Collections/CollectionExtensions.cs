using System.Collections.Generic;

namespace GA.Collections
{
	public static class CollectionExtensions
	{
		/// <summary>
		/// Swaps the data at indices indexA and indexB.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="indexA"></param>
		/// <param name="indexB"></param>
		public static void Swap<T>(this IList<T> list, int indexA, int indexB)
		{
			if (list == null)
			{
				throw new System.ArgumentNullException(nameof(list));
			}

			T temp = list[indexA];
			list[indexA] = list[indexB];
			list[indexB] = temp;
		}

		/// <summary>
		/// Reverses the order of items in the <paramref name="list"/>.
		/// </summary>
		/// <typeparam name="T">Type of the item stored in the list.</typeparam>
		/// <param name="list">The list to reverse.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if the given list is null.</exception>
		public static void Reverse<T>(this IList<T> list)
		{
			if (list == null)
			{
				throw new System.ArgumentNullException(nameof(list));
			}

			for (int i = 0; i < list.Count / 2; ++i)
			{
				int startIndex = i;
				int endIndex = list.Count - 1 - i;
				list.Swap(startIndex, endIndex);
			}
		}
	}
}