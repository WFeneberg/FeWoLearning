namespace FeWoLearning.Architecture.Exercises.Support;

/// <summary>
/// The time port every time-dependent exercise depends on, so that no test in this
/// track ever sleeps. Rate limiters, caches with a TTL, lock leases, retry backoff
/// and circuit breakers all read time through this and nothing else.
/// </summary>
public interface IClock
{
    DateTimeOffset UtcNow { get; }
}

/// <summary>
/// A clock the test drives by hand. Shared fixture - never a TODO, never a catalog row.
/// Byte-identical in exercises/_support and solutions/_support.
/// </summary>
public sealed class ManualClock(DateTimeOffset start) : IClock
{
    public DateTimeOffset UtcNow { get; private set; } = start;

    public void Advance(TimeSpan by) => UtcNow = UtcNow.Add(by);
}
