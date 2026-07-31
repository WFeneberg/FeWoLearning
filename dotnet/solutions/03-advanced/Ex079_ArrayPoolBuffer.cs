using System.Buffers;

namespace FeWoLearning.Exercises.Advanced;

// Exercise 079 — ArrayPool<T> buffer reuse (reference solution).
// The rented array is scratch space owned exclusively by this instance. A single `_returned`
// flag (flipped before the actual pool return happens) makes Dispose idempotent: whichever
// caller races to Dispose first performs the real return, everyone else is a no-op.
public sealed class ArrayPoolBuffer : IDisposable
{
    private byte[]? _rented;
    private bool _returned;

    private ArrayPoolBuffer(byte[] rented) => _rented = rented;

    public static ArrayPoolBuffer Rent(int minimumLength)
    {
        if (minimumLength < 0)
            throw new ArgumentOutOfRangeException(nameof(minimumLength), minimumLength, "Must be non-negative.");

        var rented = ArrayPool<byte>.Shared.Rent(minimumLength);
        return new ArrayPoolBuffer(rented);
    }

    public int Length
    {
        get
        {
            ThrowIfReturned();
            return _rented!.Length;
        }
    }

    public bool IsReturned => _returned;

    public byte[] Process(ReadOnlySpan<byte> source, Func<byte, byte> transform)
    {
        ThrowIfReturned();
        if (transform is null)
            throw new ArgumentNullException(nameof(transform));
        if (source.Length > _rented!.Length)
            throw new ArgumentException("Source exceeds the rented buffer's capacity.", nameof(source));

        var scratch = _rented.AsSpan(0, source.Length);
        for (int i = 0; i < source.Length; i++)
        {
            scratch[i] = transform(source[i]);
        }

        return scratch.ToArray();
    }

    public void Dispose()
    {
        if (_returned)
            return; // Already released — safe, idempotent no-op, never returns twice.

        _returned = true;
        var toReturn = _rented;
        _rented = null;
        if (toReturn is not null)
            ArrayPool<byte>.Shared.Return(toReturn, clearArray: true);
    }

    private void ThrowIfReturned()
    {
        if (_returned)
            throw new ObjectDisposedException(nameof(ArrayPoolBuffer));
    }
}
