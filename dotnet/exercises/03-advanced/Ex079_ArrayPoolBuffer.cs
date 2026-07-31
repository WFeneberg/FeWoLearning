namespace FeWoLearning.Exercises.Advanced;

// Exercise 079 — ArrayPool<T> buffer reuse (advanced).
// Goal:   Rent a byte buffer from ArrayPool<byte>, process data into it, and return the
//         processed result while guaranteeing the rented buffer is released back to the
//         pool exactly once — even if release is attempted more than once.
// Drills: System.Buffers.ArrayPool<T>, Span<T>, idempotent IDisposable, resource ownership.
public sealed class ArrayPoolBuffer : IDisposable
{
    public static ArrayPoolBuffer Rent(int minimumLength) => throw new NotImplementedException();

    public int Length => throw new NotImplementedException();

    public bool IsReturned => throw new NotImplementedException();

    // Applies `transform` to each byte of `source` using the rented buffer as scratch space,
    // then returns a right-sized copy of the processed bytes.
    public byte[] Process(ReadOnlySpan<byte> source, Func<byte, byte> transform) => throw new NotImplementedException();

    public void Dispose() => throw new NotImplementedException();
}
