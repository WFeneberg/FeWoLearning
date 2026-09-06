namespace FeWoLearning.Architecture.Exercises.ServicesData.Ex040;

/// <summary>
/// One step and the thing that undoes it. Compensation is not a rollback: the money has
/// left, the email has been sent, the warehouse has moved a box. What you can do is a
/// NEW action that puts the world back - a refund, a correction, a restock.
/// </summary>
public sealed record SagaStep(string Name, Func<Task> Execute, Func<Task> Compensate);

public sealed record SagaResult(
    bool Succeeded,
    string? FailedStep,
    IReadOnlyList<string> Compensated,
    IReadOnlyList<string> CompensationFailures);

// Exercise 040 — SagaProcessManager (reference solution).
public static class Ex040_SagaProcessManager
{
    public static async Task<SagaResult> RunAsync(IReadOnlyList<SagaStep> steps)
    {
        ArgumentNullException.ThrowIfNull(steps);

        // Only the steps that actually COMPLETED go on this stack. The failing step does
        // not: it never finished, so undoing it means undoing something that never
        // happened - for a refund, paying out money that was never taken.
        var completed = new Stack<SagaStep>();

        foreach (var step in steps)
        {
            try
            {
                await step.Execute().ConfigureAwait(false);
                completed.Push(step);
            }
            catch
            {
                var (compensated, failures) = await CompensateAsync(completed).ConfigureAwait(false);
                return new SagaResult(false, step.Name, compensated, failures);
            }
        }

        return new SagaResult(true, null, [], []);
    }

    private static async Task<(List<string> Compensated, List<string> Failures)> CompensateAsync(
        Stack<SagaStep> completed)
    {
        var compensated = new List<string>();
        var failures = new List<string>();

        // A Stack, so this unwinds in reverse. Compensating forward undoes step 1 while
        // step 2 still depends on it - refunding the payment before cancelling the
        // shipment it paid for.
        while (completed.Count > 0)
        {
            var step = completed.Pop();

            try
            {
                await step.Compensate().ConfigureAwait(false);
                compensated.Add(step.Name);
            }
            catch
            {
                // Record and keep going. There is nobody left to appeal to, and stopping
                // here would strand every step below as well - turning one manual
                // cleanup into several.
                failures.Add(step.Name);
            }
        }

        return (compensated, failures);
    }
}
