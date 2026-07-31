using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FeWoLearning.Exercises.Advanced;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Advanced;

public class Ex074_SemaphoreThrottleTests
{
    // Deterministic concurrency proof: no Task.Delay / wall-clock timing anywhere.
    // The first `limit` operations to arrive rendezvous on a CountdownEvent-backed
    // gate, so they are provably running at the same time before any of them is
    // allowed to finish. Every arrival (first batch or later) records the observed
    // concurrency via Interlocked so a violation of the cap fails immediately.
    [Fact]
    public async Task NeverExceedsConfiguredConcurrency()
    {
        const int limit = 3;
        const int operationCount = 9;

        using var throttle = new SemaphoreThrottle(limit);

        var currentConcurrency = 0;
        var maxObservedConcurrency = 0;
        var arrivalIndex = 0;
        var rendezvousCountdown = new CountdownEvent(limit);
        var rendezvousGate = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var violation = 0;

        async Task Operation()
        {
            var running = Interlocked.Increment(ref currentConcurrency);

            int observed;
            do
            {
                observed = maxObservedConcurrency;
                if (running <= observed)
                    break;
            } while (Interlocked.CompareExchange(ref maxObservedConcurrency, running, observed) != observed);

            if (running > limit)
                Interlocked.Increment(ref violation);

            // Only the first `limit` arrivals participate in the rendezvous: they
            // must all be "in flight" simultaneously before any is released, which
            // proves the throttle actually reaches (and does not exceed) `limit`
            // concurrent operations.
            if (Interlocked.Increment(ref arrivalIndex) <= limit)
            {
                if (rendezvousCountdown.Signal())
                    rendezvousGate.TrySetResult(true);

                await rendezvousGate.Task.ConfigureAwait(false);
            }

            Interlocked.Decrement(ref currentConcurrency);
        }

        var tasks = Enumerable.Range(0, operationCount)
            .Select(_ => throttle.RunAsync(Operation))
            .ToArray();

        await Task.WhenAll(tasks);

        Assert.Equal(0, violation);
        Assert.Equal(limit, maxObservedConcurrency);
    }

    [Fact]
    public async Task ReturnsResultsFromEachOperation()
    {
        using var throttle = new SemaphoreThrottle(2);

        var results = await Task.WhenAll(
            Enumerable.Range(0, 5).Select(i => throttle.RunAsync(() => Task.FromResult(i * i))));

        Assert.Equal(new[] { 0, 1, 4, 9, 16 }, results);
    }

    [Fact]
    public async Task ReleasesPermitEvenWhenOperationThrows()
    {
        using var throttle = new SemaphoreThrottle(1);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => throttle.RunAsync(() => throw new InvalidOperationException("boom")));

        // If the permit were leaked by the failing call above, this would hang
        // forever instead of completing.
        var completed = await Task.WhenAny(
            throttle.RunAsync(() => Task.FromResult(true)),
            Task.Delay(TimeSpan.FromSeconds(5)).ContinueWith(_ => false));

        Assert.True(completed is Task<bool> { Result: true });
    }

    [Fact]
    public void RejectsNonPositiveMaxConcurrency()
        => Assert.Throws<ArgumentOutOfRangeException>(() => new SemaphoreThrottle(0));

    [Fact]
    public void ExposesConfiguredMaxConcurrency()
    {
        using var throttle = new SemaphoreThrottle(4);
        Assert.Equal(4, throttle.MaxConcurrency);
    }
}
