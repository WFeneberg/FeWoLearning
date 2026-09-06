namespace FeWoLearning.Architecture.Exercises.Runtime.Ex099;

public sealed record Deposit(decimal Amount);

public sealed record Withdraw(decimal Amount);

public sealed record ReadBalance(TaskCompletionSource<decimal> Reply);

// Exercise 099 — ActorSingleThreadedOwnership (runtime).
// Goal:   Protect mutable state with a mailbox instead of a lock, and get message ordering
//         for free.
// Drills: single-threaded ownership, mailbox semantics, no shared state, back-pressure.
// Passes: ordering  - messages are processed in the order they were sent, one at a time.
//         safety    - many senders posting concurrently produce the right final balance,
//                     with no lock anywhere in the actor.
//         THE ONE    - the actor's state is NEVER touched from outside. Reading the balance
//                      is a MESSAGE, not a property. A property that reads the field
//                      directly is a data race the mailbox does nothing about - and it will
//                      look correct in every test that does not run concurrently.
//         rules     - a withdrawal beyond the balance is refused, and the refusal is
//                      observed by the state not changing rather than by an exception
//                      crossing a thread boundary.
//         draining  - after the mailbox is drained, everything sent has been applied.
//
// The trade against a lock is worth stating plainly. A lock lets any thread touch the state
// as long as it asks first, which means correctness depends on every caller remembering -
// including the one added next year. An actor makes it structurally impossible: there is
// one thread, it owns the field, and the only way in is the queue.
//
// The cost is that everything becomes asynchronous, including reads. `actor.Balance` cannot
// exist, so a read is a message with a reply - which is more ceremony, and is also the
// point: it makes visible that the answer is a snapshot from a moment that has already
// passed.
public sealed class AccountActor
{
    private decimal _balance;
    private readonly Queue<object> _mailbox = new();

    /// <summary>Every message ever applied, in order. For the ordering fact.</summary>
    public List<string> Applied { get; } = [];

    /// <summary>Post a message. Returns immediately; nothing is applied yet.</summary>
    public void Send(object message) =>
        throw new NotImplementedException("TODO: Ex099 - enqueue the message");

    /// <summary>Process everything in the mailbox, one message at a time, in order.</summary>
    public void Drain() =>
        throw new NotImplementedException(
            "TODO: Ex099 - dequeue and apply each message: deposits add, valid withdrawals subtract, ReadBalance replies, and record each in Applied");
}
