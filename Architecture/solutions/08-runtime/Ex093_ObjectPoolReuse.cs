namespace FeWoLearning.Architecture.Exercises.Runtime.Ex093;

/// <summary>
/// A buffer worth reusing. It carries state from whoever used it last, which is the whole
/// hazard.
/// </summary>
public sealed class RenderBuffer
{
    public List<string> Lines { get; } = [];

    public string? Title { get; set; }

    public int TimesRented { get; internal set; }
}

// Exercise 093 — ObjectPoolReuse (reference solution).
public sealed class BufferPool(int capacity)
{
    private readonly Stack<RenderBuffer> _idle = new();

    public int Idle => _idle.Count;

    public int Created { get; private set; }

    public RenderBuffer Rent()
    {
        RenderBuffer buffer;

        if (_idle.Count > 0)
        {
            buffer = _idle.Pop();
        }
        else
        {
            // Grows rather than failing. An object pool bounds nothing - the runtime will
            // happily allocate another buffer - so "exhausted" just means "allocate one".
            // That is exactly where it differs from the connection pool in exercise 092,
            // and getting the two backwards gives you either unlimited connections or a
            // buffer pool that throws under load.
            buffer = new RenderBuffer();
            Created++;
        }

        buffer.TimesRented++;
        return buffer;
    }

    public void Return(RenderBuffer buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        // Cleared on RETURN, not on rent. A dirty buffer sitting in the pool holds every
        // string it captured alive until somebody happens to rent it - a leak that looks
        // exactly like normal usage from a memory profile.
        buffer.Lines.Clear();
        buffer.Title = null;

        // Capped. Keeping everything returned makes this an unbounded cache of objects
        // nobody is using, which is the opposite of the point.
        if (_idle.Count < capacity)
            _idle.Push(buffer);
    }
}
