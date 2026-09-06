using FeWoLearning.Architecture.Exercises.Domain.Ex088;
using FeWoLearning.Architecture.Exercises.Support;

namespace FeWoLearning.Architecture.Tests.Domain;

public class Ex088_LongRunningOperationTests
{
    private static (OperationStore Store, ManualClock Clock) Build()
    {
        var clock = new ManualClock(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));
        return (new OperationStore(clock), clock);
    }

    [Fact]
    public void Mechanism_Starting_Returns_202_And_Somewhere_To_Watch()
    {
        // Not 200, and no result in the body - the work has not been done. An HTTP request
        // is a bad place to keep a fifteen-minute job: the load balancer has an idle
        // timeout, so does the client, a retry starts the work twice, and a deploy in the
        // middle loses it.
        var (store, _) = Build();

        var accepted = store.Start("op-1");

        Assert.Equal(202, accepted.StatusCode);
        Assert.Equal("/operations/op-1", accepted.Location);
        Assert.Equal(OperationState.Running, accepted.Body!.State);
        Assert.Null(accepted.Body.ResultLocation);
    }

    [Fact]
    public void Polling_A_Running_Operation_Reports_Progress()
    {
        var (store, _) = Build();
        store.Start("op-1");

        store.Report("op-1", 40);
        var polled = store.Poll("op-1");

        Assert.Equal(200, polled.StatusCode);
        Assert.Equal(OperationState.Running, polled.Body!.State);
        Assert.Equal(40, polled.Body.PercentComplete);
    }

    [Fact]
    public void Mechanism_Success_Points_At_A_Separate_Result_Resource()
    {
        // Returning the result in the poll body makes it reachable exactly once, from
        // exactly that poll - not cacheable, not re-fetchable, not linkable, and gone if
        // the client crashed while reading it.
        var (store, clock) = Build();
        store.Start("op-1");
        clock.Advance(TimeSpan.FromMinutes(12));

        store.Succeed("op-1", "/reports/r-1");
        var polled = store.Poll("op-1");

        Assert.Equal(OperationState.Succeeded, polled.Body!.State);
        Assert.Equal(100, polled.Body.PercentComplete);
        Assert.Equal("/reports/r-1", polled.Body.ResultLocation);
        Assert.Equal(clock.UtcNow, polled.Body.CompletedAt);
    }

    [Fact]
    public void Mechanism_A_Failed_Operation_Still_Polls_With_200()
    {
        // The clause that gets argued about. The poll asked "how is it going" and the
        // answer - "badly" - arrived successfully. A 500 fires the client's transport
        // error handling for a transport problem it does not have, and most clients then
        // retry the POLL, which changes nothing at all.
        var (store, _) = Build();
        store.Start("op-1");

        store.Fail("op-1", "the source file was malformed");
        var polled = store.Poll("op-1");

        Assert.Equal(200, polled.StatusCode);
        Assert.Equal(OperationState.Failed, polled.Body!.State);
        Assert.Equal("the source file was malformed", polled.Body.Error);
        Assert.Null(polled.Body.ResultLocation);
    }

    [Fact]
    public void An_Unknown_Operation_Is_404()
    {
        var (store, _) = Build();

        var polled = store.Poll("never-started");

        Assert.Equal(404, polled.StatusCode);
        Assert.Null(polled.Body);
    }

    [Fact]
    public void Adversarial_Progress_Reported_After_Completion_Is_Ignored()
    {
        // A worker that reports 80% after the coordinator already recorded success would
        // otherwise walk the operation backwards, and a client that polled in between sees
        // a finished job become unfinished.
        var (store, _) = Build();
        store.Start("op-1");
        store.Succeed("op-1", "/reports/r-1");

        store.Report("op-1", 80);

        Assert.Equal(OperationState.Succeeded, store.Poll("op-1").Body!.State);
        Assert.Equal(100, store.Poll("op-1").Body!.PercentComplete);
    }

    [Fact]
    public void The_Start_Time_Is_Recorded_And_Does_Not_Move()
    {
        var (store, clock) = Build();
        var startedAt = clock.UtcNow;
        store.Start("op-1");

        clock.Advance(TimeSpan.FromMinutes(5));
        store.Report("op-1", 50);

        Assert.Equal(startedAt, store.Poll("op-1").Body!.StartedAt);
        Assert.Null(store.Poll("op-1").Body!.CompletedAt);
    }
}
