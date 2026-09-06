namespace FeWoLearning.Architecture.Exercises.ServicesData.Ex046;

public sealed record Message(string Key, string Payload);

/// <summary>One worker. Records what it handled, in the order it handled it.</summary>
public sealed class Consumer(string name)
{
    public string Name => name;

    public List<Message> Handled { get; } = [];

    public void Handle(Message message) => Handled.Add(message);
}

// Exercise 046 — CompetingConsumers (reference solution).
public static class Ex046_CompetingConsumers
{
    public static int PartitionOf(string key, int consumerCount)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(consumerCount, 1);

        // An explicit, stable hash - NOT string.GetHashCode(), which is randomised per
        // process in .NET. With that, the same key lands on a different worker after
        // every restart, and the ordering guarantee this whole exercise is about holds
        // only until somebody deploys.
        var hash = 17u;
        foreach (var c in key)
            hash = (hash * 31) + c;

        return (int)(hash % (uint)consumerCount);
    }

    public static void Dispatch(IReadOnlyList<Message> messages, IReadOnlyList<Consumer> consumers)
    {
        ArgumentNullException.ThrowIfNull(messages);
        ArgumentOutOfRangeException.ThrowIfLessThan(consumers.Count, 1);

        // In order, one consumer each. Round-robin here would spread the load perfectly,
        // satisfy exactly-once, and send "order 7 created" to one worker and "order 7
        // cancelled" to another, where they race.
        foreach (var message in messages)
            consumers[PartitionOf(message.Key, consumers.Count)].Handle(message);
    }
}
