namespace FeWoLearning.Exercises.Beginner;

// Exercise 032 — Generic Stack (beginner).
// Goal:   Implement a generic LIFO stack wrapper around List<T> with Push,
//         Pop, Peek and a Count property.
// Drills: generic classes, generic type parameters, basic collection wrapping.
public class GenericStack<T>
{
    private readonly List<T> _items = new();

    public int Count => throw new NotImplementedException();

    public void Push(T item) => throw new NotImplementedException();

    public T Pop() => throw new NotImplementedException();

    public T Peek() => throw new NotImplementedException();
}
