using Microsoft.Extensions.Logging;

namespace FeWoLearning.Telemetry.Exercises.Logging;

// Exercise 014 — LogSamplingAndRateLimit (logging).
// Goal:   Survive the incident where one failing thing logs ten thousand times a
//         second, without losing the fact that it happened.
// Drills: per-event budgets, a window anchored on first use, an injected clock.
// Passes: the first MaxPerWindow records of an event id pass and the rest are dropped;
//         each event id gets its OWN budget, so a flood of one does not silence
//                     another;
//         once Window has elapsed since that event's window opened, the next record
//                     passes and opens a new window;
//         and the new window is anchored on THAT record, not on a fixed grid.
//
// The last clause is the whole design decision, and it is invisible in the easy test.
// A limiter that resets on a fixed grid - every ten seconds on the clock - hands a
// caller a fresh budget the instant the grid ticks over, so a flood arriving just
// before a boundary gets two budgets back to back. Anchoring the window on the record
// that opened it means the budget is spent from that moment, whenever it was.
//
// The clock is injected for a reason that is not testability: it is the only way the
// window can be reasoned about at all. A limiter reading DateTime.UtcNow cannot be
// tested without sleeping, and a test that sleeps is a test that is flaky on a busy
// build agent.
public static class Ex014_LogSamplingAndRateLimit
{
    /// <summary>How many records of one event id may pass per window.</summary>
    public const int MaxPerWindow = 3;

    /// <summary>How long a window lasts, from the record that opened it.</summary>
    public static readonly TimeSpan Window = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Wrap <paramref name="inner"/> so that, per <see cref="EventId"/>, at most
    /// <see cref="MaxPerWindow"/> records reach it per <see cref="Window"/>.
    ///
    /// A window OPENS when a record of that event id passes while no window is open
    /// for it, and lasts <see cref="Window"/> from that moment. Records beyond the
    /// budget while a window is open are dropped and must not reach
    /// <paramref name="inner"/> at all. Budgets are per event id and independent.
    ///
    /// Read the time only from <paramref name="clock"/>. IsEnabled and BeginScope
    /// belong to <paramref name="inner"/>.
    /// </summary>
    public static ILogger RateLimit(ILogger inner, Func<DateTimeOffset> clock) =>
        throw new NotImplementedException(
            "TODO: Ex014 - drop records past the per-event budget, with the window anchored on first use");
}
