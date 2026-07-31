using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace FeWoLearning.Exercises.Expert;

// Exercise 100 — Backpressure pipeline (reference solution).
// A bounded Channel<T> already suspends writers when full; the work here is
// wiring that up as a reusable stage with accurate produced/consumed/in-flight
// bookkeeping, and completing the channel (optionally with an error) cleanly.
public sealed class BackpressurePipeline<T>
{
    private readonly Channel<T> _channel;
    private readonly object _gate = new();
    private long _produced;
    private long _consumed;
    private int _maxObservedInFlight;

    public BackpressurePipeline(int capacity)
    {
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacity), capacity, "Must be positive.");

        Capacity = capacity;
        _channel = Channel.CreateBounded<T>(new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = false,
            SingleWriter = false,
        });
    }

    public int Capacity { get; }

    public long ProducedCount
    {
        get { lock (_gate) return _produced; }
    }

    public long ConsumedCount
    {
        get { lock (_gate) return _consumed; }
    }

    public int MaxObservedInFlight
    {
        get { lock (_gate) return _maxObservedInFlight; }
    }

    public int InFlightCount
    {
        get { lock (_gate) return (int)(_produced - _consumed); }
    }

    public ValueTask ProduceAsync(T item, CancellationToken cancellationToken = default)
    {
        var writeTask = _channel.Writer.WriteAsync(item, cancellationToken);
        if (writeTask.IsCompletedSuccessfully)
        {
            // Fast path: capacity was available, so the write — and the
            // bookkeeping — happen synchronously with no suspension.
            RecordProduced();
            return ValueTask.CompletedTask;
        }

        return AwaitAndRecordAsync(writeTask);
    }

    private async ValueTask AwaitAndRecordAsync(ValueTask writeTask)
    {
        await writeTask.ConfigureAwait(false);
        RecordProduced();
    }

    private void RecordProduced()
    {
        lock (_gate)
        {
            _produced++;
            var inFlight = (int)(_produced - _consumed);
            if (inFlight > _maxObservedInFlight)
                _maxObservedInFlight = inFlight;
        }
    }

    public async IAsyncEnumerable<T> ConsumeAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var item in _channel.Reader.ReadAllAsync(cancellationToken).ConfigureAwait(false))
        {
            lock (_gate) _consumed++;
            yield return item;
        }
    }

    public void Complete(Exception? error = null) => _channel.Writer.Complete(error);
}
