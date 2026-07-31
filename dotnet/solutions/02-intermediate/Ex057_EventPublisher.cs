namespace FeWoLearning.Exercises.Intermediate;

// Exercise 057 — EventPublisher (reference solution).
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
    private readonly int _threshold;

    public Counter(int threshold)
    {
        _threshold = threshold;
    }

    public int Value { get; private set; }

    public bool HasReachedThreshold { get; private set; }

    public event EventHandler<ThresholdReachedEventArgs>? ThresholdReached;

    public void Increment(int amount = 1)
    {
        Value += amount;

        if (!HasReachedThreshold && Value > _threshold)
        {
            HasReachedThreshold = true;
            ThresholdReached?.Invoke(this, new ThresholdReachedEventArgs(Value));
        }
    }
}
