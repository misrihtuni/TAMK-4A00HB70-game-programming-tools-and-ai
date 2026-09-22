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

	#endregion Enqueue


	#region Peek

	[Fact]
	public void Peek_WhenEmpty_ThrowsInvalidOperationException()
	{
		PriorityQueue<int> queue = new PriorityQueue<int>();

		Assert.Throws<InvalidOperationException>(() => queue.Peek());
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
