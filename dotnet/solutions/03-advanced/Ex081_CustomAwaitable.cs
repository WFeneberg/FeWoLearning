using System.Runtime.CompilerServices;

namespace FeWoLearning.Exercises.Advanced;

// Exercise 081 — Custom awaitable type (reference solution).
// A minimal hand-rolled awaitable/awaiter pair. The compiler only requires
// three members to make `await` work: GetAwaiter() on the awaitable, plus
// IsCompleted / OnCompleted(Action) / GetResult() on the awaiter (the last
// two coming from INotifyCompletion + convention, respectively).
public sealed class CustomAwaitable
{
    private readonly int _result;
    private bool _isCompleted;
    private Action? _continuation;

    public CustomAwaitable(int result) => _result = result;

    public void Complete()
    {
        if (_isCompleted)
            return;

        _isCompleted = true;
        var continuation = _continuation;
        _continuation = null;
        continuation?.Invoke();
    }

    public Awaiter GetAwaiter() => new(this);

    public readonly struct Awaiter : INotifyCompletion
    {
        private readonly CustomAwaitable _owner;

        internal Awaiter(CustomAwaitable owner) => _owner = owner;

        public bool IsCompleted => _owner._isCompleted;

        public void OnCompleted(Action continuation)
        {
            if (_owner._isCompleted)
            {
                // Already done: invoke immediately, matching the guarantee
                // that a completed awaiter's continuation runs without delay.
                continuation();
                return;
            }

            _owner._continuation += continuation;
        }

        public int GetResult()
        {
            if (!_owner._isCompleted)
                throw new InvalidOperationException("The awaitable has not completed yet.");

            return _owner._result;
        }
    }
}
