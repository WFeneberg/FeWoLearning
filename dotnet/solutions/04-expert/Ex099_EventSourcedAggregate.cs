namespace FeWoLearning.Exercises.Expert;

// Exercise 099 — Event-sourced aggregate (reference solution).
// State is derived exclusively from applying events; commands are pure
// decisions that either raise new events or produce nothing when invalid.

public abstract record AccountEvent;

public sealed record AccountOpened(string Owner, decimal InitialBalance) : AccountEvent;

public sealed record MoneyDeposited(decimal Amount) : AccountEvent;

public sealed record MoneyWithdrawn(decimal Amount) : AccountEvent;

public sealed class EventSourcedAggregate
{
    private readonly List<AccountEvent> _pending = new();

    public string Owner { get; private set; } = string.Empty;

    public decimal Balance { get; private set; }

    public int Version { get; private set; }

    public IReadOnlyList<AccountEvent> PendingEvents => _pending;

    public static EventSourcedAggregate LoadFromHistory(IEnumerable<AccountEvent> history)
    {
        var aggregate = new EventSourcedAggregate();
        foreach (var @event in history)
            aggregate.Apply(@event);
        return aggregate;
    }

    public void Apply(AccountEvent @event)
    {
        switch (@event)
        {
            case AccountOpened opened:
                Owner = opened.Owner;
                Balance = opened.InitialBalance;
                break;
            case MoneyDeposited deposited:
                Balance += deposited.Amount;
                break;
            case MoneyWithdrawn withdrawn:
                Balance -= withdrawn.Amount;
                break;
            default:
                throw new InvalidOperationException($"Unknown event type '{@event.GetType().Name}'.");
        }
        Version++;
    }

    public IReadOnlyList<AccountEvent> Open(string owner, decimal initialBalance)
    {
        if (Version != 0 || string.IsNullOrWhiteSpace(owner) || initialBalance < 0)
            return Array.Empty<AccountEvent>();

        return Raise(new AccountOpened(owner, initialBalance));
    }

    public IReadOnlyList<AccountEvent> Deposit(decimal amount)
    {
        if (Version == 0 || amount <= 0)
            return Array.Empty<AccountEvent>();

        return Raise(new MoneyDeposited(amount));
    }

    public IReadOnlyList<AccountEvent> Withdraw(decimal amount)
    {
        if (Version == 0 || amount <= 0 || amount > Balance)
            return Array.Empty<AccountEvent>();

        return Raise(new MoneyWithdrawn(amount));
    }

    private IReadOnlyList<AccountEvent> Raise(AccountEvent @event)
    {
        Apply(@event);
        _pending.Add(@event);
        return new[] { @event };
    }
}
