namespace FeWoLearning.Exercises.Intermediate;

// Exercise 057 — EventPublisher (intermediate).
// Goal:   Implement a Counter class that raises a ThresholdReached event the
//         moment its running total first exceeds a configured threshold.
//         The event must fire exactly once — additional increments after the
//         threshold has been crossed must NOT raise it again.
// Drills: events, delegates, EventArgs, encapsulated mutable state.
public class ThresholdReachedEventArgs : EventArgs
{
    public int Value { get; }

    public ThresholdReachedEventArgs(int value)
    {
        Value = value;
    }
}

public class Counter
{
    public Counter(int threshold) => throw new NotImplementedException();

    public int Value => throw new NotImplementedException();

    public bool HasReachedThreshold => throw new NotImplementedException();

    public event EventHandler<ThresholdReachedEventArgs>? ThresholdReached;

    public void Increment(int amount = 1) => throw new NotImplementedException();
}
