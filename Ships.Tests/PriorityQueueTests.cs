using GA.Collections;
using Xunit;

public class PriorityQueueTests
{
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
