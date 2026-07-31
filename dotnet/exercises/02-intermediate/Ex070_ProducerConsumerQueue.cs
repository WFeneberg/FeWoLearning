namespace FeWoLearning.Exercises.Intermediate;

// Exercise 070 — Producer/Consumer Queue (intermediate).
// Goal:   Implement a thread-safe FIFO queue using `lock` statements around
//         Enqueue/Dequeue so that many producer threads/tasks can push items
//         concurrently and every enqueued item is dequeued exactly once, with
//         no items lost or duplicated.
// Drills: lock statement, shared mutable state, basic synchronization,
//         Queue<T> as the backing store, TryDequeue-style non-blocking pop.
public class ProducerConsumerQueue<T>
{
    private readonly Queue<T> _items = new();
    private readonly object _gate = new();

    // Adds an item to the back of the queue. Must be safe to call
    // concurrently from multiple threads/tasks.
    public void Enqueue(T item) => throw new NotImplementedException();

    // Attempts to remove and return the item at the front of the queue.
    // Returns false (and default(T) via out param) if the queue is empty.
    // Must be safe to call concurrently from multiple threads/tasks.
    public bool TryDequeue(out T? item) => throw new NotImplementedException();

    // Current number of items in the queue. Safe to call concurrently.
    public int Count => throw new NotImplementedException();
}
