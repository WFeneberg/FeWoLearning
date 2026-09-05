using FeWoLearning.Architecture.Exercises.Desktop.Ex021;

namespace FeWoLearning.Architecture.Tests.Desktop;

public class Ex021_BackgroundJobSchedulerTests
{
    [Fact]
    public async Task Jobs_Run_In_The_Order_They_Were_Enqueued()
    {
        var scheduler = new JobScheduler();
        var log = new List<string>();

        _ = scheduler.Enqueue("a", _ => { log.Add("a"); return Task.CompletedTask; });
        _ = scheduler.Enqueue("b", _ => { log.Add("b"); return Task.CompletedTask; });
        _ = scheduler.Enqueue("c", _ => { log.Add("c"); return Task.CompletedTask; });

        await scheduler.DrainAsync();

        Assert.Equal(["a", "b", "c"], log);
    }

    [Fact]
    public async Task Mechanism_No_Two_Jobs_Ever_Overlap()
    {
        // Collecting the jobs and Task.WhenAll-ing them passes the ordering fact above
        // whenever the work happens to be synchronous, and is a completely different
        // scheduler. A job that yields is what tells them apart.
        var scheduler = new JobScheduler();
        var running = 0;
        var peak = 0;

        for (var i = 0; i < 5; i++)
        {
            _ = scheduler.Enqueue($"job-{i}", async _ =>
            {
                peak = Math.Max(peak, Interlocked.Increment(ref running));
                await Task.Yield();
                Interlocked.Decrement(ref running);
            });
        }

        await scheduler.DrainAsync();

        Assert.Equal(1, peak);
    }

    [Fact]
    public async Task Mechanism_Cancelling_A_Queued_Job_Skips_It_And_Leaves_The_Others_Alone()
    {
        var scheduler = new JobScheduler();
        var log = new List<string>();

        _ = scheduler.Enqueue("a", _ => { log.Add("a"); return Task.CompletedTask; });
        _ = scheduler.Enqueue("b", _ => { log.Add("b"); return Task.CompletedTask; });
        _ = scheduler.Enqueue("c", _ => { log.Add("c"); return Task.CompletedTask; });

        scheduler.Cancel("b");
        await scheduler.DrainAsync();

        Assert.Equal(["a", "c"], log);
    }

    [Fact]
    public async Task Adversarial_A_Cancelled_Jobs_Task_Completes_As_Cancelled()
    {
        // The trap. Dropping the job from the queue and forgetting it satisfies the fact
        // above completely, and leaves whoever awaited Enqueue waiting for a completion
        // that never arrives - a hang, not an error, visible only under load. The
        // timeout is what turns that hang into a failing test rather than a stuck suite.
        var scheduler = new JobScheduler();

        var pending = scheduler.Enqueue("b", _ => Task.CompletedTask);
        scheduler.Cancel("b");

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => pending.WaitAsync(TimeSpan.FromSeconds(10)));
    }

    [Fact]
    public async Task Cancelling_An_Unknown_Id_Does_Nothing()
    {
        var scheduler = new JobScheduler();
        var log = new List<string>();
        _ = scheduler.Enqueue("a", _ => { log.Add("a"); return Task.CompletedTask; });

        scheduler.Cancel("never-enqueued");
        await scheduler.DrainAsync();

        Assert.Equal(["a"], log);
    }

    [Fact]
    public async Task One_Failing_Job_Does_Not_Abandon_The_Ones_Behind_It()
    {
        var scheduler = new JobScheduler();
        var log = new List<string>();

        var failing = scheduler.Enqueue("a", _ => throw new InvalidOperationException("boom"));
        _ = scheduler.Enqueue("b", _ => { log.Add("b"); return Task.CompletedTask; });

        await scheduler.DrainAsync();

        Assert.Equal(["b"], log);
        await Assert.ThrowsAsync<InvalidOperationException>(() => failing);
    }
}
