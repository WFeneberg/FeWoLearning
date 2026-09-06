using System.Diagnostics.Metrics;

namespace FeWoLearning.Telemetry.Tests.Harness;

/// <summary>One measurement exactly as the BCL delivered it.</summary>
/// <param name="Instrument">The instrument it came from.</param>
/// <param name="Value">The value, widened to double so one probe covers every T.</param>
/// <param name="Tags">The dimensions attached at the call site.</param>
public sealed record Measurement(
    string Instrument, double Value, IReadOnlyList<KeyValuePair<string, object?>> Tags)
{
    /// <summary>The value of one dimension, or null when this measurement has no such tag.</summary>
    public string? Tag(string key) =>
        Tags.FirstOrDefault(t => t.Key == key).Value?.ToString();
}

/// <summary>
/// A raw <see cref="MeterListener"/> over EXACTLY ONE meter name.
///
/// This is the BCL-level counterpart to <see cref="MetricProbe"/>, and the two are not
/// interchangeable: block <c>02-diagnostics</c> is explicitly about the primitives
/// before any SDK, so its rows are graded on the measurements the runtime delivers,
/// never on what an OpenTelemetry pipeline made of them.
///
/// It records the four numeric types the instruments in this block use. A row needing
/// another one adds it here rather than building a second listener.
/// </summary>
public sealed class MeasurementProbe : IDisposable
{
    private readonly List<Measurement> _measurements = [];
    private readonly List<Instrument> _published = [];
    private readonly MeterListener _listener;

    public MeasurementProbe(string meterName)
    {
        _listener = new MeterListener
        {
            InstrumentPublished = (instrument, listener) =>
            {
                if (instrument.Meter.Name != meterName) return;

                lock (_published) _published.Add(instrument);
                listener.EnableMeasurementEvents(instrument);
            },
        };

        _listener.SetMeasurementEventCallback<long>(Record);
        _listener.SetMeasurementEventCallback<int>(Record);
        _listener.SetMeasurementEventCallback<double>(Record);
        _listener.SetMeasurementEventCallback<float>(Record);

        // Nothing is delivered until Start. A listener that wires up every callback and
        // forgets this line receives absolute silence, with no error to explain it.
        _listener.Start();
    }

    /// <summary>Every measurement so far, oldest first.</summary>
    public IReadOnlyList<Measurement> Measurements
    {
        get { lock (_measurements) return _measurements.ToArray(); }
    }

    /// <summary>The names of the instruments this meter published, in publication order.</summary>
    public IReadOnlyList<string> PublishedInstruments
    {
        get { lock (_published) return _published.Select(i => i.Name).ToArray(); }
    }

    /// <summary>
    /// The declared unit of one published instrument, or null when it declared none.
    /// The unit lives on the instrument, not on the measurement.
    /// </summary>
    public string? UnitOf(string instrument)
    {
        lock (_published) return _published.FirstOrDefault(i => i.Name == instrument)?.Unit;
    }

    /// <summary>Measurements from one instrument.</summary>
    public IReadOnlyList<Measurement> For(string instrument) =>
        Measurements.Where(m => m.Instrument == instrument).ToArray();

    /// <summary>
    /// Poll every observable instrument. Observable instruments deliver NOTHING until
    /// this is called - they are a pull model, and that is their whole point.
    /// </summary>
    public void Poll() => _listener.RecordObservableInstruments();

    public void Dispose() => _listener.Dispose();

    private void Record<T>(
        Instrument instrument,
        T value,
        ReadOnlySpan<KeyValuePair<string, object?>> tags,
        object? state)
        where T : struct
    {
        var measurement = new Measurement(
            instrument.Name,
            Convert.ToDouble(value),
            tags.ToArray());

        lock (_measurements) _measurements.Add(measurement);
    }
}
