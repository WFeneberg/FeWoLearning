using FeWoLearning.Architecture.Exercises.Domain.Ex086;
using FeWoLearning.Architecture.Exercises.Support;

namespace FeWoLearning.Architecture.Tests.Domain;

public class Ex086_WorkflowTimeoutsTests
{
    private static readonly TimeSpan Window = TimeSpan.FromHours(24);

    private static (WaitingStepStore Store, ManualClock Clock) Build()
    {
        var clock = new ManualClock(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));
        return (new WaitingStepStore(clock), clock);
    }

    [Fact]
    public void A_Step_Inside_Its_Window_Is_Pending()
    {
        var (store, clock) = Build();
        store.Start("awaiting-courier", Window);

        clock.Advance(Window - TimeSpan.FromMinutes(1));

        Assert.Equal(StepOutcome.Pending, store.Read("awaiting-courier")!.Outcome);
    }

    [Fact]
    public void An_Answer_Inside_The_Window_Completes_It()
    {
        var (store, clock) = Build();
        store.Start("awaiting-courier", Window);
        clock.Advance(TimeSpan.FromHours(2));

        Assert.True(store.TryComplete("awaiting-courier"));
        Assert.Equal(StepOutcome.Completed, store.Read("awaiting-courier")!.Outcome);
    }

    [Fact]
    public void A_Completed_Step_Does_Not_Time_Out_Later()
    {
        // Once answered, the deadline stops mattering. A Judge that looks only at the
        // clock would re-open a finished step the moment the window passes.
        var (store, clock) = Build();
        store.Start("awaiting-courier", Window);
        store.TryComplete("awaiting-courier");

        clock.Advance(Window * 10);

        Assert.Equal(StepOutcome.Completed, store.Read("awaiting-courier")!.Outcome);
    }

    [Fact]
    public void Mechanism_A_Step_Times_Out_Without_Anybody_Polling_It()
    {
        // A step that only becomes TimedOut when a background job gets round to setting a
        // flag reads as Pending in every query until then - including the one the
        // escalation runs. A sweep an hour late then hides an hour of overdue work rather
        // than reporting it.
        var (store, clock) = Build();
        store.Start("awaiting-courier", Window);

        clock.Advance(Window);

        Assert.Equal(StepOutcome.TimedOut, store.Read("awaiting-courier")!.Outcome);
    }

    [Fact]
    public void Mechanism_An_Answer_After_The_Deadline_Is_Refused()
    {
        // The escalation has already run. Accepting the late answer means the escalation
        // path and the happy path have both acted on one step - and which of them the
        // business wants is a business question, not a race.
        var (store, clock) = Build();
        store.Start("awaiting-courier", Window);
        clock.Advance(Window);

        Assert.False(store.TryComplete("awaiting-courier"));
        Assert.Equal(StepOutcome.TimedOut, store.Read("awaiting-courier")!.Outcome);
    }

    [Fact]
    public void The_Sweep_Reports_Every_Overdue_Step_Oldest_First()
    {
        // Escalation driven by a query rather than by luck. Ordering by age is what lets
        // the operator start with the one that has been waiting longest.
        var (store, clock) = Build();
        store.Start("first", Window);
        clock.Advance(TimeSpan.FromHours(1));
        store.Start("second", Window);
        clock.Advance(TimeSpan.FromHours(1));
        store.Start("fresh", Window * 10);

        clock.Advance(Window);

        Assert.Equal(["first", "second"], store.SweepTimedOut().Select(s => s.Name));
    }

    [Fact]
    public void Adversarial_The_Sweep_Does_Not_Change_Anything()
    {
        // Judging on read means the sweep is a pure query. One that WRITES a flag turns a
        // reporting call into a state transition, and running it twice - or from two
        // instances - becomes something that needs its own coordination.
        var (store, clock) = Build();
        store.Start("awaiting-courier", Window);
        clock.Advance(Window);

        Assert.Single(store.SweepTimedOut());
        Assert.Single(store.SweepTimedOut());
        Assert.Equal(StepOutcome.TimedOut, store.Read("awaiting-courier")!.Outcome);
    }

    [Fact]
    public void An_Unknown_Step_Reads_As_Nothing()
    {
        var (store, _) = Build();

        Assert.Null(store.Read("never-started"));
        Assert.False(store.TryComplete("never-started"));
    }
}
