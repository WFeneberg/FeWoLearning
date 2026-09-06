using FeWoLearning.Architecture.Exercises.Runtime.Ex091;
using FeWoLearning.Architecture.Exercises.Support;

namespace FeWoLearning.Architecture.Tests.Runtime;

public class Ex091_DeterministicByDesignTests
{
    private static readonly DateTimeOffset Start = new(2026, 1, 1, 9, 0, 0, TimeSpan.Zero);

    private sealed class SequentialIds : IIdSource
    {
        private int _next;

        public string Next() => $"CPN-{++_next:D3}";
    }

    private sealed class FixedRandom(double value) : IRandomSource
    {
        public double NextDouble() => value;
    }

    private static CouponIssuer Issuer(double random = 0.0) =>
        new(new ManualClock(Start), new SequentialIds(), new FixedRandom(random));

    [Fact]
    public void Mechanism_Every_Varying_Input_Comes_From_A_Source_The_Test_Chose()
    {
        // A test can NAME the code and both timestamps rather than assert loosely around
        // them. That is the whole return on three constructor parameters.
        var coupon = Issuer().Issue(TimeSpan.FromDays(30));

        Assert.Equal("CPN-001", coupon.Code);
        Assert.Equal(Start, coupon.IssuedAt);
        Assert.Equal(Start.AddDays(30), coupon.ExpiresAt);
    }

    [Fact]
    public void Mechanism_The_Same_Sources_Produce_The_Same_Coupon()
    {
        // The property a type reading DateTime.UtcNow and Guid.NewGuid cannot have. Every
        // timing fact in this repository rests on it.
        var first = Issuer(0.5).Issue(TimeSpan.FromDays(30));
        var second = Issuer(0.5).Issue(TimeSpan.FromDays(30));

        Assert.Equal(first, second);
    }

    [Fact]
    public void Adversarial_Both_Timestamps_Come_From_One_Reading_Of_The_Clock()
    {
        // Two reads of the clock can straddle a tick, and then IssuedAt and the basis for
        // ExpiresAt disagree by a millisecond nobody will ever reproduce. A clock that
        // moves between the two reads makes the difference visible.
        var clock = new ManualClock(Start);
        var moving = new MovingClock(clock);
        var issuer = new CouponIssuer(moving, new SequentialIds(), new FixedRandom(0));

        var coupon = issuer.Issue(TimeSpan.FromDays(30));

        Assert.Equal(coupon.ExpiresAt - coupon.IssuedAt, TimeSpan.FromDays(30));
    }

    [Fact]
    public void The_Discount_Is_Scaled_By_The_Random_Source()
    {
        // Both ends of the range are checkable because the source is a parameter. With
        // Random.Shared the only honest assertion would be a range, and a range wide
        // enough to be safe is wide enough to be useless.
        Assert.Equal(CouponIssuer.BaseDiscount, Issuer(0.0).Issue(TimeSpan.FromDays(1)).DiscountFraction);
        Assert.Equal(CouponIssuer.BaseDiscount * 2, Issuer(1.0).Issue(TimeSpan.FromDays(1)).DiscountFraction);
    }

    [Fact]
    public void Fitness_A_Type_That_Takes_Its_Clock_Is_Not_Reported()
    {
        // Paired with the fact below - alone, an empty list satisfies it.
        Assert.DoesNotContain(nameof(CouponIssuer), Ex091_DeterministicByDesign.FindAmbientStateUsers());
    }

    [Fact]
    public void Fitness_A_Type_Reaching_For_The_Ambient_Clock_Is_Reported()
    {
        // Nothing else can tell these two apart: both compile, both work, both produce
        // sensible coupons. Only one of them can be tested at a boundary, and the
        // difference is visible only in the constructor.
        Assert.Contains(nameof(AmbientCouponIssuer), Ex091_DeterministicByDesign.FindAmbientStateUsers());
    }

    /// <summary>Advances on every read, so a second reading is provably a different instant.</summary>
    private sealed class MovingClock(ManualClock inner) : IClock
    {
        public DateTimeOffset UtcNow
        {
            get
            {
                var now = inner.UtcNow;
                inner.Advance(TimeSpan.FromMinutes(1));
                return now;
            }
        }
    }
}
