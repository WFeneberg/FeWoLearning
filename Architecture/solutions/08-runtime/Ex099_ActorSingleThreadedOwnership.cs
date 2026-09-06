namespace FeWoLearning.Architecture.Exercises.Runtime.Ex099;

public sealed record Deposit(decimal Amount);

public sealed record Withdraw(decimal Amount);

public sealed record ReadBalance(TaskCompletionSource<decimal> Reply);

// Exercise 099 — ActorSingleThreadedOwnership (reference solution).
public sealed class AccountActor
{
    private decimal _balance;
    private readonly Queue<object> _mailbox = new();
    private readonly Lock _mailboxGate = new();

    public List<string> Applied { get; } = [];

    public void Send(object message)
    {
        // The MAILBOX is the only shared thing, and it is the only thing guarded. The
        // balance is not - because nothing outside Drain ever reaches it.
        lock (_mailboxGate)
            _mailbox.Enqueue(message);
    }

    public void Drain()
    {
        while (true)
        {
            object message;

            lock (_mailboxGate)
            {
                if (_mailbox.Count == 0)
                    return;

                message = _mailbox.Dequeue();
            }

            // Applied outside the mailbox lock, one at a time, on this thread. The state
            // needs no protection at all: correctness comes from there being exactly one
            // way in, rather than from every caller remembering to ask first - including
            // the caller somebody adds next year.
            switch (message)
            {
                case Deposit deposit:
                    _balance += deposit.Amount;
                    Applied.Add($"deposit:{deposit.Amount}");
                    break;

                case Withdraw withdraw when withdraw.Amount <= _balance:
                    _balance -= withdraw.Amount;
                    Applied.Add($"withdraw:{withdraw.Amount}");
                    break;

                case Withdraw withdraw:
                    // Refused by not happening. Throwing here would surface on the drain
                    // thread, which is nobody's caller.
                    Applied.Add($"refused:{withdraw.Amount}");
                    break;

                case ReadBalance read:
                    // A read is a MESSAGE. A property reading the field directly is a data
                    // race the mailbox does nothing about, and it looks correct in every
                    // test that does not run concurrently.
                    read.Reply.TrySetResult(_balance);
                    Applied.Add("read");
                    break;
            }
        }
    }
}
