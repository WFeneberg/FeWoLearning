using FeWoLearning.Architecture.Exercises.CrossCutting.Ex054;
using FeWoLearning.Architecture.Exercises.Support;

namespace FeWoLearning.Architecture.Tests.CrossCutting;

public class Ex054_CircuitBreakerStatesTests
{
    private static readonly TimeSpan Break = TimeSpan.FromSeconds(30);

    private sealed class Dependency
    {
        public int Calls { get; private set; }

        public bool ShouldFail { get; set; } = true;

        public string Call()
        {
            Calls++;
            return ShouldFail ? throw new InvalidOperationException("upstream is down") : "ok";
        }
    }

    private static (CircuitBreaker Breaker, ManualClock Clock, Dependency Dependency) Build(int threshold = 3)
    {
        var clock = new ManualClock(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));
        return (new CircuitBreaker(clock, threshold, Break), clock, new Dependency());
    }

    private static void Trip(CircuitBreaker breaker, Dependency dependency, int threshold = 3)
    {
        for (var i = 0; i < threshold; i++)
            Assert.Throws<InvalidOperationException>(() => breaker.Execute(dependency.Call));
    }

    [Fact]
    public void A_Healthy_Circuit_Stays_Closed()
    {
        var (breaker, _, dependency) = Build();
        dependency.ShouldFail = false;

        Assert.Equal("ok", breaker.Execute(dependency.Call));
        Assert.Equal(CircuitState.Closed, breaker.State);
    }

    [Fact]
    public void A_Success_Resets_The_Failure_Count()
    {
        // Without the reset, three failures spread across a week trip the breaker - which
        // is not a broken dependency, it is a normal one.
        var (breaker, _, dependency) = Build();

        Assert.Throws<InvalidOperationException>(() => breaker.Execute(dependency.Call));
        Assert.Throws<InvalidOperationException>(() => breaker.Execute(dependency.Call));

        dependency.ShouldFail = false;
        breaker.Execute(dependency.Call);
        dependency.ShouldFail = true;

        Assert.Throws<InvalidOperationException>(() => breaker.Execute(dependency.Call));
        Assert.Equal(CircuitState.Closed, breaker.State);
    }

    [Fact]
    public void Mechanism_An_Open_Circuit_Does_Not_Call_The_Dependency_At_All()
    {
        // The entire point. A breaker that calls through and converts the failure is a
        // logging decorator: the broken dependency gets no rest, and the caller still
        // waits for its timeout.
        var (breaker, _, dependency) = Build();
        Trip(breaker, dependency);

        var callsWhenTripped = dependency.Calls;

        Assert.Throws<CircuitOpenException>(() => breaker.Execute(dependency.Call));
        Assert.Throws<CircuitOpenException>(() => breaker.Execute(dependency.Call));

        Assert.Equal(callsWhenTripped, dependency.Calls);
        Assert.Equal(CircuitState.Open, breaker.State);
    }

    [Fact]
    public void After_The_Break_The_Circuit_Reports_Half_Open_On_Its_Own()
    {
        // State is a function of the clock, not of anybody calling. A breaker that only
        // transitions inside Execute reports Open forever if nothing tries it, and every
        // dashboard built on State is wrong.
        var (breaker, clock, dependency) = Build();
        Trip(breaker, dependency);

        clock.Advance(Break);

        Assert.Equal(CircuitState.HalfOpen, breaker.State);
    }

    [Fact]
    public void A_Succeeding_Probe_Closes_The_Circuit()
    {
        var (breaker, clock, dependency) = Build();
        Trip(breaker, dependency);
        clock.Advance(Break);

        dependency.ShouldFail = false;

        Assert.Equal("ok", breaker.Execute(dependency.Call));
        Assert.Equal(CircuitState.Closed, breaker.State);
    }

    [Fact]
    public void Mechanism_A_Failing_Probe_Re_Opens_The_Circuit_Immediately()
    {
        // The half of the design people leave out. A breaker that simply closes after the
        // break duration sends the whole backlog at a service that may still be down -
        // the stampede it was installed to prevent, merely delayed. One that needs another
        // full threshold hammers a service that is down for an hour once per break
        // duration, in bursts, all hour.
        var (breaker, clock, dependency) = Build();
        Trip(breaker, dependency);
        clock.Advance(Break);

        Assert.Throws<InvalidOperationException>(() => breaker.Execute(dependency.Call));

        var callsAfterProbe = dependency.Calls;
        Assert.Equal(CircuitState.Open, breaker.State);
        Assert.Throws<CircuitOpenException>(() => breaker.Execute(dependency.Call));
        Assert.Equal(callsAfterProbe, dependency.Calls);
    }

    [Fact]
    public void Adversarial_A_Failed_Probe_Restarts_The_Whole_Break()
    {
        // Pairs with the fact above: re-opening must reset the timer too, or the next
        // call after a failed probe is immediately half-open again and the "break" is
        // nothing at all.
        var (breaker, clock, dependency) = Build();
        Trip(breaker, dependency);
        clock.Advance(Break);
        Assert.Throws<InvalidOperationException>(() => breaker.Execute(dependency.Call));

        clock.Advance(Break - TimeSpan.FromSeconds(1));
        Assert.Equal(CircuitState.Open, breaker.State);

        clock.Advance(TimeSpan.FromSeconds(1));
        Assert.Equal(CircuitState.HalfOpen, breaker.State);
    }
}
