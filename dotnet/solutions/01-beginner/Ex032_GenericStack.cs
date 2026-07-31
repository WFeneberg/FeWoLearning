namespace FeWoLearning.Exercises.Beginner;

// Exercise 032 — Generic Stack (reference solution).
public class GenericStack<T>
{
    private readonly List<T> _items = new();

    public int Count => _items.Count;

    public void Push(T item) => _items.Add(item);

    public T Pop()
    {
        if (_items.Count == 0)
        {
            throw new InvalidOperationException("Stack is empty.");
        }

        var index = _items.Count - 1;
        var item = _items[index];
        _items.RemoveAt(index);
        return item;
    }

    public T Peek()
    {
        if (_items.Count == 0)
        {
            throw new InvalidOperationException("Stack is empty.");
        }

        return _items[^1];
    }
}
