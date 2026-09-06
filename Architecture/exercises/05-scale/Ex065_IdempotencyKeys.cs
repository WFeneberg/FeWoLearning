namespace FeWoLearning.Architecture.Exercises.Scale.Ex065;

public sealed record StoredResponse(int StatusCode, string Body);

/// <summary>
/// Where completed responses are remembered, keyed by the client's idempotency key. An
/// interface, so the same exercise code runs against an in-memory store and against
/// Redis.
/// </summary>
public interface IIdempotencyStore
{
    (string RequestFingerprint, StoredResponse Response)? Get(string key);

    void Save(string key, string requestFingerprint, StoredResponse response);
}

public sealed class InMemoryIdempotencyStore : IIdempotencyStore
{
    private readonly Dictionary<string, (string Fingerprint, StoredResponse Response)> _entries = [];

    public (string RequestFingerprint, StoredResponse Response)? Get(string key) =>
        _entries.TryGetValue(key, out var entry) ? entry : null;

    public void Save(string key, string requestFingerprint, StoredResponse response) =>
        _entries[key] = (requestFingerprint, response);
}

/// <summary>The client reused a key for a genuinely different request.</summary>
public sealed class IdempotencyConflictException(string key)
    : Exception($"Idempotency key '{key}' was already used for a different request.")
{
    public string Key { get; } = key;
}

// Exercise 065 — IdempotencyKeys (scale).
// Goal:   Let a client retry a request that changes something, without the change
//         happening twice.
// Drills: idempotency keys, stored responses, replay vs re-execution, key reuse.
// Passes: first call  - the handler runs, and its response is remembered under the key.
//         retry       - the SAME key with the SAME body replays the stored response and
//                       does NOT run the handler again. The handler's invocation count is
//                       the mechanism; the response body alone proves nothing.
//         THE ONE      - the same key with a DIFFERENT body is a CONFLICT, not a replay.
//                       The client has a bug, and silently returning the first response
//                       hides it while quietly dropping the second request.
//         new key     - a different key runs the handler again.
//         failure     - a handler that throws stores nothing, so the retry can succeed.
//
// This is exercise 033 moved to the edge of the system, and the difference matters. A
// consumer dedupes on an id the PRODUCER generated, and it can simply ignore a duplicate.
// An HTTP endpoint dedupes on a key the CLIENT generated, and it has to answer - with the
// same answer as last time, because the client never saw it. That is why the response is
// stored and not just the key.
//
// A network timeout is indistinguishable from a slow success. The client that retries has
// no idea whether the first attempt charged the card, and neither does anything else
// unless the key is there.
public static class Ex065_IdempotencyKeys
{
    /// <summary>
    /// Run <paramref name="handler"/> for <paramref name="requestBody"/> under
    /// <paramref name="key"/>, or replay what it returned last time. Use
    /// <paramref name="requestBody"/> itself as the fingerprint.
    /// </summary>
    public static StoredResponse Handle(
        IIdempotencyStore store, string key, string requestBody, Func<string, StoredResponse> handler) =>
        throw new NotImplementedException(
            "TODO: Ex065 - replay a stored response for the same key and body, reject the same key with a different body, and only store a response the handler actually produced");
}
