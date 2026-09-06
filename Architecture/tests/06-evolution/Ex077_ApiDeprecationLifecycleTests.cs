using FeWoLearning.Architecture.Exercises.Evolution.Ex077;
using FeWoLearning.Architecture.Exercises.Support;

namespace FeWoLearning.Architecture.Tests.Evolution;

public class Ex077_ApiDeprecationLifecycleTests
{
    private static readonly DateTimeOffset Now = new(2026, 6, 1, 0, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset DeprecatedOn = new(2026, 3, 1, 0, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset SunsetOn = new(2026, 9, 1, 0, 0, 0, TimeSpan.Zero);

    private static (DeprecationGate Gate, ManualClock Clock, UsageLog Usage) Build()
    {
        var clock = new ManualClock(Now);
        var usage = new UsageLog();

        var policies = new Dictionary<string, EndpointPolicy>
        {
            ["/v1/orders"] = new("/v1/orders", DeprecatedOn, SunsetOn, "/v2/orders"),
            ["/v2/orders"] = new("/v2/orders", null, null, null),
        };

        return (new DeprecationGate(clock, policies, usage), clock, usage);
    }

    [Fact]
    public void A_Live_Endpoint_Says_Nothing()
    {
        var (gate, _, _) = Build();

        var (status, headers) = gate.Handle("/v2/orders", "billing");

        Assert.Equal(200, status);
        Assert.False(headers.Deprecation);
        Assert.Null(headers.Sunset);
    }

    [Fact]
    public void Mechanism_A_Deprecated_Endpoint_Still_Works_And_Says_Until_When()
    {
        // Deprecated does not mean broken; it means "we have told you, and here is until
        // when". Failing early is a removal announced in the past tense, and it breaks
        // callers who were given a date and were using it.
        var (gate, _, _) = Build();

        var (status, headers) = gate.Handle("/v1/orders", "billing");

        Assert.Equal(200, status);
        Assert.True(headers.Deprecation);
        Assert.Equal(SunsetOn, headers.Sunset);
        Assert.Equal("/v2/orders", headers.Link);
    }

    [Fact]
    public void Adversarial_The_Notice_Points_At_A_Replacement()
    {
        // "This is going away" without "use this instead" is a complaint rather than a
        // migration, and it is the difference between a caller who moves and one who
        // opens a ticket asking what to do.
        var (gate, _, _) = Build();

        Assert.Equal("/v2/orders", gate.Handle("/v1/orders", "billing").Headers.Link);
    }

    [Fact]
    public void Past_The_Sunset_The_Endpoint_Is_Gone()
    {
        var (gate, clock, _) = Build();
        clock.Advance(SunsetOn - Now);

        var (status, headers) = gate.Handle("/v1/orders", "billing");

        Assert.Equal(410, status);
        Assert.True(headers.Deprecation);
    }

    [Fact]
    public void Mechanism_Calls_To_A_Deprecated_Endpoint_Are_Recorded_Per_Consumer()
    {
        // The part that gets skipped, and the only one that makes the sunset date
        // meaningful. Without it, the choice on the day is between deleting something that
        // might still be load-bearing and postponing again - and postponing is free, so it
        // is what happens, every time.
        var (gate, _, usage) = Build();

        gate.Handle("/v1/orders", "billing");
        gate.Handle("/v1/orders", "reporting");
        gate.Handle("/v1/orders", "billing");

        Assert.Equal(["billing", "reporting"], usage.ConsumersOf("/v1/orders"));
    }

    [Fact]
    public void Adversarial_Calls_After_The_Sunset_Are_Recorded_Too()
    {
        // A caller still hitting a removed endpoint is exactly who somebody needs to hear
        // from. An implementation that returns 410 before recording anything loses the one
        // piece of information the 410 generated.
        var (gate, clock, usage) = Build();
        clock.Advance(SunsetOn - Now);

        gate.Handle("/v1/orders", "forgotten-batch-job");

        Assert.Equal(["forgotten-batch-job"], usage.ConsumersOf("/v1/orders"));
    }

    [Fact]
    public void An_Endpoint_With_No_Policy_Is_Simply_Live()
    {
        var (gate, _, usage) = Build();

        Assert.Equal(200, gate.Handle("/v3/something-new", "billing").StatusCode);
        Assert.Empty(usage.ConsumersOf("/v3/something-new"));
    }

    [Fact]
    public void Before_The_Deprecation_Date_Nothing_Is_Announced()
    {
        // A policy can be written ahead of time. Announcing early is not harmless: callers
        // start migrating off an endpoint that is still the recommended one.
        var clock = new ManualClock(DeprecatedOn.AddDays(-1));
        var usage = new UsageLog();
        var gate = new DeprecationGate(
            clock,
            new Dictionary<string, EndpointPolicy> { ["/v1/orders"] = new("/v1/orders", DeprecatedOn, SunsetOn, "/v2/orders") },
            usage);

        var (status, headers) = gate.Handle("/v1/orders", "billing");

        Assert.Equal(200, status);
        Assert.False(headers.Deprecation);
        Assert.Empty(usage.ConsumersOf("/v1/orders"));
    }
}
