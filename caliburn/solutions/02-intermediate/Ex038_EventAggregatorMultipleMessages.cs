// Exercise 038 - Event Aggregator Multiple Messages (intermediate).
// Goal:   See that Subscribe is per OBJECT, not per message type: one call registers every
//         IHandle<T> interface the subscriber implements at once, and each keeps receiving
//         independently of how many times the others are published.
// Drills: subscribing an object that implements two different IHandle<T> interfaces through a
//         single Subscribe call, and confirming both types are covered via HandlerExistsFor.
// Passes: dotnet test --filter FullyQualifiedName~Ex038_

using System.Threading;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Intermediate;

public class Ex038_EventAggregatorMultipleMessages
{
    /// <summary>Subscribes subscriber to the aggregator - one call, covering every IHandle&lt;T&gt; interface it implements.</summary>
    public void SubscribeToAll(IEventAggregator aggregator, object subscriber) =>
        aggregator.Subscribe(subscriber, deliver => deliver());
}

public class Ex038_Ping;

public class Ex038_Pong;

/// <summary>Handles both Ex038_Ping and Ex038_Pong independently.</summary>
public class Ex038_MultiHandler : IHandle<Ex038_Ping>, IHandle<Ex038_Pong>
{
    public int PingCount { get; private set; }

    public int PongCount { get; private set; }

    public Task HandleAsync(Ex038_Ping message, CancellationToken cancellationToken)
    {
        PingCount++;
        return Task.CompletedTask;
    }

    public Task HandleAsync(Ex038_Pong message, CancellationToken cancellationToken)
    {
        PongCount++;
        return Task.CompletedTask;
    }
}

/// <summary>Handles ONLY Ex038_Ping - a control used to prove per-type delivery stays independent.</summary>
public class Ex038_PingOnlyHandler : IHandle<Ex038_Ping>
{
    public int PingCount { get; private set; }

    public Task HandleAsync(Ex038_Ping message, CancellationToken cancellationToken)
    {
        PingCount++;
        return Task.CompletedTask;
    }
}
