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

	#endregion Dequeue
}
