namespace FeWoLearning.Architecture.Exercises.Desktop.Ex019;

public sealed record OrderPlaced(string OrderId);

public sealed record OrderCancelled(string OrderId);

// Exercise 019 — MessengerEventAggregator (reference solution).
public sealed class Messenger
{
    private readonly Dictionary<Type, List<object>> _subscribers = [];

    public IDisposable Subscribe<TMessage>(Action<TMessage> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        if (!_subscribers.TryGetValue(typeof(TMessage), out var list))
            _subscribers[typeof(TMessage)] = list = [];

        list.Add(handler);

        return new Token(() => list.Remove(handler));
    }

    public void Publish<TMessage>(TMessage message)
    {
        if (!_subscribers.TryGetValue(typeof(TMessage), out var list))
            return;

        // ToArray, and not `foreach (var h in list)`. A handler is allowed to
        // unsubscribe itself - "stop listening now that I have seen what I was waiting
        // for" is the commonest thing a subscriber does - and iterating the live list
        // throws InvalidOperationException the moment it happens. The snapshot also
        // fixes the other half: a handler that subscribes someone new does not deliver
        // the in-flight message to them.
        foreach (var handler in list.ToArray())
            ((Action<TMessage>)handler)(message);
    }

    public int SubscriberCount<TMessage>() =>
        _subscribers.TryGetValue(typeof(TMessage), out var list) ? list.Count : 0;

    /// <summary>Idempotent: disposing twice must not remove somebody else's subscription.</summary>
    private sealed class Token(Action unsubscribe) : IDisposable
    {
        private Action? _unsubscribe = unsubscribe;

        public void Dispose()
        {
            var action = _unsubscribe;
            _unsubscribe = null;
            action?.Invoke();
        }
    }
}
