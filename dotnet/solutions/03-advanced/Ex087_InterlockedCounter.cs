using System.Threading;

namespace FeWoLearning.Exercises.Advanced;

// Exercise 087 — Interlocked atomic counter (reference solution).
// Interlocked.* compiles to atomic CPU instructions (e.g. LOCK XADD on x86),
// so concurrent callers never interleave a read-modify-write and never lose an update.
public sealed class InterlockedCounter
{
    private long _value;

    public InterlockedCounter(long initialValue = 0) => _value = initialValue;

    // Interlocked.Read gives a torn-read-free view even on 32-bit runtimes.
    public long Value => Interlocked.Read(ref _value);

    public long Increment() => Interlocked.Increment(ref _value);

    public long Add(long amount) => Interlocked.Add(ref _value, amount);

    public long Reset() => Interlocked.Exchange(ref _value, 0);
}
