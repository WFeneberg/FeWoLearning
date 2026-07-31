namespace FeWoLearning.Exercises.Advanced;

// Exercise 087 — Interlocked atomic counter (advanced).
// Goal:   A thread-safe counter that many tasks can increment concurrently
//         without losing updates (no locks — atomic CPU instructions only).
// Drills: System.Threading.Interlocked, race conditions, concurrent correctness.
public sealed class InterlockedCounter
{
    public InterlockedCounter(long initialValue = 0) => throw new NotImplementedException();

    // Current value of the counter (must be read atomically).
    public long Value => throw new NotImplementedException();

    // Atomically increments the counter by 1 and returns the new value.
    public long Increment() => throw new NotImplementedException();

    // Atomically adds 'amount' (may be negative) and returns the new value.
    public long Add(long amount) => throw new NotImplementedException();

    // Atomically resets the counter to 0 and returns the value it held before the reset.
    public long Reset() => throw new NotImplementedException();
}
