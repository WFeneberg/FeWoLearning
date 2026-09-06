using FeWoLearning.Architecture.Exercises.Scale.Ex062;

namespace FeWoLearning.Architecture.Tests.Scale;

public class Ex062_ConcurrencyLimiterTests
{
    private static readonly TimeSpan Patience = TimeSpan.FromSeconds(15);

    private static void WaitFor(Func<bool> condition, string what)
    {
        var deadline = DateTime.UtcNow + Patience;

        while (!condition())
        {
            Assert.True(DateTime.UtcNow < deadline, what);
            Thread.Sleep(10);
        }
    }

    /// <summary>Occupies a slot until released, so the controller's state is observable.</summary>
    private sealed class Occupancy
    {
        public TaskCompletionSource Release { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public async Task<string> Hold()
        {
            await Release.Task;
            return "held";
        }
    }

    [Fact]
    public async Task Work_Runs_And_Returns()
    {
        var controller = new AdmissionController(concurrency: 2, queueDepth: 2);

        Assert.Equal("ok", await controller.ExecuteAsync(() => Task.FromResult("ok")));
        Assert.Equal(0, controller.Running);
    }

    [Fact]
    public async Task Callers_Beyond_The_Concurrency_Wait_Rather_Than_Failing()
    {
        // The queue absorbs a burst the service could genuinely have handled. Rejecting
        // at exactly `concurrency` refuses work that would have been served a
        // millisecond later.
        var controller = new AdmissionController(concurrency: 2, queueDepth: 2);
        var occupancy = new Occupancy();

        var running = Enumerable.Range(0, 2).Select(_ => Task.Run(() => controller.ExecuteAsync(occupancy.Hold))).ToArray();
        WaitFor(() => controller.Running == 2, "the first two never started");

        var waiting = Task.Run(() => controller.ExecuteAsync(() => Task.FromResult("queued")));
        WaitFor(() => controller.Queued == 1, "the third caller was not queued");

        Assert.False(waiting.IsCompleted);

        occupancy.Release.SetResult();
        await Task.WhenAll(running).WaitAsync(Patience);
        Assert.Equal("queued", await waiting.WaitAsync(Patience));
    }

    [Fact]
    public async Task Mechanism_Beyond_The_Queue_A_Caller_Is_Refused_Without_Running()
    {
        // The line that turns "slow" into "refused". With an unbounded queue nothing is
        // ever refused, the queue grows until everything in it has already timed out on
        // the client side, and the service spends all its capacity computing answers
        // nobody is waiting for. That state is stable, self-sustaining, and reads from
        // outside as a total outage.
        var controller = new AdmissionController(concurrency: 1, queueDepth: 1);
        var occupancy = new Occupancy();
        var shedWorkRan = 0;

        var running = Task.Run(() => controller.ExecuteAsync(occupancy.Hold));
        WaitFor(() => controller.Running == 1, "the first caller never started");

        var queued = Task.Run(() => controller.ExecuteAsync(() => Task.FromResult("queued")));
        WaitFor(() => controller.Queued == 1, "the second caller was not queued");

        // WaitAsync, not a bare await. An unbounded-queue implementation does not reject
        // this caller, it QUEUES it - behind a holder the test only releases further down,
        // which never runs because the assertion above it has not returned. The fact would
        // hang instead of failing, and a hanging fact is worse than a failing one: it
        // stalls the whole suite and reports nothing. Measured while probing this batch.
        await Assert.ThrowsAsync<LoadSheddingException>(() => controller.ExecuteAsync(() =>
        {
            Interlocked.Increment(ref shedWorkRan);
            return Task.FromResult("should not run");
        }).WaitAsync(TimeSpan.FromSeconds(5)));

        Assert.Equal(0, shedWorkRan);

        occupancy.Release.SetResult();
        await running.WaitAsync(Patience);
        await queued.WaitAsync(Patience);
    }

    [Fact]
    public async Task Adversarial_Refusal_Is_Immediate_Rather_Than_After_A_Wait()
    {
        // "Reject after waiting for a slot" is the natural half-implementation, and it
        // gives the caller the worst of both: it waits as long as a queued request and
        // then fails anyway. A client told no in a millisecond can fail over, retry
        // elsewhere or show a message; one held for thirty seconds cannot.
        var controller = new AdmissionController(concurrency: 1, queueDepth: 0);
        var occupancy = new Occupancy();

        var running = Task.Run(() => controller.ExecuteAsync(occupancy.Hold));
        WaitFor(() => controller.Running == 1, "the first caller never started");

        var shed = controller.ExecuteAsync(() => Task.FromResult("no"));

        // Already faulted, without anything having been released. The timeout is the same
        // guard as above: an implementation that queues instead of refusing must fail
        // here rather than hang.
        await Assert.ThrowsAsync<LoadSheddingException>(() => shed.WaitAsync(TimeSpan.FromSeconds(5)));
        Assert.Equal(1, controller.Running);

        occupancy.Release.SetResult();
        await running.WaitAsync(Patience);
    }

    [Fact]
    public async Task A_Finished_Caller_Lets_A_Queued_One_In()
    {
        var controller = new AdmissionController(concurrency: 1, queueDepth: 3);
        var occupancy = new Occupancy();

        var running = Task.Run(() => controller.ExecuteAsync(occupancy.Hold));
        WaitFor(() => controller.Running == 1, "the first caller never started");

        var queued = Task.Run(() => controller.ExecuteAsync(() => Task.FromResult("second")));
        WaitFor(() => controller.Queued == 1, "the second caller was not queued");

        occupancy.Release.SetResult();

        Assert.Equal("held", await running.WaitAsync(Patience));
        Assert.Equal("second", await queued.WaitAsync(Patience));
        Assert.Equal(0, controller.Running);
        Assert.Equal(0, controller.Queued);
    }

    [Fact]
    public async Task Adversarial_A_Throwing_Caller_Still_Frees_Its_Slot()
    {
        var controller = new AdmissionController(concurrency: 1, queueDepth: 1);

        for (var i = 0; i < 5; i++)
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                controller.ExecuteAsync<int>(() => throw new InvalidOperationException("boom")));

        Assert.Equal(0, controller.Running);
        Assert.Equal(7, await controller.ExecuteAsync(() => Task.FromResult(7)).WaitAsync(Patience));
    }
}
