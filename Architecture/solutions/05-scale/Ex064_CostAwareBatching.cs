using FeWoLearning.Architecture.Exercises.Support;

namespace FeWoLearning.Architecture.Exercises.Scale.Ex064;

// Exercise 064 — CostAwareBatching (reference solution).
public sealed class Batcher<T>(IClock clock, int maxSize, TimeSpan maxAge, Action<IReadOnlyList<T>> flush)
{
    private readonly List<T> _buffer = [];

    // When the OLDEST item arrived - not when the last flush happened. Measuring from the
    // last flush flushes a fresh item early after an idle period and, worse, resets on
    // every flush, so a steady trickle can leave an item waiting almost 2 x maxAge. The
    // guarantee people believe they are buying is "no item waits longer than maxAge".
    private DateTimeOffset? _oldestArrivedAt;

    public int Pending => _buffer.Count;

    public void Add(T item)
    {
        if (_buffer.Count == 0)
            _oldestArrivedAt = clock.UtcNow;

        _buffer.Add(item);

        if (_buffer.Count >= maxSize)
            Flush();
    }

    public void Tick()
    {
        if (_oldestArrivedAt is not { } oldest)
            return;

        if (clock.UtcNow - oldest >= maxAge)
            Flush();
    }

    public void Flush()
    {
        // An empty batch is never handed over. A flush handler that opens a transaction,
        // or bills per call, should not be invoked to do nothing.
        if (_buffer.Count == 0)
            return;

        var batch = _buffer.ToArray();
        _buffer.Clear();
        _oldestArrivedAt = null;

        flush(batch);
    }
}
