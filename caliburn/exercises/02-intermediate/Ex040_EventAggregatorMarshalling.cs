// Exercise 040 - Event Aggregator Marshalling (intermediate).
// Goal:   Learn what PublishAsync's second parameter actually is: not a threading option picked
//         from a menu, but a delegate the CALLER controls, which wraps the entire delivery -
//         every handler for that publish runs strictly inside whatever the marshal decides to
//         do, and the marshal itself runs exactly once per PublishAsync call, no matter how many
//         handlers exist.
// Drills: writing a PublishAsync wrapper that passes the caller-supplied marshal straight
//         through instead of hard-coding its own pass-through, so a marshal that refuses to run
//         its argument, or one that counts its own invocations, actually has the effect it should.
// Passes: dotnet test --filter FullyQualifiedName~Ex040_
//
// Measured on this machine (Caliburn.Micro 5.0.258): PublishAsync's marshal is invoked exactly
// once per PublishAsync call regardless of how many subscribers or IHandle<T> implementations
// end up delivered to inside it - and it is invoked even when nobody is subscribed at all, since
// the aggregator has no way of knowing that in advance without running the marshal.

using System.Threading;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Intermediate;

public class Ex040_EventAggregatorMarshalling
{
    /// <summary>Publishes message through the aggregator using the CALLER's marshal, not one of its own choosing.</summary>
    public Task PublishWithMarshalAsync(IEventAggregator aggregator, object message, Func<Func<Task>, Task> marshal) =>
        throw new NotImplementedException("TODO: Ex040 - publish via aggregator.PublishAsync, passing marshal straight through");
}

public class Ex040_Ping;

/// <summary>Handles Ex040_Ping and counts how many it actually received.</summary>
public class Ex040_PingHandler : IHandle<Ex040_Ping>
{
    public int ReceivedCount { get; private set; }

    public Task HandleAsync(Ex040_Ping message, CancellationToken cancellationToken)
    {
        ReceivedCount++;
        return Task.CompletedTask;
    }
}

/// <summary>A marshal that counts its own invocations, and can optionally refuse to run the delivery it was handed.</summary>
public class Ex040_CountingMarshal
{
    public int InvokeCount { get; private set; }

    /// <summary>When true, MarshalAsync counts itself but never actually calls deliver.</summary>
    public bool SuppressDelivery { get; set; }

    public Task MarshalAsync(Func<Task> deliver)
    {
        InvokeCount++;
        return SuppressDelivery ? Task.CompletedTask : deliver();
    }
}
