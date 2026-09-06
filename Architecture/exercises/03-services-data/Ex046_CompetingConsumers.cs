namespace FeWoLearning.Architecture.Exercises.ServicesData.Ex046;

public sealed record Message(string Key, string Payload);

/// <summary>One worker. Records what it handled, in the order it handled it.</summary>
public sealed class Consumer(string name)
{
    public string Name => name;

    public List<Message> Handled { get; } = [];

    public void Handle(Message message) => Handled.Add(message);
}

// Exercise 046 — CompetingConsumers (services-data).
// Goal:   Spread work across several workers without losing the ordering that some of
//         it depends on.
// Drills: work distribution, partition keys, per-key ordering.
// Passes: exactly once - every message is handled by exactly ONE consumer; the counts add
//                        up to the number of messages.
//         THE ONE       - every message with the same KEY goes to the SAME consumer, and
//                        arrives there in publish order.
//         spread        - different keys are distributed across the consumers rather than
//                        all landing on one.
//         stability     - the same key always maps to the same consumer, across calls.
//
// Round-robin is the obvious way to spread work evenly, and it is what makes this
// exercise worth doing: it satisfies exactly-once and spreads perfectly, and it sends
// "order 7 created" to worker 1 and "order 7 cancelled" to worker 2, which then race.
// Most messages do not care. The ones that do care never announce themselves, and the
// bug appears as a cancelled order that is somehow still shipping.
//
// The price is written on the tin: a partition can only be consumed by one worker, so
// the number of partitions is a ceiling on parallelism, and one hot key is a hot worker.
public static class Ex046_CompetingConsumers
{
    /// <summary>
    /// Which consumer handles <paramref name="key"/>, given
    /// <paramref name="consumerCount"/> of them. Must be stable: the same key and count
    /// always give the same index.
    /// </summary>
    public static int PartitionOf(string key, int consumerCount) =>
        throw new NotImplementedException(
            "TODO: Ex046 - map the key to a consumer index deterministically, so the same key always lands in the same place");

    /// <summary>Hand every message to exactly one consumer, keeping same-key order.</summary>
    public static void Dispatch(IReadOnlyList<Message> messages, IReadOnlyList<Consumer> consumers) =>
        throw new NotImplementedException(
            "TODO: Ex046 - send each message to consumers[PartitionOf(key, consumers.Count)], in order");
}
