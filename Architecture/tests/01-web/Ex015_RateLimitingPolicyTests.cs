using FeWoLearning.Architecture.Exercises.Support;
using FeWoLearning.Architecture.Exercises.Web.Ex015;

namespace FeWoLearning.Architecture.Tests.Web;

public class Ex015_RateLimitingPolicyTests
{
    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(10);

    private static (TokenBucketLimiter Limiter, ManualClock Clock) Build(int capacity = 3)
    {
        var clock = new ManualClock(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));
        return (new TokenBucketLimiter(clock, capacity, Interval), clock);
    }

    [Fact]
    public void A_Client_Gets_Its_Capacity_And_No_More()
    {
        var (limiter, _) = Build();

        Assert.True(limiter.TryAcquire("a"));
        Assert.True(limiter.TryAcquire("a"));
        Assert.True(limiter.TryAcquire("a"));
        Assert.False(limiter.TryAcquire("a"));
    }

    [Fact]
    public void Mechanism_Budgets_Are_Held_Per_Client()
    {
        // A single shared counter passes the fact above and silently rate-limits every
        // other caller the moment one of them gets noisy.
        var (limiter, _) = Build();

        for (var i = 0; i < 3; i++)
            Assert.True(limiter.TryAcquire("a"));
        Assert.False(limiter.TryAcquire("a"));

        Assert.True(limiter.TryAcquire("b"));
    }

    [Fact]
    public void Mechanism_One_Interval_Grants_Exactly_One_Token()
    {
        // The fact that separates a token bucket from a fixed window. A fixed window
        // with the same numbers restores the WHOLE budget when the window rolls over,
        // so the second assertion below would succeed for it - and a client would fire
        // 2 x capacity requests across the boundary.
        var (limiter, clock) = Build();

        for (var i = 0; i < 3; i++)
            limiter.TryAcquire("a");
        Assert.False(limiter.TryAcquire("a"));

        clock.Advance(Interval);

        Assert.True(limiter.TryAcquire("a"));
        Assert.False(limiter.TryAcquire("a"));
    }

    [Fact]
    public void Adversarial_An_Idle_Client_Cannot_Hoard_Tokens()
    {
        // Without the cap, a client that stays quiet for an hour accrues an hour's
        // worth of tokens and can spend all of them at once - the exact burst the
        // limiter exists to prevent. Nothing above catches a missing cap.
        var (limiter, clock) = Build();

        clock.Advance(TimeSpan.FromHours(1));

        Assert.True(limiter.TryAcquire("a"));
        Assert.True(limiter.TryAcquire("a"));
        Assert.True(limiter.TryAcquire("a"));
        Assert.False(limiter.TryAcquire("a"));
    }

    [Fact]
    public void Partial_Intervals_Do_Not_Grant_A_Token()
    {
        var (limiter, clock) = Build();

        for (var i = 0; i < 3; i++)
            limiter.TryAcquire("a");

        clock.Advance(Interval / 2);

        Assert.False(limiter.TryAcquire("a"));

        // ...and the half already earned is not thrown away either: the other half
        // completes the interval and the token arrives.
        clock.Advance(Interval / 2);
        Assert.True(limiter.TryAcquire("a"));
    }
}
