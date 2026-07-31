namespace FeWoLearning.Exercises.Advanced;

// Exercise 075 — Cancellable operation (advanced).
// Goal:   Implement a long-running async loop that cooperatively observes a
//         CancellationToken and stops promptly by throwing
//         OperationCanceledException instead of running to completion.
// Drills: async/await, CancellationToken propagation, ThrowIfCancellationRequested,
//         cooperative cancellation semantics.
public static class CancellableOperation
{
    // Runs 'totalIterations' async steps, invoking 'onIteration' (if provided)
    // with the zero-based index before each step's async work. Must check the
    // token for cancellation before doing (or reporting) each step's work, and
    // must throw OperationCanceledException (carrying 'cancellationToken') as
    // soon as cancellation is observed, without invoking 'onIteration' for the
    // step that detected cancellation.
    // Returns 'totalIterations' if the loop runs to completion uncancelled.
    public static Task<int> RunAsync(int totalIterations, Action<int>? onIteration, CancellationToken cancellationToken)
        => throw new NotImplementedException();
}
