using System;
using System.Text;
using FeWoLearning.Exercises.Advanced;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Advanced;

public class Ex079_ArrayPoolBufferTests
{
    [Fact]
    public void Process_TransformsBytesCorrectly()
    {
        using var buffer = ArrayPoolBuffer.Rent(16);
        var source = Encoding.ASCII.GetBytes("hello");

        var result = buffer.Process(source, b => (byte)char.ToUpperInvariant((char)b));

        Assert.Equal("HELLO", Encoding.ASCII.GetString(result));
        Assert.Equal(source.Length, result.Length);
    }

    [Fact]
    public void RentedBufferMeetsRequestedMinimumLength()
    {
        using var buffer = ArrayPoolBuffer.Rent(100);

        Assert.True(buffer.Length >= 100);
    }

    [Fact]
    public void DisposeIsIdempotent_NoDoubleReturnError()
    {
        var buffer = ArrayPoolBuffer.Rent(32);
        Assert.False(buffer.IsReturned);

        buffer.Dispose();
        Assert.True(buffer.IsReturned);

        var ex = Record.Exception(() => buffer.Dispose()); // second dispose must be a safe no-op
        Assert.Null(ex);
        Assert.True(buffer.IsReturned);
    }

    [Fact]
    public void ProcessAfterDispose_ThrowsObjectDisposed()
    {
        var buffer = ArrayPoolBuffer.Rent(8);
        buffer.Dispose();

        Assert.Throws<ObjectDisposedException>(() =>
            buffer.Process(new byte[] { 1, 2, 3 }, b => b));
    }

    [Fact]
    public void MultipleBuffersDoNotShareState()
    {
        using var a = ArrayPoolBuffer.Rent(10);
        using var b = ArrayPoolBuffer.Rent(10);

        var resultA = a.Process(new byte[] { 1, 2, 3 }, x => (byte)(x + 1));
        var resultB = b.Process(new byte[] { 10, 20, 30 }, x => (byte)(x * 2));

        Assert.Equal(new byte[] { 2, 3, 4 }, resultA);
        Assert.Equal(new byte[] { 20, 40, 60 }, resultB);
    }

    [Fact]
    public void Process_RejectsSourceLargerThanRentedBuffer()
    {
        using var buffer = ArrayPoolBuffer.Rent(4);
        var tooLarge = new byte[buffer.Length + 1];

        Assert.Throws<ArgumentException>(() => buffer.Process(tooLarge, x => x));
    }
}
