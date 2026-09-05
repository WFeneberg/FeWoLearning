// Exercise 039 - Event Aggregator Unsubscribe (intermediate).
// Goal:   Learn the deterministic way to stop receiving messages: unsubscribe explicitly when a
//         screen deactivates, rather than relying on the aggregator ever noticing on its own -
//         and see why that matters: forgetting it does not leak the subscriber, but it does not
//         fail loudly either.
// Drills: subscribing in OnActivatedAsync and unsubscribing in OnDeactivateAsync, so a screen's
//         own activation lifecycle drives its EventAggregator membership automatically.
// Passes: dotnet test --filter FullyQualifiedName~Ex039_

using System.Threading;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Intermediate;

/// <summary>A plain message with no behaviour of its own - just something to publish.</summary>
public class Ex039_Ping;

/// <summary>A screen that subscribes to the aggregator only while it is active, and unsubscribes the instant it deactivates - the deterministic alternative to letting the aggregator's weak references do it eventually.</summary>
public class Ex039_SubscribingScreen : Screen, IHandle<Ex039_Ping>
{
    public Ex039_SubscribingScreen(IEventAggregator eventAggregator) => EventAggregator = eventAggregator;

    private IEventAggregator EventAggregator { get; }

    /// <summary>How many Ex039_Ping messages this screen has actually handled.</summary>
    public int ReceivedCount { get; private set; }

    protected override Task OnActivatedAsync(CancellationToken cancellationToken)
    {
        EventAggregator.Subscribe(this, deliver => deliver());
        return Task.CompletedTask;
    }

    protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
    {
        EventAggregator.Unsubscribe(this);
        return Task.CompletedTask;
    }

    public Task HandleAsync(Ex039_Ping message, CancellationToken cancellationToken)
    {
        ReceivedCount++;
        return Task.CompletedTask;
    }
}
