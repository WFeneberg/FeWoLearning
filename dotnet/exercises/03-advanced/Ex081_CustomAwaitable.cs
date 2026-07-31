using System.Runtime.CompilerServices;

namespace FeWoLearning.Exercises.Advanced;

// Exercise 081 — Custom awaitable type (advanced).
// Goal:   Build a type usable with the `await` keyword by implementing the
//         GetAwaiter pattern by hand (no Task involved): GetAwaiter() returns
//         an awaiter exposing IsCompleted, OnCompleted(Action) and GetResult().
// Drills: awaitable/awaiter pattern, INotifyCompletion, manual continuations.
public sealed class CustomAwaitable
{
    public CustomAwaitable(int result) => throw new NotImplementedException();

    // Marks the awaitable as completed, invoking any continuation registered
    // via the awaiter's OnCompleted (mirrors how a real async resource signals
    // completion to whatever is awaiting it).
    public void Complete() => throw new NotImplementedException();

    public Awaiter GetAwaiter() => throw new NotImplementedException();

    public readonly struct Awaiter : INotifyCompletion
    {
        public bool IsCompleted => throw new NotImplementedException();

        public void OnCompleted(Action continuation) => throw new NotImplementedException();

        public int GetResult() => throw new NotImplementedException();
    }
}
