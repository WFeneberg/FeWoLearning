namespace FeWoLearning.Blazor.Support;

/// <summary>
/// Test fixture for Ex020. Counts live subscribers so a test can prove a
/// component unsubscribed on dispose. Not an exercise.
/// </summary>
public sealed class Ticker
{
    private Action? _handlers;

    public int SubscriberCount { get; private set; }

    public void Subscribe(Action handler)
    {
        _handlers += handler;
        SubscriberCount++;
    }

    public void Unsubscribe(Action handler)
    {
        _handlers -= handler;
        SubscriberCount--;
    }

    public void Tick() => _handlers?.Invoke();
}
