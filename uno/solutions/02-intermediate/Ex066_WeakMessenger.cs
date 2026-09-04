// Exercise 066 - Weak Messenger (intermediate).
// Goal:   Let view models talk to each other without the bus keeping them alive.
// Drills: WeakReference<T> recipients, a handler signature that cannot capture the
//         recipient, and pruning dead entries on publish.
// Passes: dotnet test --filter FullyQualifiedName~Ex066_
//
// A messenger is usually a singleton, and a singleton that holds its recipients strongly
// holds every page the user ever visited. The fix is not "remember to unsubscribe" -
// somebody will forget - it is for the bus to hold recipients weakly.
//
// Note the handler shape: Action<TRecipient, TMessage>, with the recipient handed back as
// an argument. A plain Action<TMessage> would close over the recipient, the bus would hold
// that closure strongly, and the weak reference next to it would never die. That is why
// every messenger worth using - the CommunityToolkit one included - has this signature.

namespace FeWoLearning.Uno.Exercises.Intermediate;

/// <summary>
/// A publish/subscribe bus keyed by message type, holding its recipients weakly.
/// </summary>
public sealed class Ex066_WeakMessenger
{
    private readonly List<(Type MessageType, WeakReference<object> Recipient, Action<object, object> Handler)> _subscribers = [];

    /// <summary>How many subscriptions the bus still holds, dead ones included.</summary>
    public int SubscriptionCount => _subscribers.Count;

    /// <summary>
    /// Subscribes <paramref name="recipient"/> to messages of type
    /// <typeparamref name="TMessage"/>, for as long as it is alive.
    /// </summary>
    public void Subscribe<TRecipient, TMessage>(TRecipient recipient, Action<TRecipient, TMessage> handler)
        where TRecipient : class =>
        _subscribers.Add((
            typeof(TMessage),
            new WeakReference<object>(recipient),

            // Captures `handler` and nothing else. Capturing `recipient` here would make
            // the bus hold it strongly, and the WeakReference beside it would never die -
            // which is the whole failure this signature exists to prevent.
            (recipientArgument, message) => handler((TRecipient)recipientArgument, (TMessage)message)));

    /// <summary>
    /// Delivers <paramref name="message"/> to every live subscriber of its type, and
    /// forgets any subscription whose recipient has been collected.
    /// </summary>
    /// <returns>How many handlers were called.</returns>
    public int Publish<TMessage>(TMessage message)
        where TMessage : notnull
    {
        var delivered = 0;

        // Backwards, so removing a dead entry does not shift the indices still to visit.
        for (var i = _subscribers.Count - 1; i >= 0; i--)
        {
            var (messageType, recipient, handler) = _subscribers[i];

            if (!recipient.TryGetTarget(out var target))
            {
                // Pruned here rather than in a timer or a finaliser: a publish is the one
                // moment the bus is already walking the list.
                _subscribers.RemoveAt(i);
                continue;
            }

            if (messageType != typeof(TMessage))
            {
                continue;
            }

            handler(target, message);
            delivered++;
        }

        return delivered;
    }

    /// <summary>Drops every subscription belonging to <paramref name="recipient"/>.</summary>
    public void Unsubscribe(object recipient) =>
        _subscribers.RemoveAll(entry =>
            !entry.Recipient.TryGetTarget(out var target) || ReferenceEquals(target, recipient));
}
