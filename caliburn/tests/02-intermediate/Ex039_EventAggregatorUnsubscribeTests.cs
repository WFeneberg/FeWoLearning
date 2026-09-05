using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Intermediate;

namespace FeWoLearning.Caliburn.Tests.Intermediate;

public class Ex039_EventAggregatorUnsubscribeTests : CaliburnCoreContext
{
    private static Task Activate(Ex039_SubscribingScreen vm) => ((IActivate)vm).ActivateAsync();

    private static Task Deactivate(Ex039_SubscribingScreen vm, bool close) => ((IDeactivate)vm).DeactivateAsync(close);

    [Fact]
    public async Task Activating_Subscribes_The_Screen_So_Published_Messages_Are_Delivered()
    {
        var aggregator = new EventAggregator();
        var screen = new Ex039_SubscribingScreen(aggregator);

        await Activate(screen);
        await aggregator.PublishAsync(new Ex039_Ping(), deliver => deliver());

        Assert.Equal(1, screen.ReceivedCount);
    }

    [Fact]
    public async Task Deactivating_Unsubscribes_HandlerExistsFor_Becomes_False()
    {
        var aggregator = new EventAggregator();
        var screen = new Ex039_SubscribingScreen(aggregator);
        await Activate(screen);

        await Deactivate(screen, close: false);

        Assert.False(aggregator.HandlerExistsFor(typeof(Ex039_Ping)));

        // Critical: screen is never read again after Deactivate above. Without this, a JIT that
        // treats its local as dead here could make it collectible before the assert runs - and
        // an implementation that never actually calls Unsubscribe would then pass for the WRONG
        // reason (the screen going weakly unreachable) instead of failing as it should.
        GC.KeepAlive(screen);
    }

    [Fact]
    public async Task After_Deactivating_Published_Messages_Are_No_Longer_Delivered()
    {
        var aggregator = new EventAggregator();
        var screen = new Ex039_SubscribingScreen(aggregator);
        await Activate(screen);
        await Deactivate(screen, close: false);

        await aggregator.PublishAsync(new Ex039_Ping(), deliver => deliver());

        // A wrong implementation that never actually unsubscribes would let this keep counting.
        Assert.Equal(0, screen.ReceivedCount);
    }

    [Fact]
    public async Task Reactivating_After_A_Deactivate_Resubscribes_And_Delivery_Resumes()
    {
        var aggregator = new EventAggregator();
        var screen = new Ex039_SubscribingScreen(aggregator);
        await Activate(screen);
        await Deactivate(screen, close: false);

        await Activate(screen);
        await aggregator.PublishAsync(new Ex039_Ping(), deliver => deliver());

        Assert.Equal(1, screen.ReceivedCount);
    }

    [Fact]
    public async Task A_Subscriber_With_No_Strong_Reference_Silently_Stops_Being_Delivered_To_After_Gc()
    {
        // First, drive the exercise's own stub the normal way, so this test still fails red on
        // an untouched stub. screen is kept alive for the whole test via the explicit
        // GC.KeepAlive below - exactly as every EventAggregator test in this track must
        // (subscribers are held WEAKLY - never subscribe an object created inline in a call
        // argument).
        var trackedAggregator = new EventAggregator();
        var screen = new Ex039_SubscribingScreen(trackedAggregator);
        await Activate(screen);
        await trackedAggregator.PublishAsync(new Ex039_Ping(), deliver => deliver());
        Assert.Equal(1, screen.ReceivedCount);

        // Now the actual lesson: a raw subscriber with NO Unsubscribe wiring at all, scoped so
        // that no root outlives this call - the non-deterministic pattern this exercise argues
        // against. [MethodImpl(NoInlining)] is defensive insurance, not a measured requirement:
        // collection was 1000/1000 reliable across every configuration measured even without
        // it, since the subscriber's only root already lives inside this call's own frame.
        var throwawayAggregator = new EventAggregator();
        var throwawayRef = SubscribeThrowaway(throwawayAggregator);

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        // Nothing threw, and nothing was ever unsubscribed explicitly - but the aggregator has
        // already quietly forgotten it, because nothing else in the process still references it.
        // The WeakReference is the positive control: it distinguishes "subscribed, then
        // collected" from "SubscribeThrowaway silently failed to subscribe at all", which the
        // HandlerExistsFor assertion alone cannot tell apart.
        Assert.False(throwawayRef.IsAlive);
        Assert.False(throwawayAggregator.HandlerExistsFor(typeof(Ex039_Ping)));

        // screen (the tracked subscriber above) must survive the forced collections too - this
        // is the whole point of the track-wide "keep a strong reference for the whole test" rule.
        GC.KeepAlive(screen);

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        static WeakReference SubscribeThrowaway(IEventAggregator aggregator)
        {
            var subscriber = new Ex039_ThrowawaySubscriber();
            aggregator.Subscribe(subscriber, deliver => deliver());
            return new WeakReference(subscriber);
        }
    }
}

/// <summary>A handler that exists only to be subscribed and then abandoned - it proves the aggregator's weak references, not anything the learner writes.</summary>
file sealed class Ex039_ThrowawaySubscriber : IHandle<Ex039_Ping>
{
    public Task HandleAsync(Ex039_Ping message, CancellationToken cancellationToken) => Task.CompletedTask;
}
