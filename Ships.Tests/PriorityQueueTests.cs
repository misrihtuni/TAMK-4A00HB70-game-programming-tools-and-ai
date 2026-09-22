using GA.Collections;
using Xunit;

public class PriorityQueueTests
{
	#region Enqueue

	[Fact]
	public void Enqueue_WhenNotAddingDuplicates_IncreasesCount()
	{
		PriorityQueue<int> queue = new PriorityQueue<int>();

		queue.Enqueue(1);
		int count1 = queue.Count;

		queue.Enqueue(0);
		int count2 = queue.Count;

		queue.Enqueue(-1);
		int count3 = queue.Count;

		Assert.Equal(1, count1);
		Assert.Equal(2, count2);
		Assert.Equal(3, count3);
	}

	[Fact]
	public void Enqueue_WhenAddingDuplicates_IncreasesCount()
	{
		PriorityQueue<int> queue = new PriorityQueue<int>();

		queue.Enqueue(1);
		int count1 = queue.Count;

		queue.Enqueue(1);
		int count2 = queue.Count;

		queue.Enqueue(1);
		int count3 = queue.Count;

		Assert.Equal(1, count1);
		Assert.Equal(2, count2);
		Assert.Equal(3, count3);
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

	#endregion Peek


	#region Dequeue

	[Fact]
	public void Dequeue_WhenEmpty_ThrowsInvalidOperationException()
	{
		PriorityQueue<int> queue = new PriorityQueue<int>();

		Assert.Throws<InvalidOperationException>(() => queue.Dequeue());
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

	#endregion Dequeue


	#region Clear

	[Fact]
	public void Clear_WhenEmpty_ThrowsNoExceptions()
	{
		PriorityQueue<int> queue = new PriorityQueue<int>();

		try
		{
			queue.Clear();
		}
		catch
		{
			Assert.Fail();
		}
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
