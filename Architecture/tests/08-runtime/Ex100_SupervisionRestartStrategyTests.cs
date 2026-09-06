using FeWoLearning.Architecture.Exercises.Runtime.Ex100;
using FeWoLearning.Architecture.Exercises.Support;

namespace FeWoLearning.Architecture.Tests.Runtime;

public class Ex100_SupervisionRestartStrategyTests
{
    private static readonly TimeSpan Window = TimeSpan.FromMinutes(1);

    private static (Supervisor Supervisor, ManualClock Clock) Build(
        SupervisionMode mode = SupervisionMode.OneForOne, int maxRestarts = 3)
    {
        var clock = new ManualClock(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var supervisor = new Supervisor(clock, mode, maxRestarts, Window);

        supervisor.Register("ingest");
        supervisor.Register("projection");
        supervisor.Register("scheduler");

        return (supervisor, clock);
    }

    [Fact]
    public void Mechanism_One_For_One_Restarts_Only_The_Child_That_Failed()
    {
        // Used where one-for-all is not needed, restarting everything turns a small fault
        // in one component into a full outage several times a day.
        var (supervisor, _) = Build();

        var decision = supervisor.Fail("ingest");

        Assert.Equal(["ingest"], decision.Restarted);
        Assert.Equal(1, supervisor.Read("ingest")!.Restarts);
        Assert.Equal(0, supervisor.Read("projection")!.Restarts);
        Assert.Equal(ChildState.Running, supervisor.Read("projection")!.State);
    }

    [Fact]
    public void Mechanism_One_For_All_Restarts_The_Siblings_Too()
    {
        // The whole reason the mode exists: their state only makes sense together, and a
        // restarted child talking to siblings holding state from before it died does not
        // look like a restart problem at all.
        var (supervisor, _) = Build(SupervisionMode.OneForAll);

        var decision = supervisor.Fail("ingest");

        Assert.Equal(["ingest", "projection", "scheduler"], decision.Restarted);
        Assert.All(["ingest", "projection", "scheduler"],
            name => Assert.Equal(ChildState.Running, supervisor.Read(name)!.State));
    }

    [Fact]
    public void Adversarial_One_For_All_Charges_The_Budget_Only_To_The_Child_That_Failed()
    {
        // Charging a sibling for somebody else's crash loop gives up on components that
        // never failed - and the log then blames the wrong one.
        var (supervisor, _) = Build(SupervisionMode.OneForAll);

        supervisor.Fail("ingest");
        supervisor.Fail("ingest");

        Assert.Equal(2, supervisor.Read("ingest")!.Restarts);
        Assert.Equal(0, supervisor.Read("projection")!.Restarts);
    }

    [Fact]
    public void Mechanism_A_Child_Over_Its_Budget_Is_Given_Up_On()
    {
        // A supervisor with no budget restarts a permanently broken child several times a
        // second, for ever - converting a failing component into a failing machine, and a
        // log nobody can read.
        var (supervisor, _) = Build(maxRestarts: 3);

        for (var i = 0; i < 3; i++)
            Assert.Equal(["ingest"], supervisor.Fail("ingest").Restarted);

        var decision = supervisor.Fail("ingest");

        Assert.Empty(decision.Restarted);
        Assert.Equal(["ingest"], decision.GivenUpOn);
        Assert.Equal(ChildState.GivenUp, supervisor.Read("ingest")!.State);
    }

    [Fact]
    public void Mechanism_Restarts_Outside_The_Window_Do_Not_Count()
    {
        // A child that fails once a week is not a crash loop, and treating it as one gives
        // up on a system that was recovering perfectly well. A budget with no window is a
        // lifetime limit, which is a different and much worse policy.
        var (supervisor, clock) = Build(maxRestarts: 3);

        for (var i = 0; i < 5; i++)
        {
            Assert.Equal(["ingest"], supervisor.Fail("ingest").Restarted);
            clock.Advance(Window * 2);
        }

        Assert.Equal(ChildState.Running, supervisor.Read("ingest")!.State);
        Assert.Equal(1, supervisor.Read("ingest")!.Restarts);
    }

    [Fact]
    public void Adversarial_A_Child_Given_Up_On_Stays_Given_Up_On()
    {
        // Otherwise the budget is only ever a pause, and the crash loop resumes at the
        // next failure - slower, and therefore harder to spot.
        var (supervisor, clock) = Build(maxRestarts: 1);
        supervisor.Fail("ingest");
        supervisor.Fail("ingest");
        Assert.Equal(ChildState.GivenUp, supervisor.Read("ingest")!.State);

        clock.Advance(Window * 10);
        var decision = supervisor.Fail("ingest");

        Assert.Empty(decision.Restarted);
        Assert.Equal(ChildState.GivenUp, supervisor.Read("ingest")!.State);
    }

    [Fact]
    public void One_For_All_Does_Not_Resurrect_A_Child_Already_Given_Up_On()
    {
        var (supervisor, _) = Build(SupervisionMode.OneForAll, maxRestarts: 1);
        supervisor.Fail("ingest");
        supervisor.Fail("ingest");
        Assert.Equal(ChildState.GivenUp, supervisor.Read("ingest")!.State);

        var decision = supervisor.Fail("projection");

        Assert.DoesNotContain("ingest", decision.Restarted);
        Assert.Equal(ChildState.GivenUp, supervisor.Read("ingest")!.State);
    }

    [Fact]
    public void Failing_An_Unknown_Child_Does_Nothing()
    {
        var (supervisor, _) = Build();

        var decision = supervisor.Fail("never-registered");

        Assert.Empty(decision.Restarted);
        Assert.Empty(decision.GivenUpOn);
    }
}
