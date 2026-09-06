namespace FeWoLearning.Architecture.Exercises.CrossCutting.Ex056;

/// <summary>
/// Where entries end up. The message and the fields arrive separately, and stay
/// separate - rendering them into one string is the sink's decision, not the domain's.
/// </summary>
public interface ILogSink
{
    void Write(string message, IReadOnlyDictionary<string, object?> fields);
}

public sealed class RecordingSink : ILogSink
{
    public List<(string Message, IReadOnlyDictionary<string, object?> Fields)> Entries { get; } = [];

    public void Write(string message, IReadOnlyDictionary<string, object?> fields) =>
        Entries.Add((message, fields));
}

// Exercise 056 — StructuredLoggingBoundary (reference solution).
public sealed class ScopedLogger(ILogSink sink)
{
    private readonly List<(string Name, object? Value)[]> _scopes = [];

    public IDisposable BeginScope(params (string Name, object? Value)[] fields)
    {
        _scopes.Add(fields);
        return new Scope(() => _scopes.Remove(fields));
    }

    public void Log(string message, params (string Name, object? Value)[] fields)
    {
        var merged = new Dictionary<string, object?>(StringComparer.Ordinal);

        // Outermost first, so an inner scope overwrites an outer one, and the entry's own
        // fields overwrite both. Anything more specific wins - which is the only ordering
        // a reader would guess.
        foreach (var scope in _scopes)
            foreach (var (name, value) in scope)
                merged[name] = value;

        foreach (var (name, value) in fields)
            merged[name] = value;

        // message goes through untouched. Interpolating the values into it produces a
        // unique string per occurrence, and every log system groups by message template -
        // so "how often does this happen" becomes unanswerable.
        sink.Write(message, merged);
    }

    /// <summary>Removes exactly its own frame, so an out-of-order dispose cannot corrupt the rest.</summary>
    private sealed class Scope(Action pop) : IDisposable
    {
        private Action? _pop = pop;

        public void Dispose()
        {
            var action = _pop;
            _pop = null;
            action?.Invoke();
        }
    }
}
