// Exercise 037 - Event Aggregator Basics (intermediate).
// Goal:   Learn the minimum EventAggregator round trip: Subscribe registers an object, and every
//         IHandle<T> it implements starts receiving messages of that type the moment something
//         calls PublishAsync - HandleAsync's own signature carries a CancellationToken too, not
//         just the message.
// Drills: IEventAggregator.Subscribe(object, marshal) and PublishAsync(object, marshal, token) -
//         the four-method instance surface, used directly rather than through one of its
//         SubscribeOnXxxThread/PublishOnXxxThreadAsync extension-method shortcuts.
// Passes: dotnet test --filter FullyQualifiedName~Ex037_
//
// Measured on this machine (Caliburn.Micro 5.0.258): IEventAggregator has exactly four instance
// members - Subscribe, PublishAsync, Unsubscribe, HandlerExistsFor - everything else
// (SubscribeOnUIThread, PublishOnBackgroundThreadAsync, and friends) is an extension method
// layered on top of those four. IHandle<T>'s single method is HandleAsync(T message,
// CancellationToken cancellationToken) - PublishAsync's own CancellationToken parameter is what
// reaches it.

using System.Threading;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Intermediate;

public class Ex037_EventAggregatorBasics
{
    /// <summary>Subscribes subscriber to the aggregator so every IHandle&lt;T&gt; it implements starts receiving.</summary>
    public void Subscribe(IEventAggregator aggregator, object subscriber) =>
        throw new NotImplementedException("TODO: Ex037 - subscribe via aggregator.Subscribe");

    /// <summary>Publishes message through the aggregator, forwarding cancellationToken all the way to any HandleAsync it reaches.</summary>
    public Task PublishAsync(IEventAggregator aggregator, object message, CancellationToken cancellationToken = default) =>
        throw new NotImplementedException("TODO: Ex037 - publish via aggregator.PublishAsync, forwarding cancellationToken");
}

/// <summary>A plain message with no behaviour of its own - just something to publish.</summary>
public class Ex037_PingMessage
{
    public string? Text { get; init; }
}

/// <summary>Handles Ex037_PingMessage and records what it received, including the cancellation token.</summary>
public class Ex037_Subscriber : IHandle<Ex037_PingMessage>
{
    public int ReceivedCount { get; private set; }

    public Ex037_PingMessage? LastMessage { get; private set; }

    public CancellationToken LastCancellationToken { get; private set; }

    public Task HandleAsync(Ex037_PingMessage message, CancellationToken cancellationToken)
    {
        ReceivedCount++;
        LastMessage = message;
        LastCancellationToken = cancellationToken;
        return Task.CompletedTask;
    }
}
