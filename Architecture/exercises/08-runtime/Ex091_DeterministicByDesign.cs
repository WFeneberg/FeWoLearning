using System.Reflection;
using FeWoLearning.Architecture.Exercises.Support;

namespace FeWoLearning.Architecture.Exercises.Runtime.Ex091;

public interface IIdSource
{
    string Next();
}

public interface IRandomSource
{
    /// <summary>A value in [0, 1).</summary>
    double NextDouble();
}

public sealed record Coupon(string Code, DateTimeOffset IssuedAt, DateTimeOffset ExpiresAt, decimal DiscountFraction);

/// <summary>
/// The ambient versions, shipped so the fitness check has something to catch. Nothing in
/// this exercise may reach them.
/// </summary>
public static class Ambient
{
    public static DateTimeOffset Now => DateTimeOffset.UtcNow;

    public static string NewId() => Guid.NewGuid().ToString("N");
}

/// <summary>A deliberate violation for the fitness check: reaches for the ambient clock.</summary>
public sealed class AmbientCouponIssuer
{
    public Coupon Issue(decimal discount) =>
        new(Ambient.NewId(), Ambient.Now, Ambient.Now.AddDays(30), discount);
}

// Exercise 091 — DeterministicByDesign (runtime).
// Goal:   Learn, explicitly, the idiom every timing exercise in this track has been
//         quietly using since 015: time, identity and randomness are DEPENDENCIES.
// Drills: ambient state as a hidden input, testability as a design property, fitness checks.
// Passes: issuing  - the coupon's code, timestamps and discount all come from the injected
//                    sources, so a test names them rather than guessing.
//         repeatable- issuing twice with the same sources produces the same coupon. That is
//                    the whole property; a type that reads DateTime.UtcNow cannot have it.
//         jitter   - the discount is the base rate scaled by the random source, and both
//                    ends of the range are checkable because the source is a parameter.
//         THE ONE   - the fitness check reports AmbientCouponIssuer and does NOT report
//                    CouponIssuer. Nothing else can tell them apart: both compile, both
//                    work, and only one of them can be tested at a boundary.
//
// This is the exercise the rest of the track presumed. Every row that advanced a
// ManualClock - the rate limiter, the cache TTL, the lease, the circuit breaker, the
// backoff - was only testable because the type took its clock as a parameter. Written the
// ordinary way, each of those tests would need Thread.Sleep, and the suite would take an
// hour and still be flaky.
//
// The general shape: a hidden input is one you cannot vary, and the three that hide most
// often are the clock, the id generator and the random source, because none of them looks
// like an input. A test that cannot set them has to either sleep, or assert loosely enough
// to be worthless. The cost is three constructor parameters; the return is every timing
// fact in this repository.
public sealed class CouponIssuer(IClock clock, IIdSource ids, IRandomSource random)
{
    public const decimal BaseDiscount = 0.10m;

    /// <summary>
    /// Issue a coupon valid for <paramref name="validFor"/>, with a discount of
    /// BaseDiscount scaled by 1 + the random source's value.
    /// </summary>
    public Coupon Issue(TimeSpan validFor) =>
        throw new NotImplementedException(
            "TODO: Ex091 - take the code from ids, both timestamps from the clock, and the discount from BaseDiscount scaled by 1 + random");
}

public static class Ex091_DeterministicByDesign
{
    /// <summary>
    /// Report every type in a namespace ending ".Ex091" that PRODUCES a Coupon - a
    /// time-stamped value - without receiving a clock as a constructor dependency. A type
    /// that cannot be handed a clock is reading one from somewhere, and that somewhere is
    /// ambient.
    ///
    /// Structural rather than an IL scan on purpose. Walking method bodies for calls into
    /// Ambient would be more thorough and is what a real fitness rule for this would do;
    /// it also depends on IL layout and metadata token resolution, which is a lot of
    /// fragility to take on for a rule the constructor signature already answers. The
    /// question "can this type be given a clock" is the one that decides whether it can be
    /// tested at a boundary.
    /// </summary>
    public static IReadOnlyList<string> FindAmbientStateUsers() =>
        throw new NotImplementedException(
            "TODO: Ex091 - report Ex091 types that return a Coupon from some method but take no IClock in any constructor");
}
