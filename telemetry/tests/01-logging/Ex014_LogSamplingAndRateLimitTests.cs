using FeWoLearning.Telemetry.Exercises.Logging;
using FeWoLearning.Telemetry.Tests.Harness;
using Microsoft.Extensions.Logging;

namespace FeWoLearning.Telemetry.Tests.Logging;

public class Ex014_LogSamplingAndRateLimitTests
{
    private static readonly DateTimeOffset Origin = new(2026, 9, 6, 12, 0, 0, TimeSpan.Zero);

    /// <summary>A clock the test moves by hand. No sleeping, ever.</summary>
    private sealed class VirtualClock
    {
        public TimeSpan Elapsed { get; private set; }

        public DateTimeOffset Now() => Origin + Elapsed;

        public void AdvanceTo(int seconds) => Elapsed = TimeSpan.FromSeconds(seconds);
    }

    private static readonly EventId Flood = new(9001, "Flood");
    private static readonly EventId Other = new(9002, "Other");

    private static void Write(ILogger logger, EventId id, string marker) =>
        logger.LogWarning(id, "event {Marker}", marker);

    [Fact]
    public void The_budget_lets_the_first_records_through_and_drops_the_rest()
    {
        using var logs = new LogProbe();
        var clock = new VirtualClock();
        var limited = Ex014_LogSamplingAndRateLimit.RateLimit(logs.For("flood"), clock.Now);

        for (var i = 0; i < 10; i++) Write(limited, Flood, $"a{i}");

        Assert.Equal(Ex014_LogSamplingAndRateLimit.MaxPerWindow, logs.Records.Count);
        Assert.Equal(["a0", "a1", "a2"], logs.Records.Select(r => LogProbe.Field(r, "Marker")));
    }

    [Fact]
    public void Adversarial_A_Each_event_id_has_its_own_independent_budget()
    {
        // The failure this prevents is the nastiest kind: a component melts down,
        // floods its own event, and in doing so silences every OTHER event in the
        // process - including the one that would have told you what happened.
        using var logs = new LogProbe();
        var clock = new VirtualClock();
        var limited = Ex014_LogSamplingAndRateLimit.RateLimit(logs.For("flood"), clock.Now);

        for (var i = 0; i < 10; i++) Write(limited, Flood, $"a{i}");
        Write(limited, Other, "b0");

        Assert.Equal(4, logs.Records.Count);
        Assert.Equal("b0", LogProbe.Field(logs.Records[^1], "Marker"));
    }

    [Fact]
    public void A_new_window_opens_once_the_old_one_has_elapsed()
    {
        using var logs = new LogProbe();
        var clock = new VirtualClock();
        var limited = Ex014_LogSamplingAndRateLimit.RateLimit(logs.For("flood"), clock.Now);

        for (var i = 0; i < 10; i++) Write(limited, Flood, $"a{i}");

        clock.AdvanceTo(15);
        Write(limited, Flood, "b0");

        Assert.Equal(4, logs.Records.Count);
        Assert.Equal("b0", LogProbe.Field(logs.Records[^1], "Marker"));
    }

    [Fact]
    public void Adversarial_B_The_new_window_is_anchored_on_the_record_that_opened_it()
    {
        // The design decision the easy test cannot see. A limiter that resets on a
        // FIXED GRID - every ten seconds on the clock - hands a caller a fresh budget
        // the instant the grid ticks over, so a flood arriving just before a boundary
        // gets two budgets back to back.
        //
        // Window one opens at t=0 and closes at t=10. The next record arrives at t=15
        // and opens window two, which must run to t=25. A grid implementation would
        // instead treat [10,20) as window two and [20,30) as window three, and would
        // therefore let the t=22 record through.
        using var logs = new LogProbe();
        var clock = new VirtualClock();
        var limited = Ex014_LogSamplingAndRateLimit.RateLimit(logs.For("flood"), clock.Now);

        Write(limited, Flood, "a0");

        clock.AdvanceTo(15);
        Write(limited, Flood, "b0");
        Write(limited, Flood, "b1");
        Write(limited, Flood, "b2");

        clock.AdvanceTo(22);
        Write(limited, Flood, "grid-would-let-this-through");

        clock.AdvanceTo(25);
        Write(limited, Flood, "c0");

        Assert.Equal(
            ["a0", "b0", "b1", "b2", "c0"],
            logs.Records.Select(r => LogProbe.Field(r, "Marker")));
    }

    [Fact]
    public void Adversarial_C_IsEnabled_and_BeginScope_reach_the_inner_logger()
    {
        // A limiter is a decorator, and a decorator that forgets to forward IsEnabled
        // answers "yes" to everything, so every filter rule in the application stops
        // working - silently, as always.
        using var logs = new LogProbe(builder => builder.SetMinimumLevel(LogLevel.Warning));
        var clock = new VirtualClock();
        var limited = Ex014_LogSamplingAndRateLimit.RateLimit(logs.For("flood"), clock.Now);

        Assert.False(limited.IsEnabled(LogLevel.Information));
        Assert.True(limited.IsEnabled(LogLevel.Warning));

        using (limited.BeginScope(new Dictionary<string, object> { ["TenantId"] = "acme" }))
        {
            Write(limited, Flood, "a0");
        }

        Assert.Equal("acme", LogProbe.ScopeValue(Assert.Single(logs.Records), 0, "TenantId"));
    }
}
