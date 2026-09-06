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

// Exercise 040 — SagaProcessManager (services-data).
// Goal:   Run a multi-step process across services that have no shared transaction, and
//         unwind it correctly when a step fails.
// Drills: saga state, compensation, unwind order, best-effort cleanup.
// Passes: all steps succeed - Succeeded, nothing compensated.
//         step 3 of 4 fails - FailedStep is step 3; step 4 NEVER RUNS; steps 2 and 1 are
//                             compensated IN THAT ORDER; and step 3 itself is NOT
//                             compensated, because it never completed.
//         a compensation that itself throws does not stop the remaining ones, and is
//                             reported in CompensationFailures.
//         an empty saga succeeds.
//
// Two things separate this from a try/catch. First, the ORDER: compensating forward
// undoes step 1 while step 2 still depends on it - refunding the payment before
// cancelling the shipment it paid for. Second, NOT compensating the step that failed:
// it did not complete, so undoing it is undoing something that never happened, which for
// a refund means paying out money that was never taken.
//
// Compensation is best-effort by necessity. If undoing step 2 fails there is nobody left
// to appeal to, so the saga records it and keeps unwinding - stopping there would strand
// step 1 as well and turn one manual cleanup into two.
public static class Ex040_SagaProcessManager
{
    public static Task<SagaResult> RunAsync(IReadOnlyList<SagaStep> steps) =>
        throw new NotImplementedException(
            "TODO: Ex040 - run the steps in order; on a failure compensate the COMPLETED ones in reverse, recording any compensation that itself throws");
}
