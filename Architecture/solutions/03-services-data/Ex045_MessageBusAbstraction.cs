namespace FeWoLearning.Architecture.Exercises.ServicesData.Ex045;

// Exercise 045 — MessageBusAbstraction (reference solution).
public sealed class TopicBus
{
    private readonly List<(string Pattern, Action<string, string> Handler)> _subscriptions = [];

    public IDisposable Subscribe(string pattern, Action<string, string> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        var subscription = (pattern, handler);
        _subscriptions.Add(subscription);
        return new Token(() => _subscriptions.Remove(subscription));
    }

    public void Publish(string topic, string payload)
    {
        // Snapshot before dispatching, for the same reason as exercise 019: a handler is
        // allowed to unsubscribe itself while it runs.
        foreach (var (pattern, handler) in _subscriptions.ToArray())
            if (Matches(pattern, topic))
                handler(topic, payload);
    }

    public static bool Matches(string pattern, string topic)
    {
        var patternSegments = pattern.Split('.');
        var topicSegments = topic.Split('.');

        for (var i = 0; i < patternSegments.Length; i++)
        {
            // ">" swallows everything from here on - but only if there is at least one
            // segment left to swallow.
            if (patternSegments[i] == ">")
                return topicSegments.Length > i;

            if (i >= topicSegments.Length)
                return false;

            // "*" matches exactly one segment. Segment-by-segment comparison is what
            // makes that true; a prefix test ("does the topic start with orders.")
            // accepts "orders.created" and also every sub-topic anybody ever adds below
            // it, and the subscriber that asked for one level starts receiving a
            // firehose without a line of its code changing.
            if (patternSegments[i] != "*" && patternSegments[i] != topicSegments[i])
                return false;
        }

        // Every pattern segment matched, so the topic must have run out at the same time.
        return patternSegments.Length == topicSegments.Length;
    }

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
