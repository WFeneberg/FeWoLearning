namespace FeWoLearning.Exercises.Intermediate;

// Exercise 070 — Producer/Consumer Queue (reference solution).
public class ProducerConsumerQueue<T>
{
    private readonly Queue<T> _items = new();
    private readonly object _gate = new();

    public void Enqueue(T item)
    {
        lock (_gate)
        {
            _items.Enqueue(item);
        }
    }

    public bool TryDequeue(out T? item)
    {
        lock (_gate)
        {
            if (_items.Count == 0)
            {
                item = default;
                return false;
            }

            item = _items.Dequeue();
            return true;
        }
    }

    public int Count
    {
        get
        {
            lock (_gate)
            {
                return _items.Count;
            }
        }
    }
}
