namespace FeWoLearning.Architecture.Exercises.ServicesData.Ex033;

/// <summary>
/// MessageId is assigned by the PRODUCER and travels with the message. It is not a hash
/// of the content, and that distinction is the exercise.
/// </summary>
public sealed record Payment(string MessageId, string AccountId, decimal Amount);

/// <summary>Counts how often the side effect actually happened.</summary>
public sealed class Ledger
{
    private readonly Dictionary<string, decimal> _balances = [];

    public int Applications { get; private set; }

    public decimal BalanceOf(string accountId) => _balances.GetValueOrDefault(accountId);

    public void Credit(string accountId, decimal amount)
    {
        Applications++;
        _balances[accountId] = BalanceOf(accountId) + amount;
    }
}

// Exercise 033 — IdempotentConsumer (services-data).
// Goal:   Survive at-least-once delivery: the message may arrive twice, the money may
//         move only once.
// Drills: inbox dedup, at-least-once delivery, duplicate suppression.
// Passes: first delivery  - the credit is applied and Handle returns true.
//         redelivery      - the SAME MessageId does nothing: the balance is unchanged,
//                           Ledger.Applications is unchanged, Handle returns false, and
//                           nothing is thrown - the message must still be acknowledged,
//                           or the broker will keep sending it forever.
//         THE ONE          - two DIFFERENT messages that happen to be identical in every
//                           other field are both applied.
//         separate accounts do not interfere.
//
// The last-but-one clause is what separates an inbox from a content hash. Deduplicating
// on "same account, same amount" is an ordinary-looking implementation that passes every
// redelivery assertion - and silently swallows the second of two genuine, identical
// payments. A customer paying the same subscription twice in a month is not a duplicate,
// and the system has no way to know that unless the producer says so.
public sealed class IdempotentConsumer(Ledger ledger)
{
    /// <summary>Returns whether this delivery actually applied the payment.</summary>
    public bool Handle(Payment payment) =>
        throw new NotImplementedException(
            "TODO: Ex033 - apply the credit only for a MessageId this consumer has not seen, and acknowledge either way");
}
