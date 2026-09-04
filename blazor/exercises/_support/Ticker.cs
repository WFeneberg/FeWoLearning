namespace FeWoLearning.Blazor.Support;

/// <summary>
/// Test fixture for Ex020. Counts live subscribers so a test can prove a
/// component unsubscribed on dispose. Not an exercise.
/// </summary>
public sealed class Ticker
{
    private Action? _handlers;

    public int SubscriberCount => _handlers?.GetInvocationList().Length ?? 0;

    public void Subscribe(Action handler)
    {
        _handlers += handler;
    }

    public void Unsubscribe(Action handler)
    {
        _handlers -= handler;
    }

    public void Tick() => _handlers?.Invoke();
}
