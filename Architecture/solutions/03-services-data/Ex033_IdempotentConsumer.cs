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

// Exercise 033 — IdempotentConsumer (reference solution).
public sealed class IdempotentConsumer(Ledger ledger)
{
    // The inbox. Keyed by the producer's MessageId - NOT by a hash of the content.
    // Deduplicating on "same account, same amount" passes every redelivery assertion and
    // swallows the second of two genuine identical payments; a customer paying the same
    // subscription twice in a month is not a duplicate.
    private readonly HashSet<string> _processed = new(StringComparer.Ordinal);

    public bool Handle(Payment payment)
    {
        ArgumentNullException.ThrowIfNull(payment);

        if (!_processed.Add(payment.MessageId))
            return false; // seen before: do nothing, and do not throw - the broker still
                          // needs its acknowledgement, or it will redeliver forever

        ledger.Credit(payment.AccountId, payment.Amount);
        return true;
    }
}
