namespace FeWoLearning.Architecture.Exercises.Desktop.Ex019;

public sealed record OrderPlaced(string OrderId);

public sealed record OrderCancelled(string OrderId);

// Exercise 019 — MessengerEventAggregator (desktop).
// Goal:   In-process publish/subscribe where unsubscribing actually works - including
//         when it happens from inside a handler that is being invoked right now.
// Drills: in-process pub/sub, subscription lifetime, re-entrancy, snapshot semantics.
// Passes: routing      - a subscriber receives messages of its own type and no others.
//         unsubscribe  - disposing the token stops delivery, and disposing twice is
//                        harmless.
//         re-entrancy  - a handler that disposes its OWN token while being invoked does
//                        not break the publish in progress, and is not called again.
//         snapshot     - a handler that subscribes a NEW handler while being invoked
//                        does not deliver the in-flight message to it.
//
// The last two are the whole exercise. The naive implementation keeps a List and
// iterates it directly, which throws InvalidOperationException the moment a handler
// unsubscribes itself - and "unsubscribe myself once I have seen the message I was
// waiting for" is the single most common thing a subscriber does. Take a snapshot
// before dispatching, and the re-entrancy question answers itself.
//
// Deliberately out of scope: weak subscriptions. They are the other classic messenger
// topic, and grading them needs GC.Collect to actually collect, which makes a test that
// fails for reasons unrelated to the learner's code. Lifetime here is explicit, through
// the returned token.
public sealed class Messenger
{
    /// <summary>
    /// Register <paramref name="handler"/> for messages of type TMessage. Dispose the
    /// returned token to stop receiving them.
    /// </summary>
    public IDisposable Subscribe<TMessage>(Action<TMessage> handler) =>
        throw new NotImplementedException(
            "TODO: Ex019 - register the handler under typeof(TMessage) and return a token that removes it");

    /// <summary>Deliver <paramref name="message"/> to every current subscriber of its type.</summary>
    public void Publish<TMessage>(TMessage message) =>
        throw new NotImplementedException(
            "TODO: Ex019 - dispatch to a SNAPSHOT of the current subscribers, so a handler may subscribe or unsubscribe while running");

    /// <summary>How many subscribers are currently registered for TMessage.</summary>
    public int SubscriberCount<TMessage>() =>
        throw new NotImplementedException("TODO: Ex019 - the number of live subscriptions for this message type");
}
