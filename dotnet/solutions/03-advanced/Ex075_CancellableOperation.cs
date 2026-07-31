namespace FeWoLearning.Exercises.Advanced;

// Exercise 075 — Cancellable operation (reference solution).
// Checks the token before each iteration's work so cancellation is observed
// promptly and cooperatively, rather than letting the loop run to completion.
public static class CancellableOperation
{
    public static async Task<int> RunAsync(int totalIterations, Action<int>? onIteration, CancellationToken cancellationToken)
    {
        for (var i = 0; i < totalIterations; i++)
        {
            // Observe cancellation before performing (or reporting) this step's
            // work, so a cancellation requested between steps stops us here
            // rather than after one more unit of work has already happened.
            cancellationToken.ThrowIfCancellationRequested();

            onIteration?.Invoke(i);

            // Yield to simulate real async work and give cancellation a chance
            // to be observed again on the next loop check.
            await Task.Yield();
        }

        return totalIterations;
    }
}
