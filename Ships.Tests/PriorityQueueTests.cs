using GA.Collections;
using Xunit;

public class PriorityQueueTests
{
	#region Enqueue

	[Fact]
	public void Enqueue_IncreasesCount()
	{
		// Rename suggested by Copilot.
		PriorityQueue<int> queue = new PriorityQueue<int>();

		queue.Enqueue(1);
		int count1 = queue.Count;

		queue.Enqueue(0);
		int count2 = queue.Count;

		queue.Enqueue(-1);
		int count3 = queue.Count;

		queue.Enqueue(1);
		int count4 = queue.Count;

		Assert.Equal(1, count1);
		Assert.Equal(2, count2);
		Assert.Equal(3, count3);
		Assert.Equal(4, count4);
	}

	[Fact]
	public void Enqueue_KeepsHighesPriorityAtTop()
	{
		PriorityQueue<int> queue = new PriorityQueue<int>();

		queue.Enqueue(5);
		int peek1 = queue.Peek(); // 5

		queue.Enqueue(3);
		int peek2 = queue.Peek(); // 3

		queue.Enqueue(6);
		int peek3 = queue.Peek(); // 3

		queue.Enqueue(4);
		int peek4 = queue.Peek(); // 3

		Assert.Equal(5, peek1);
		Assert.Equal(3, peek2);
		Assert.Equal(3, peek3);
		Assert.Equal(3, peek4);
	}

	[Fact]
	public void Enqueue_MaintainsConsistency()
	{
		// Suggested by Copilot.
		PriorityQueue<int> queue = new PriorityQueue<int>();

		queue.Enqueue(5);
		bool isConsistent = queue.IsConsistent();

		queue.Enqueue(3);
		isConsistent = isConsistent && queue.IsConsistent();

		queue.Enqueue(6);
		isConsistent = isConsistent && queue.IsConsistent();

		queue.Enqueue(-1);
		isConsistent = isConsistent && queue.IsConsistent();

		Assert.True(isConsistent);
	}

	#endregion Enqueue


	#region Peek

	[Fact]
	public void Peek_WhenEmpty_ThrowsInvalidOperationException()
	{
		PriorityQueue<int> queue = new PriorityQueue<int>();

		Assert.Throws<InvalidOperationException>(() => queue.Peek());
	}

	[Fact]
	public void Peek_WhenNotEmpty_ReturnsFirstItem()
	{
		PriorityQueue<int> queue = new PriorityQueue<int>();
		queue.Enqueue(1);

		int result = queue.Peek();

		Assert.Equal(1, result);

		// Since the Peek method is not responsible for the actual order of the
		// items and it only returns the first item on the internal list, we can
		// assume that if it returns the only item in the list, it will always
		// return an item at index 0.
	}

	[Fact]
	public void Peek_DoesNotRemoveItem()
	{
		// Suggested by Copilot.
		PriorityQueue<int> queue = new PriorityQueue<int>();
		queue.Enqueue(1);

		Assert.Equal(1, queue.Peek());
		Assert.Equal(1, queue.Count);
	}

	#endregion Peek


	#region Dequeue

	[Fact]
	public void Dequeue_WhenEmpty_ThrowsInvalidOperationException()
	{
		PriorityQueue<int> queue = new PriorityQueue<int>();

		Assert.Throws<InvalidOperationException>(() => queue.Dequeue());
	}

	[Fact]
	public void Dequeue_WhenSingleItem_ReturnsItemAndClearsQueue()
	{
		// Suggested by Copilot.
		PriorityQueue<int> queue = new PriorityQueue<int>();
		queue.Enqueue(1);

		int result = queue.Dequeue();

		Assert.Equal(1, result);
		Assert.Equal(0, queue.Count);
	}

	[Fact]
	public void Dequeue_WhenNoDuplicates_RemovesLowestPriorityAndReducesCount()
	{
		PriorityQueue<int> queue = new PriorityQueue<int>();
		queue.Enqueue(5);
		queue.Enqueue(3);
		queue.Enqueue(6);
		queue.Enqueue(4);

		Assert.Equal(3, queue.Dequeue());
		Assert.Equal(3, queue.Count);

		Assert.Equal(4, queue.Dequeue());
		Assert.Equal(2, queue.Count);

		Assert.Equal(5, queue.Dequeue());
		Assert.Equal(1, queue.Count);

		Assert.Equal(6, queue.Dequeue());
		Assert.Equal(0, queue.Count);
	}

	[Fact]
	public void Dequeue_WhenHasDuplicates_RemovesOnlyOneOccurenceAndReducesCount()
	{
		PriorityQueue<int> queue = new PriorityQueue<int>();
		queue.Enqueue(5);
		queue.Enqueue(3);
		queue.Enqueue(5);
		queue.Enqueue(3);

		Assert.Equal(3, queue.Dequeue());
		Assert.Equal(3, queue.Count);

		Assert.Equal(3, queue.Dequeue());
		Assert.Equal(2, queue.Count);

		Assert.Equal(5, queue.Dequeue());
		Assert.Equal(1, queue.Count);

		Assert.Equal(5, queue.Dequeue());
		Assert.Equal(0, queue.Count);
	}

	[Fact]
	public void Dequeue_MaintainsConsistency()
	{
		// Suggested by Copilot.
		PriorityQueue<int> queue = new PriorityQueue<int>();
		queue.Enqueue(5);
		queue.Enqueue(3);
		queue.Enqueue(6);
		queue.Enqueue(-1);

		queue.Dequeue();
		bool isConsistent = queue.IsConsistent();

		queue.Dequeue();
		isConsistent = isConsistent && queue.IsConsistent();

		queue.Dequeue();
		isConsistent = isConsistent && queue.IsConsistent();

		queue.Dequeue();
		isConsistent = isConsistent && queue.IsConsistent();

		Assert.True(isConsistent);
	}

	#endregion Dequeue


	#region Clear

	[Fact]
	public void Clear_WhenEmpty_ThrowsNoExceptions()
	{
		PriorityQueue<int> queue = new PriorityQueue<int>();

		queue.Clear();

		Assert.Equal(0, queue.Count);

		// Suggested by Copilot.
		Assert.Throws<InvalidOperationException>(() => queue.Peek());
		Assert.Throws<InvalidOperationException>(() => queue.Dequeue());
	}

	[Fact]
	public void Clear_WhenNotEmpty_RemovesAllItems()
	{
		// Suggested by Copilot.
		PriorityQueue<int> queue = new PriorityQueue<int>();
		queue.Enqueue(5);
		queue.Enqueue(3);
		queue.Enqueue(6);
		queue.Enqueue(4);

		queue.Clear();

		Assert.False(queue.Contains(5));
		Assert.False(queue.Contains(3));
		Assert.False(queue.Contains(6));
		Assert.False(queue.Contains(4));
		Assert.Equal(0, queue.Count);

		// Suggested by Copilot.
		Assert.Throws<InvalidOperationException>(() => queue.Peek());
		Assert.Throws<InvalidOperationException>(() => queue.Dequeue());
	}

	#endregion Clear


	#region Contains

	[Fact]
	public void Contains_WhenEmpty_ReturnsFalse()
	{
		PriorityQueue<int> queue = new PriorityQueue<int>();

		Assert.False(queue.Contains(1));
	}

	[Fact]
	public void Contains_WhenExists_ReturnsTrue()
	{
		PriorityQueue<int> queue = new PriorityQueue<int>();
		queue.Enqueue(1);

		Assert.True(queue.Contains(1));
	}

	[Fact]
	public void Contains_WhenNotExists_ReturnsFalse()
	{
		PriorityQueue<int> queue = new PriorityQueue<int>();
		queue.Enqueue(1);

		Assert.False(queue.Contains(99));
	}

	#endregion Contains
}
