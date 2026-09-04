namespace FeWoLearning.Blazor.Support;

/// <summary>
/// Test fixture state container for the DI exercises. SubscriberCount is
/// derived from the live invocation list, never hand-tracked, so a component
/// that subscribes and never unsubscribes cannot pass. Not an exercise.
/// </summary>
public sealed class CounterStore
{
    public int Value { get; private set; }

    public event Action? Changed;

    public int SubscriberCount => Changed?.GetInvocationList().Length ?? 0;

    public void Increment()
    {
        Value++;
        Changed?.Invoke();
    }
}
