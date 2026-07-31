using FeWoLearning.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Intermediate;

public class Ex070_ProducerConsumerQueueTests
{
    [Fact]
    public void Enqueue_SingleThread_TryDequeue_ReturnsItemsInFifoOrder()
    {
        var queue = new ProducerConsumerQueue<int>();
        queue.Enqueue(1);
        queue.Enqueue(2);
        queue.Enqueue(3);

        Assert.Equal(3, queue.Count);

        Assert.True(queue.TryDequeue(out var first));
        Assert.Equal(1, first);
        Assert.True(queue.TryDequeue(out var second));
        Assert.Equal(2, second);
        Assert.True(queue.TryDequeue(out var third));
        Assert.Equal(3, third);

        Assert.False(queue.TryDequeue(out var none));
        Assert.Equal(0, none);
        Assert.Equal(0, queue.Count);
    }

    [Fact]
    public void Enqueue_FromMultipleTasks_AllItemsDequeuedExactlyOnceWithNoLoss()
    {
        const int producers = 8;
        const int itemsPerProducer = 500;
        var queue = new ProducerConsumerQueue<int>();

        var tasks = new Task[producers];
        for (int p = 0; p < producers; p++)
        {
            int producerId = p;
            tasks[producerId] = Task.Run(() =>
            {
                int start = producerId * itemsPerProducer;
                for (int i = 0; i < itemsPerProducer; i++)
                {
                    queue.Enqueue(start + i);
                }
            });
        }

        Task.WaitAll(tasks);

        var expectedTotal = producers * itemsPerProducer;
        Assert.Equal(expectedTotal, queue.Count);

        var dequeued = new List<int>(expectedTotal);
        while (queue.TryDequeue(out var item))
        {
            dequeued.Add(item);
        }

        Assert.Equal(0, queue.Count);
        Assert.Equal(expectedTotal, dequeued.Count);

        // Every produced value 0..expectedTotal-1 must appear exactly once:
        // no lost updates and no duplicates from the shared queue.
        var seenCounts = new int[expectedTotal];
        foreach (var value in dequeued)
        {
            seenCounts[value]++;
        }

        Assert.All(seenCounts, count => Assert.Equal(1, count));
    }
}
