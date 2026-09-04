// Exercise 076 - Bounded Element Pool (advanced).
// Goal:   Reuse elements without letting the pool itself become the leak.
// Drills: a capacity the pool actually enforces, resetting an element on return rather
//         than on hand-out, and counting hits and misses so the pool can be judged.
// Passes: dotnet test --filter FullyQualifiedName~Ex076_
//
// ex046 pooled with an unbounded queue, which is fine for a list with a viewport and wrong
// for anything that returns in bursts: the pool grows to the high-water mark and keeps
// every element for the life of the app. A cap turns that into a decision.
//
// Reset on return, not on hand-out: an element sitting in the pool must not hold a
// reference to the data it last showed, or the pool retains that data too.

using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Exercises.Advanced;

/// <summary>
/// Hands out <see cref="TextBlock"/>s from a pool of at most <see cref="Capacity"/>.
/// </summary>
public sealed class Ex076_BoundedElementPool
{
    private readonly Stack<TextBlock> _pool = new();

    public Ex076_BoundedElementPool(int capacity) => Capacity = capacity;

    /// <summary>The most elements this pool will ever hold.</summary>
    public int Capacity { get; }

    /// <summary>How many elements are pooled right now.</summary>
    public int Pooled => _pool.Count;

    /// <summary>How many hand-outs were served from the pool.</summary>
    public int Hits { get; private set; }

    /// <summary>How many hand-outs had to construct an element.</summary>
    public int Misses { get; private set; }

    /// <summary>How many returned elements were dropped because the pool was full.</summary>
    public int Evictions { get; private set; }

    /// <summary>
    /// An element showing <paramref name="text"/>, from the pool when possible.
    /// </summary>
    public TextBlock Rent(string text)
    {
        TextBlock element;

        if (_pool.Count > 0)
        {
            element = _pool.Pop();
            Hits++;
        }
        else
        {
            element = new TextBlock();
            Misses++;
        }

        element.Text = text;
        return element;
    }

    /// <summary>
    /// Takes <paramref name="element"/> back: cleared and pooled while there is room,
    /// cleared and dropped once the pool is full.
    /// </summary>
    public void Return(TextBlock element)
    {
        // Cleared whatever happens next: a dropped element may still be held by whoever
        // handed it back, and it must not be the thing keeping the old item alive.
        element.Text = "";

        if (_pool.Count >= Capacity)
        {
            // The cap is the decision an unbounded pool never makes: it simply keeps the
            // high-water mark for the life of the app.
            Evictions++;
            return;
        }

        _pool.Push(element);
    }
}
