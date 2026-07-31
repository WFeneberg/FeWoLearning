namespace FeWoLearning.Exercises.Intermediate;

// Exercise 044 — Memory Buffer Processor (reference solution).
public static class MemoryBufferProcessor
{
    public static long ComputeChecksum(Memory<byte> buffer, int chunkSize)
    {
        if (chunkSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(chunkSize), "chunkSize must be positive.");
        }

        long checksum = 0;
        int offset = 0;
        int chunkIndex = 0;

        while (offset < buffer.Length)
        {
            int length = Math.Min(chunkSize, buffer.Length - offset);
            ReadOnlySpan<byte> chunk = buffer.Slice(offset, length).Span;

            long chunkSum = 0;
            foreach (byte b in chunk)
            {
                chunkSum += b;
            }

            checksum += chunkSum ^ chunkIndex;

            offset += length;
            chunkIndex++;
        }

        return checksum;
    }
}
