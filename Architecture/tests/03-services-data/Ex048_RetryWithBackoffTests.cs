using FeWoLearning.Architecture.Exercises.ServicesData.Ex048;

namespace FeWoLearning.Architecture.Tests.ServicesData;

public class Ex048_RetryWithBackoffTests
{
    private static readonly TimeSpan Base = TimeSpan.FromSeconds(1);

    private static RetryPolicy NoJitter(int maxAttempts = 4) =>
        new(Base, maxAttempts, jitterFraction: 0, random: () => 0.5);

    [Fact]
    public void The_First_Attempt_Waits_For_Nothing()
    {
        Assert.Equal(TimeSpan.Zero, NoJitter().DelayBefore(1));
    }

    [Fact]
    public void Mechanism_The_Delays_Double()
    {
        // A fixed delay passes any assertion that merely checks "there was a wait", and
        // re-applies the same load to something that is already struggling.
        var policy = NoJitter();

        Assert.Equal(Base, policy.DelayBefore(2));
        Assert.Equal(Base * 2, policy.DelayBefore(3));
        Assert.Equal(Base * 4, policy.DelayBefore(4));
    }

    [Fact]
    public void Mechanism_Jitter_Stays_Inside_Its_Declared_Bounds()
    {
        // Bounds, not equality - a jittered delay has no single correct value, and a
        // fact that pinned one would either be asserting the implementation's arithmetic
        // or would have to disable the jitter it is supposed to be testing.
        var lowest = new RetryPolicy(Base, 4, jitterFraction: 0.5, random: () => 0.0);
        var highest = new RetryPolicy(Base, 4, jitterFraction: 0.5, random: () => 1.0);
        var middle = new RetryPolicy(Base, 4, jitterFraction: 0.5, random: () => 0.5);

        Assert.Equal(Base * 0.5, lowest.DelayBefore(2));
        Assert.Equal(Base * 1.5, highest.DelayBefore(2));
        Assert.Equal(Base, middle.DelayBefore(2));

        // ...and it scales with the doubling rather than replacing it.
        Assert.InRange(highest.DelayBefore(4), Base * 4 * 0.5, Base * 4 * 1.5);
    }

    [Fact]
    public void Adversarial_Zero_Jitter_Really_Means_No_Spread()
    {
        // Pairs with the fact above: an implementation that always jitters, or that
        // ignores the fraction, would make the growth fact untestable.
        var policy = NoJitter();

        Assert.Equal(policy.DelayBefore(3), policy.DelayBefore(3));
        Assert.Equal(Base * 2, policy.DelayBefore(3));
    }

    [Fact]
    public void Execute_Retries_Until_The_Work_Succeeds()
    {
        var slept = new List<TimeSpan>();
        var attempts = 0;

        var result = NoJitter().Execute(
            () => ++attempts < 3 ? throw new InvalidOperationException("not yet") : "done",
            slept.Add);

        Assert.Equal("done", result);
        Assert.Equal(3, attempts);
        Assert.Equal([Base, Base * 2], slept);
    }

    [Fact]
    public void Mechanism_The_Budget_Runs_Out_And_The_Last_Failure_Propagates()
    {
        // Without a budget a permanently broken dependency turns every caller into an
        // infinite loop against it - the retry becomes its own outage. And the caller
        // must see the real exception, not a wrapper that hides what actually failed.
        var slept = new List<TimeSpan>();
        var attempts = 0;

        var failure = Assert.Throws<InvalidOperationException>(() => NoJitter(maxAttempts: 3).Execute<string>(
            () => { attempts++; throw new InvalidOperationException("always broken"); },
            slept.Add));

        Assert.Equal("always broken", failure.Message);
        Assert.Equal(3, attempts);
        Assert.Equal(2, slept.Count);
    }

    [Fact]
    public void Work_That_Succeeds_First_Time_Never_Sleeps()
    {
        var slept = new List<TimeSpan>();

        Assert.Equal("done", NoJitter().Execute(() => "done", slept.Add));
        Assert.Empty(slept);
    }
}
