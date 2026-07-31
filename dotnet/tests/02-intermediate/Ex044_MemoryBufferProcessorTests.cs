using FeWoLearning.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Intermediate;

public class Ex044_MemoryBufferProcessorTests
{
    [Fact]
    public void ComputeChecksum_TenByteBufferThreeByteChunks_ReturnsExpectedValue()
    {
        // Chunks: [1,2,3]=6, [4,5,6]=15, [7,8,9]=24, [10]=10
        // checksum = (6^0) + (15^1) + (24^2) + (10^3) = 6 + 14 + 26 + 9 = 55
        Memory<byte> buffer = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        long result = MemoryBufferProcessor.ComputeChecksum(buffer, 3);

        Assert.Equal(55, result);
    }

    [Fact]
    public void ComputeChecksum_ChunkSizeLargerThanBuffer_TreatsWholeBufferAsSingleChunk()
    {
        // Single chunk: sum = 1+2+3+4+5 = 15, checksum = 15 ^ 0 = 15
        Memory<byte> buffer = new byte[] { 1, 2, 3, 4, 5 };

        long result = MemoryBufferProcessor.ComputeChecksum(buffer, 100);

        Assert.Equal(15, result);
    }

    [Fact]
    public void ComputeChecksum_EmptyBuffer_ReturnsZero()
    {
        Memory<byte> buffer = Array.Empty<byte>();

        long result = MemoryBufferProcessor.ComputeChecksum(buffer, 4);

        Assert.Equal(0, result);
    }

    [Fact]
    public void ComputeChecksum_NonZeroChunkIndexAffectsResult()
    {
        // Chunks of size 2 over [10, 20, 30, 40]:
        // [10,20]=30 ^0 = 30
        // [30,40]=70 ^1 = 71
        // total = 101
        Memory<byte> buffer = new byte[] { 10, 20, 30, 40 };

        long result = MemoryBufferProcessor.ComputeChecksum(buffer, 2);

        Assert.Equal(101, result);
    }

    [Fact]
    public void ComputeChecksum_NonPositiveChunkSize_Throws()
    {
        Memory<byte> buffer = new byte[] { 1, 2, 3 };

        Assert.Throws<ArgumentOutOfRangeException>(() => MemoryBufferProcessor.ComputeChecksum(buffer, 0));
    }
}
