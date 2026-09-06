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

// Exercise 065 — IdempotencyKeys (reference solution).
public static class Ex065_IdempotencyKeys
{
    public static StoredResponse Handle(
        IIdempotencyStore store, string key, string requestBody, Func<string, StoredResponse> handler)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        if (store.Get(key) is { } existing)
        {
            // The fingerprint is checked, not just the key. The client reusing a key for a
            // different request has a bug, and silently returning the first response hides
            // it while quietly dropping the second request - which, at an endpoint that
            // moves money, is the worst of the three possible behaviours.
            if (existing.RequestFingerprint != requestBody)
                throw new IdempotencyConflictException(key);

            // Replayed, not re-run. The client never saw the first response, so it needs
            // an answer - the SAME answer - and that is why the response is stored rather
            // than just the key.
            return existing.Response;
        }

        var response = handler(requestBody);

        // Only after the handler returned. Storing before it runs would make a failed
        // attempt permanent, so the retry that was supposed to fix it replays the failure
        // for ever.
        store.Save(key, requestBody, response);
        return response;
    }
}
