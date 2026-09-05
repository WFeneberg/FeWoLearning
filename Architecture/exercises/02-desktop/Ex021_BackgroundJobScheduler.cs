namespace FeWoLearning.Architecture.Exercises.Desktop.Ex021;

// Exercise 021 — BackgroundJobScheduler (desktop).
// Goal:   A queue that runs background work ONE JOB AT A TIME in enqueue order, and
//         lets a job still waiting in the queue be cancelled without disturbing the
//         rest.
// Drills: queued work, serial execution, cancellation, deterministic async testing.
// Passes: order        - jobs run in the order they were enqueued.
//         serialisation- no two jobs overlap, ever.
//         cancellation - cancelling a QUEUED job leaves it unexecuted, leaves the jobs
//                        after it running normally, and completes the Task that Enqueue
//                        returned as CANCELLED rather than leaving it pending forever.
//         unknown id   - cancelling something that was never enqueued is a no-op.
//
// Enqueue does not start anything: DrainAsync does. That split is what makes the
// cancellation facts deterministic - "cancel a job before it starts" is a race in any
// design where enqueueing starts the work, and a test for it would be timing-dependent
// and flaky. Nothing here sleeps.
//
// The abandoned Task is the trap. Dropping a cancelled job from the queue and forgetting
// it leaves whoever awaited Enqueue's Task waiting for a completion that will never
// come - a hang, not an error, and one that only shows up under load.
public sealed class JobScheduler
{
    /// <summary>
    /// Queue <paramref name="work"/> under <paramref name="id"/>. The returned Task
    /// completes when the job has run, or is cancelled if the job is cancelled first.
    /// </summary>
    public Task Enqueue(string id, Func<CancellationToken, Task> work) =>
        throw new NotImplementedException(
            "TODO: Ex021 - queue the job and return a Task that completes when it runs, or cancels when it is cancelled");

    /// <summary>Cancel a job that has not run yet. Unknown ids are ignored.</summary>
    public void Cancel(string id) =>
        throw new NotImplementedException(
            "TODO: Ex021 - remove the queued job and cancel the Task that Enqueue handed out for it");

    /// <summary>Run everything currently queued, one at a time, in order.</summary>
    public Task DrainAsync() =>
        throw new NotImplementedException(
            "TODO: Ex021 - run the queued jobs serially in enqueue order and complete when the queue is empty");
}
