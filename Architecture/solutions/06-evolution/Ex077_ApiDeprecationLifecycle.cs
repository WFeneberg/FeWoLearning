using FeWoLearning.Architecture.Exercises.Support;

namespace FeWoLearning.Architecture.Exercises.Evolution.Ex077;

public sealed record EndpointPolicy(string Endpoint, DateTimeOffset? DeprecatedOn, DateTimeOffset? SunsetOn, string? Replacement);

/// <summary>
/// What the response carries. Deprecation and Sunset are the RFC 8594 / RFC 9745 headers;
/// Link points at what to use instead, because "this is going away" without "use this" is
/// a complaint rather than a migration.
/// </summary>
public sealed record ResponseHeaders(bool Deprecation, DateTimeOffset? Sunset, string? Link);

/// <summary>Who called what, and when. The only evidence that removal is safe.</summary>
public sealed class UsageLog
{
    private readonly List<(string Endpoint, string Consumer, DateTimeOffset At)> _calls = [];

    public void Record(string endpoint, string consumer, DateTimeOffset at) => _calls.Add((endpoint, consumer, at));

    public IReadOnlyList<string> ConsumersOf(string endpoint) =>
        [.. _calls.Where(c => c.Endpoint == endpoint).Select(c => c.Consumer).Distinct().OrderBy(c => c, StringComparer.Ordinal)];
}

// Exercise 077 — ApiDeprecationLifecycle (reference solution).
public sealed class DeprecationGate(IClock clock, IReadOnlyDictionary<string, EndpointPolicy> policies, UsageLog usage)
{
    public (int StatusCode, ResponseHeaders Headers) Handle(string endpoint, string consumer)
    {
        var now = clock.UtcNow;

        if (!policies.TryGetValue(endpoint, out var policy))
            return (200, new ResponseHeaders(false, null, null));

        var deprecated = policy.DeprecatedOn is { } from && now >= from;

        if (deprecated)
            // Recorded BEFORE the outcome is decided, and for the sunset case too: a
            // caller still hitting a removed endpoint is exactly who somebody needs to
            // hear from. Without this log, the choice on the day is between deleting
            // something that might be load-bearing and postponing again - and postponing
            // is free, so it is what happens, every time.
            usage.Record(endpoint, consumer, now);

        if (policy.SunsetOn is { } until && now >= until)
            return (410, new ResponseHeaders(true, until, policy.Replacement));

        if (!deprecated)
            return (200, new ResponseHeaders(false, null, null));

        // Still 200. Deprecated does not mean broken - it means "we have told you, and
        // here is until when". Failing early is a removal announced in the past tense.
        return (200, new ResponseHeaders(true, policy.SunsetOn, policy.Replacement));
    }
}
