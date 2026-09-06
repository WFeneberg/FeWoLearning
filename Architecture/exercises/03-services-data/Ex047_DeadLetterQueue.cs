namespace FeWoLearning.Architecture.Exercises.ServicesData.Ex047;

/// <summary>
/// A message that gave up, with everything needed to work out why. The payload is here
/// on purpose: a dead letter without its body is a log line, and nobody can replay a
/// log line.
/// </summary>
public sealed record DeadLetter(string MessageId, string Payload, int Attempts, string Reason, string ExceptionType);

// Exercise 047 — DeadLetterQueue (services-data).
// Goal:   Give a message that cannot be processed somewhere to go, instead of retrying
//         it until the end of time or dropping it on the floor.
// Drills: attempt counting, poison messages, dead-lettering with a reason.
// Passes: success        - handled, nothing dead-lettered.
//         transient      - failing twice and then succeeding on the third delivery is NOT
//                          dead-lettered.
//         poison         - failing maxAttempts times is dead-lettered ONCE, carrying the
//                          attempt count, the exception's message as Reason, and its type
//                          name.
//         THE ONE         - the dead letter carries the PAYLOAD.
//         no duplicates  - delivering the same id again after it was dead-lettered does
//                          not add a second entry.
//
// Two things separate a dead-letter queue from a try/catch that gives up. The payload,
// because the only reason to keep a dead letter is to fix the cause and replay it - and
// an entry that records "message m-7 failed 3 times" cannot be replayed by anyone. And
// the attempt count, because "retry forever" is how one malformed message stops a queue
// for everybody behind it, which is the outage this pattern exists to prevent.
public sealed class DeadLetterDispatcher(int maxAttempts)
{
    public IReadOnlyList<DeadLetter> DeadLetters =>
        throw new NotImplementedException("TODO: Ex047 - the messages that gave up");

    /// <summary>
    /// One delivery attempt. Returns whether the handler succeeded. On the
    /// <paramref name="maxAttempts"/>-th consecutive failure for a message id, record a
    /// dead letter instead of letting the exception escape.
    /// </summary>
    public bool Deliver(string messageId, string payload, Action<string> handler) =>
        throw new NotImplementedException(
            "TODO: Ex047 - count attempts per message id, and on the last failure record a dead letter with payload, count, reason and exception type");
}
