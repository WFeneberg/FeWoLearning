namespace FeWoLearning.Exercises.Expert;

// Exercise 099 — Event-sourced aggregate (expert).
// Goal:   Model a bank-account aggregate that rebuilds its state by replaying
//         a stream of domain events, and that turns commands into new events
//         only when the command is valid against the current state.
// Drills: event sourcing, aggregate replay, command validation, immutable events.

public abstract record AccountEvent;

public sealed record AccountOpened(string Owner, decimal InitialBalance) : AccountEvent;

public sealed record MoneyDeposited(decimal Amount) : AccountEvent;

public sealed record MoneyWithdrawn(decimal Amount) : AccountEvent;

public sealed class EventSourcedAggregate
{
    public string Owner { get; private set; } = string.Empty;

    public decimal Balance { get; private set; }

    public int Version { get; private set; }

    public IReadOnlyList<AccountEvent> PendingEvents => throw new NotImplementedException();

    /// <summary>Rebuilds an aggregate purely by replaying a previously stored event stream.</summary>
    public static EventSourcedAggregate LoadFromHistory(IEnumerable<AccountEvent> history) => throw new NotImplementedException();

    /// <summary>Mutates state from a single event without recording it as pending (used for replay).</summary>
    public void Apply(AccountEvent @event) => throw new NotImplementedException();

    /// <summary>Command: open the account. Invalid (already open) yields no new events.</summary>
    public IReadOnlyList<AccountEvent> Open(string owner, decimal initialBalance) => throw new NotImplementedException();

    /// <summary>Command: deposit funds. Invalid (non-positive amount) yields no new events.</summary>
    public IReadOnlyList<AccountEvent> Deposit(decimal amount) => throw new NotImplementedException();

    /// <summary>Command: withdraw funds. Invalid (non-positive or insufficient balance) yields no new events.</summary>
    public IReadOnlyList<AccountEvent> Withdraw(decimal amount) => throw new NotImplementedException();
}
