namespace FeWoLearning.Exercises.Expert;

// Exercise 100 — Backpressure pipeline (expert).
// Goal:   Wrap a bounded System.Threading.Channels channel as a single pipeline
//         stage: ProduceAsync must genuinely suspend (not just logically "wait")
//         once the channel is full, so a slow downstream consumer throttles the
//         producer instead of letting unbounded work pile up in memory.
// Drills: System.Threading.Channels, ValueTask synchronous-completion semantics,
//         IAsyncEnumerable, backpressure vs. unbounded buffering.
public sealed class BackpressurePipeline<T>
{
    public BackpressurePipeline(int capacity) => throw new NotImplementedException();

    // Maximum number of items allowed to sit in the channel at once.
    public int Capacity => throw new NotImplementedException();

    // Total items that have completed a WriteAsync (i.e. entered the channel).
    public long ProducedCount => throw new NotImplementedException();

    // Total items that have been pulled out via ConsumeAsync.
    public long ConsumedCount => throw new NotImplementedException();

    // High-water mark of (ProducedCount - ConsumedCount) ever observed.
    // Must never exceed Capacity.
    public int MaxObservedInFlight => throw new NotImplementedException();

    // Items currently sitting in the channel (produced but not yet consumed).
    public int InFlightCount => throw new NotImplementedException();

    // Writes one item into the bounded channel. The returned ValueTask completes
    // synchronously when there is free capacity, and only completes once a
    // consumer has made room when the channel is full — this is the backpressure.
    public ValueTask ProduceAsync(T item, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    // Streams items out of the channel as they become available, tracking
    // ConsumedCount as each one is yielded.
    public IAsyncEnumerable<T> ConsumeAsync(CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    // Marks the pipeline as done producing. Any pending/future ConsumeAsync
    // enumeration drains remaining items and then completes. Passing an
    // exception propagates it to the consumer side.
    public void Complete(Exception? error = null) => throw new NotImplementedException();
}
