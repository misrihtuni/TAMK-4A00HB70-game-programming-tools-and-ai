// Inspired by the article at https://visualstudiomagazine.com/articles/2012/11/01/priority-queues-with-c.aspx
using System;
using System.Collections.Generic;

namespace GA.Collections
{
	public class PriorityQueue<T>
		where T : IComparable<T>
	{
		private List<T> _data = new List<T>();

		/// <summary>
		/// The number of items in the queue.
		/// </summary>
		public int Count => _data.Count;

		/// <summary>
		/// Adds an item to the queue.
		/// </summary>
		/// <param name="item">Item to add to the queue.</param>
		public void Enqueue(T item)
		{
			// 1. Add the item to the end of the list
			_data.Add(item);

			// 2. Bubble up the item to its correct position
			// The list is ordered from the end to the beginning
			int childIndex = Count - 1;

			// 3. Ensure data at parent and child indices are ordered correctly.
			// If they are, sorting is done. Otherwise swap them and move on.
			while (childIndex > 0)
			{
				int parentIndex = (childIndex - 1) / 2;
				if (_data[childIndex].CompareTo(_data[parentIndex]) >= 0)
				{
					// Data is ordered correctly. No need to continue.
					break;
				}

				// Swap the parent and the child
				_data.Swap(childIndex, parentIndex);

				// Move up the tree
				childIndex = parentIndex;
			}
		}

		/// <summary>
		/// Removes an item from the top of the queue and returns it.
		/// </summary>
		/// <returns>The item at the top of the queue.</returns>
		/// <exception cref="InvalidOperationException">Thrown when the queue is empty.</exception>
		public T Dequeue()
		{
			if (Count == 0)
			{
				throw new InvalidOperationException("The queue is empty.");
			}

			// 1. Save the first item
			T result = _data[0];

			// 2. Move the last item to the first position
			int lastIndex = Count - 1;
			_data[0] = _data[lastIndex];
			_data.RemoveAt(lastIndex);
			lastIndex--;

			// 3. Bubble down the first item to its correct position
			int parentIndex = 0;

			// Notice the infinite while loop. Be careful with these!
			while (true)
			{
				// The child index for the left child.
				// Initially, childIndex refers to the left child and it will be updated
				// later (if needed).
				int childIndex = parentIndex * 2 + 1;
				if (childIndex > lastIndex)
				{
					// No children. We are done.
					break;
				}

				// The child index for the right child
				int rightChildIndex = childIndex + 1;
				if (rightChildIndex <= lastIndex &&
					_data[rightChildIndex].CompareTo(_data[childIndex]) < 0)
				{
					// The right child has higher priority.
					childIndex = rightChildIndex;
				}

				if (_data[parentIndex].CompareTo(_data[childIndex]) <= 0)
				{
					// The parent has higher priority than its children. We are done.
					break;
				}

				_data.Swap(parentIndex, childIndex);

				parentIndex = childIndex;
			}

			return result;
		}

		/// <summary>
		/// Returns the item at the top of the queue without removing it.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">Thrown when the queue is empty.</exception>
		public T Peek()
		{
			if (Count == 0)
			{
				throw new System.InvalidOperationException("The queue is empty.");
			}

			return _data[0];
		}

		/// <summary>
		/// Removes all items from the queue.
		/// </summary>
		public void Clear()
		{
			_data.Clear();
		}

		/// <summary>
		/// Checks if the queue contains <paramref name="item"/>.
		/// </summary>
		///
		/// <param name="item">The item to look for.</param>
		///
		/// <returns>
		/// <c>true</c> if the <paramref name="item"/> is in the queue,
		/// <c>false</c> otherwise.
		/// <returns>
		public bool Contains(T item)
		{
			return _data.Contains(item);
		}

		/// <summary>
		/// Checks if the order of the items in the queue is consistent.
		/// Used for testing and debug purposes.
		/// </summary>
		public bool IsConsistent()
		{
			if (Count == 0)
			{
				return true;
			}

			int lastIndex = Count - 1;

			for (int parentIndex = 0; parentIndex < Count; parentIndex++)
			{
				int leftChild = 2 * parentIndex + 1;
				int rightChild = 2 * parentIndex + 2;

				if (leftChild <= lastIndex && _data[parentIndex].CompareTo(_data[leftChild]) > 0)
				{
					// If left exists and it's greater than the parent, the queue is ordered
					// incorrectly!
					return false;
				}

				if (rightChild <= lastIndex && _data[parentIndex].CompareTo(_data[rightChild]) > 0)
				{
					// If right exists and it's greater than the parent, the queue is ordered
					// incorrectly!
					return false;
				}
			}

			return true;
		}
	}
}