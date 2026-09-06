using FeWoLearning.Architecture.Exercises.Runtime.Ex097;
using FeWoLearning.Architecture.Exercises.Support;

namespace FeWoLearning.Architecture.Tests.Runtime;

public class Ex097_TimeoutBudgetTests
{
    private static readonly TimeSpan Total = TimeSpan.FromSeconds(5);

    private static (RequestBudget Budget, ManualClock Clock) Build()
    {
        var clock = new ManualClock(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));
        return (RequestBudget.StartingNow(clock, Total), clock);
    }

    [Fact]
    public void The_Remaining_Time_Shrinks_As_The_Clock_Moves()
    {
        var (budget, clock) = Build();

        clock.Advance(TimeSpan.FromSeconds(2));

        Assert.Equal(TimeSpan.FromSeconds(3), budget.Remaining);
        Assert.False(budget.IsExpired);
    }

    [Fact]
    public void Adversarial_The_Remaining_Time_Never_Goes_Negative()
    {
        // An overrun is "no time", not "minus four seconds" - and a negative TimeSpan
        // handed to a socket timeout is an argument exception in the one code path nobody
        // tested.
        var (budget, clock) = Build();

        clock.Advance(Total * 3);

        Assert.Equal(TimeSpan.Zero, budget.Remaining);
        Assert.True(budget.IsExpired);
    }

    [Fact]
    public void Mechanism_A_Nested_Call_Inherits_What_Is_Left_Not_A_Fresh_Copy()
    {
        // The whole exercise. Three sequential steps each starting a fresh five seconds
        // under a five-second request budget take fifteen - every layer times out
        // correctly, every number is defensible, and the total is nobody's number.
        var (budget, clock) = Build();
        clock.Advance(TimeSpan.FromSeconds(4));

        var nested = budget.Nest(Total);

        Assert.Equal(TimeSpan.FromSeconds(1), nested.Remaining);
    }

    [Fact]
    public void A_Nested_Call_May_Ask_For_Less()
    {
        // Capping is one-directional: a sub-call is allowed to be more impatient than its
        // caller, never less.
        var (budget, _) = Build();

        var nested = budget.Nest(TimeSpan.FromSeconds(2));

        Assert.Equal(TimeSpan.FromSeconds(2), nested.Remaining);
    }

    [Fact]
    public void A_Nested_Budget_Runs_Down_With_The_Same_Clock()
    {
        var (budget, clock) = Build();
        var nested = budget.Nest(TimeSpan.FromSeconds(3));

        clock.Advance(TimeSpan.FromSeconds(2));

        Assert.Equal(TimeSpan.FromSeconds(1), nested.Remaining);
        Assert.Equal(TimeSpan.FromSeconds(3), budget.Remaining);
    }

    [Fact]
    public void Mechanism_A_Step_That_Cannot_Finish_Is_Refused_Before_It_Starts()
    {
        // Beginning work that cannot finish spends the caller's last two seconds computing
        // a result that will be thrown away - and holds a connection while doing it.
        var (budget, clock) = Build();
        clock.Advance(TimeSpan.FromSeconds(4));

        var failure = Assert.Throws<BudgetExhaustedException>(
            () => budget.EnsureRoomFor("report-render", TimeSpan.FromSeconds(2)));

        Assert.Equal("report-render", failure.Step);
    }

    [Fact]
    public void A_Step_That_Fits_Is_Allowed()
    {
        // Paired with the fact above: "refuse when tight" must not become "refuse
        // whenever anything has elapsed".
        var (budget, clock) = Build();
        clock.Advance(TimeSpan.FromSeconds(4));

        Assert.Null(Record.Exception(() => budget.EnsureRoomFor("cheap-lookup", TimeSpan.FromMilliseconds(500))));
    }

    [Fact]
    public void Nothing_Starts_Once_The_Budget_Is_Gone()
    {
        var (budget, clock) = Build();
        clock.Advance(Total);

        Assert.True(budget.IsExpired);
        Assert.Throws<BudgetExhaustedException>(
            () => budget.EnsureRoomFor("anything", TimeSpan.FromMilliseconds(1)));
    }
}
