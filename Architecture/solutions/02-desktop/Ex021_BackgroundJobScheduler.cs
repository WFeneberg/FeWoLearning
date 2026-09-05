namespace FeWoLearning.Architecture.Exercises.Desktop.Ex021;

// Exercise 021 — BackgroundJobScheduler (reference solution).
public sealed class JobScheduler
{
    private sealed record Job(
        string Id,
        Func<CancellationToken, Task> Work,
        TaskCompletionSource Completion,
        CancellationTokenSource Cancellation);

    private readonly List<Job> _queue = [];

    public Task Enqueue(string id, Func<CancellationToken, Task> work)
    {
        ArgumentNullException.ThrowIfNull(work);

        var job = new Job(
            id,
            work,
            new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously),
            new CancellationTokenSource());

        _queue.Add(job);
        return job.Completion.Task;
    }

    public void Cancel(string id)
    {
        var job = _queue.FirstOrDefault(j => j.Id == id);
        if (job is null)
            return; // never enqueued, or already run - nothing to do either way

        _queue.Remove(job);
        job.Cancellation.Cancel();

        // Completing the Task is not optional. Dropping the job and forgetting it
        // leaves whoever awaited Enqueue waiting for a completion that will never
        // arrive - a hang rather than an error, and one that only shows up under load.
        job.Completion.TrySetCanceled(job.Cancellation.Token);
    }

    public async Task DrainAsync()
    {
        // Await inside the loop, on purpose: this IS the serialisation. Collecting the
        // tasks and Task.WhenAll-ing them would run every job at once, which is a
        // different scheduler with different guarantees.
        while (_queue.Count > 0)
        {
            var job = _queue[0];
            _queue.RemoveAt(0);

            try
            {
                await job.Work(job.Cancellation.Token);
                job.Completion.TrySetResult();
            }
            catch (OperationCanceledException)
            {
                job.Completion.TrySetCanceled(job.Cancellation.Token);
            }
            catch (Exception ex)
            {
                // One failing job must not abandon the ones behind it.
                job.Completion.TrySetException(ex);
            }
        }
    }
}
