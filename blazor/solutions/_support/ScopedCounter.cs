namespace FeWoLearning.Blazor.Support;

/// <summary>
/// Test fixture state container for the DI lifetime exercises (Ex044),
/// registered with AddScoped. A distinct type from SingletonCounter -
/// deliberately not sharing a base with it - so that resolving one of each
/// from two injection sites proves the lifetimes differ by type identity,
/// not by a shared count. SubscriberCount is derived from the live
/// invocation list, never hand-tracked. Not an exercise.
/// </summary>
public sealed class ScopedCounter
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
