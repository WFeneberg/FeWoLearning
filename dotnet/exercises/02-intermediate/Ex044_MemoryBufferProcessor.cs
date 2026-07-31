namespace FeWoLearning.Exercises.Intermediate;

// Exercise 044 — Memory Buffer Processor (intermediate).
// Goal:   Compute a checksum over a Memory<byte> buffer by processing it in
//         fixed-size chunks without copying the underlying data.
// Drills: Memory<T>/Span<T> basics, Memory<T>.Slice, ReadOnlySpan<T> iteration,
//         chunked processing, argument validation.
public static class MemoryBufferProcessor
{
    // Splits `buffer` into consecutive chunks of at most `chunkSize` bytes
    // (the final chunk may be shorter). For each chunk (0-based `chunkIndex`),
    // sum its bytes into `chunkSum`, then XOR that sum with `chunkIndex` and
    // add the result to a running total. Returns the running total after all
    // chunks have been processed.
    // Throws ArgumentOutOfRangeException if chunkSize <= 0.
    public static long ComputeChecksum(Memory<byte> buffer, int chunkSize) => throw new NotImplementedException();
}
