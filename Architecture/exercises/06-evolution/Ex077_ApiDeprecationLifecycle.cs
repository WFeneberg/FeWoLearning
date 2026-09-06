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

// Exercise 077 — ApiDeprecationLifecycle (evolution).
// Goal:   Retire an endpoint on a schedule the callers can see, and know before removing
//         it whether anybody is still there.
// Drills: deprecation vs sunset, machine-readable notice, usage evidence.
// Passes: live       - 200, no deprecation headers.
//         deprecated - STILL 200, plus the Deprecation header, the Sunset date and a Link
//                      to the replacement. Deprecated does not mean broken; it means "we
//                      have told you, and here is until when".
//         sunset     - once the sunset date has passed, 410 Gone.
//         THE ONE     - every call to a deprecated endpoint is RECORDED with its consumer.
//                      Removal is a decision about who is still calling, and without the
//                      log that decision is a guess dressed as a schedule.
//         unknown    - an endpoint with no policy is simply live.
//
// The three states exist because two are not enough. "Deprecated" that already fails is
// just a removal announced in the past tense, and "deprecated" with no end date is a
// warning nobody acts on - endpoints marked deprecated in 2019 are still being called,
// and everybody knows it, which is precisely why nobody hurries.
//
// The usage log is the part that gets skipped, and it is the only one that makes the
// sunset date meaningful. Without it, the choice on the day is between deleting something
// that might still be load-bearing and postponing again - and postponing is free, so it
// is what happens, every time.
public sealed class DeprecationGate(IClock clock, IReadOnlyDictionary<string, EndpointPolicy> policies, UsageLog usage)
{
    /// <summary>Handle one call. Returns the status and the headers the response carries.</summary>
    public (int StatusCode, ResponseHeaders Headers) Handle(string endpoint, string consumer) =>
        throw new NotImplementedException(
            "TODO: Ex077 - serve normally when live, serve WITH notice when deprecated, refuse with 410 past the sunset, and record who called a deprecated endpoint");
}
