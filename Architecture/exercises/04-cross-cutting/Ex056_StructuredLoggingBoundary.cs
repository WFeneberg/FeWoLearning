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

// Exercise 056 — StructuredLoggingBoundary (cross-cutting).
// Goal:   Attach the context an entry needs without threading it through every method
//         signature, and without turning it into prose.
// Drills: log scopes, nesting, field precedence, keeping formatting out of the caller.
// Passes: no scope    - an entry carries only its own fields.
//         one scope   - the scope's fields are merged into every entry inside it.
//         nesting     - both scopes contribute; the INNER one wins a key both define.
//         exit        - once a scope is disposed its fields are gone.
//         THE ONE      - the message is the message. "Processing order O-1" is a
//                       different string from "Processing order", and only the second one
//                       can be grouped, counted or alerted on.
//
// A message with the values interpolated into it is a unique string per occurrence.
// Every log system in existence groups by message template, so an interpolated message
// produces one group per event, which makes "how often does this happen" unanswerable and
// "alert me when this happens more than usual" impossible to express. The values belong
// in fields, where they can be filtered and aggregated.
//
// Scope disposal must be LIFO, and disposing out of order should not corrupt what
// remains - it happens, and the resulting entries are worse than no entries.
public sealed class ScopedLogger(ILogSink sink)
{
    /// <summary>Add fields to every entry written until the returned token is disposed.</summary>
    public IDisposable BeginScope(params (string Name, object? Value)[] fields) =>
        throw new NotImplementedException(
            "TODO: Ex056 - push these fields and return a token that pops them");

    /// <summary>Write an entry, merging the active scopes' fields with its own.</summary>
    public void Log(string message, params (string Name, object? Value)[] fields) =>
        throw new NotImplementedException(
            "TODO: Ex056 - merge outer scopes first, then inner ones, then the entry's own fields, and write");
}
