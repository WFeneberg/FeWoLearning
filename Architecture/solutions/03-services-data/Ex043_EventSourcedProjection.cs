namespace FeWoLearning.Architecture.Exercises.ServicesData.Ex043;

/// <summary>
/// One entry in the global log. Position is the thing the checkpoint refers to, and it
/// is why a projection can be resumed rather than only rebuilt.
/// </summary>
public sealed record LogEntry(long Position, string AccountId, decimal Delta);

// Exercise 043 — EventSourcedProjection (reference solution).
public sealed class BalanceProjection
{
    private readonly Dictionary<string, decimal> _balances = [];

    public IReadOnlyDictionary<string, decimal> Balances => _balances;

    public long Checkpoint { get; private set; }

    public void CatchUp(IReadOnlyList<LogEntry> log)
    {
        ArgumentNullException.ThrowIfNull(log);

        // Ordered, and filtered by the checkpoint. Both halves matter: applying whatever
        // arrives produces the right answer the first time and doubles every balance the
        // second - and it WILL arrive twice, because the thing feeding it is
        // at-least-once.
        foreach (var entry in log.Where(e => e.Position > Checkpoint).OrderBy(e => e.Position))
        {
            _balances[entry.AccountId] = _balances.GetValueOrDefault(entry.AccountId) + entry.Delta;

            // Moved with each entry, not once at the end. A projection interrupted
            // halfway resumes from where it stopped rather than replaying work it has
            // already done.
            Checkpoint = entry.Position;
        }
    }
}
