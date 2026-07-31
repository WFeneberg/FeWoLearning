namespace FeWoLearning.Exercises.Advanced;

// Exercise 072 — Channel<T> producer/consumer pipeline (advanced).
// Goal:   Build a bounded producer/consumer pipeline on top of System.Threading.Channels
//         that preserves the exact order items were written, even under backpressure
//         (capacity smaller than the number of items).
// Drills: System.Threading.Channels, async producer/consumer, backpressure, ordering guarantees.
public static class ChannelPipeline
{
    // Writes every item from 'items' into a bounded Channel<T> of the given capacity on a
    // background producer, while concurrently reading everything back out on the caller
    // side (or another background consumer). Must return the consumed items in the exact
    // order they were produced. Must not deadlock when items.Count() > capacity.
    public static Task<List<T>> RunAsync<T>(IEnumerable<T> items, int capacity)
        => throw new NotImplementedException();
}
