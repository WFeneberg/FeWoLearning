using System;
using FeWoLearning.Exercises.Advanced;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Advanced;

public class Ex090_CircuitBreakerTests
{
    [Fact]
    public void TripsOpen_ThenHalfOpensAfterTimeout_ThenClosesOnSuccessfulTrial()
    {
        var now = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var breaker = new CircuitBreaker(failureThreshold: 3, resetTimeout: TimeSpan.FromSeconds(30), clock: () => now);

        var callCount = 0;
        var shouldFail = true;
        int Flaky()
        {
            callCount++;
            if (shouldFail) throw new InvalidOperationException("boom");
            return 42;
        }

        Assert.Equal(CircuitState.Closed, breaker.State);

        // Two failures: below the threshold, breaker stays Closed.
        Assert.Throws<InvalidOperationException>(() => breaker.Execute(Flaky));
        Assert.Throws<InvalidOperationException>(() => breaker.Execute(Flaky));
        Assert.Equal(CircuitState.Closed, breaker.State);
        Assert.Equal(2, callCount);

        // Third consecutive failure trips the breaker.
        Assert.Throws<InvalidOperationException>(() => breaker.Execute(Flaky));
        Assert.Equal(CircuitState.Open, breaker.State);
        Assert.Equal(3, callCount);

        // While Open and before the timeout, calls are rejected outright and the
        // wrapped action is never invoked.
        Assert.Throws<CircuitBreakerOpenException>(() => breaker.Execute(Flaky));
        Assert.Equal(3, callCount);

        // Advance the injected clock past the reset timeout: the breaker now
        // reports HalfOpen without needing a call to be attempted.
        now = now.AddSeconds(31);
        Assert.Equal(CircuitState.HalfOpen, breaker.State);

        // The trial call succeeds, so the breaker closes and resets its failure count.
        shouldFail = false;
        var result = breaker.Execute(Flaky);
        Assert.Equal(42, result);
        Assert.Equal(4, callCount);
        Assert.Equal(CircuitState.Closed, breaker.State);
    }

    [Fact]
    public void FailedTrialCallWhileHalfOpen_ReopensImmediately()
    {
        var now = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var breaker = new CircuitBreaker(failureThreshold: 2, resetTimeout: TimeSpan.FromSeconds(10), clock: () => now);

        void AlwaysFail() => throw new InvalidOperationException("boom");
        Func<int> Fail = () => { AlwaysFail(); return 0; };

        Assert.Throws<InvalidOperationException>(() => breaker.Execute(Fail));
        Assert.Throws<InvalidOperationException>(() => breaker.Execute(Fail));
        Assert.Equal(CircuitState.Open, breaker.State);

        now = now.AddSeconds(11);
        Assert.Equal(CircuitState.HalfOpen, breaker.State);

        // The half-open trial call fails too, so the breaker reopens immediately
        // (it does not require another full threshold's worth of failures).
        Assert.Throws<InvalidOperationException>(() => breaker.Execute(Fail));
        Assert.Equal(CircuitState.Open, breaker.State);

        // Still within the (fresh) timeout window, so calls are rejected again.
        Assert.Throws<CircuitBreakerOpenException>(() => breaker.Execute(Fail));
    }

    [Fact]
    public void RejectsNonPositiveFailureThreshold()
        => Assert.Throws<ArgumentOutOfRangeException>(
            () => new CircuitBreaker(0, TimeSpan.FromSeconds(1), () => DateTime.UtcNow));
}
