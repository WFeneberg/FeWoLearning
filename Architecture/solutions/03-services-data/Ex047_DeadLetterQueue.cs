namespace FeWoLearning.Architecture.Exercises.ServicesData.Ex047;

/// <summary>
/// A message that gave up, with everything needed to work out why. The payload is here
/// on purpose: a dead letter without its body is a log line, and nobody can replay a
/// log line.
/// </summary>
public sealed record DeadLetter(string MessageId, string Payload, int Attempts, string Reason, string ExceptionType);

// Exercise 047 — DeadLetterQueue (reference solution).
public sealed class DeadLetterDispatcher(int maxAttempts)
{
    private readonly Dictionary<string, int> _attempts = [];
    private readonly HashSet<string> _buried = new(StringComparer.Ordinal);
    private readonly List<DeadLetter> _deadLetters = [];

    public IReadOnlyList<DeadLetter> DeadLetters => _deadLetters;

    public bool Deliver(string messageId, string payload, Action<string> handler)
    {
        if (_buried.Contains(messageId))
            return false; // already given up on; a redelivery must not create a second entry

        var attempt = _attempts.GetValueOrDefault(messageId) + 1;
        _attempts[messageId] = attempt;

        try
        {
            handler(payload);

            // Success clears the count, so a message that fails twice and then works is
            // not one failure away from being buried for the rest of the process.
            _attempts.Remove(messageId);
            return true;
        }
        catch (Exception ex)
        {
            if (attempt < maxAttempts)
                return false; // there is still budget: let the broker redeliver

            // The payload goes with it. A dead letter that records only "m-7 failed 3
            // times" cannot be replayed by anybody, and replaying it after fixing the
            // cause is the only reason to keep it.
            _deadLetters.Add(new DeadLetter(messageId, payload, attempt, ex.Message, ex.GetType().Name));
            _buried.Add(messageId);
            return false;
        }
    }
}
