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

// Exercise 091 — DeterministicByDesign (reference solution).
public sealed class CouponIssuer(IClock clock, IIdSource ids, IRandomSource random)
{
    public const decimal BaseDiscount = 0.10m;

    public Coupon Issue(TimeSpan validFor)
    {
        // ONE read of the clock, reused. Two reads of DateTimeOffset.UtcNow can straddle a
        // tick, and then IssuedAt and the basis for ExpiresAt disagree by a millisecond
        // that nobody will ever reproduce.
        var now = clock.UtcNow;

        return new Coupon(
            ids.Next(),
            now,
            now + validFor,
            BaseDiscount * (decimal)(1 + random.NextDouble()));
    }
}

public static class Ex091_DeterministicByDesign
{
    private const string NamespaceSuffix = ".Ex091";

    public static IReadOnlyList<string> FindAmbientStateUsers()
    {
        var assembly = typeof(CouponIssuer).Assembly;
        var offenders = new List<string>();

        foreach (var type in assembly.GetTypes())
        {
            if (type.Namespace?.EndsWith(NamespaceSuffix, StringComparison.Ordinal) != true || !type.IsClass)
                continue;

            const BindingFlags members = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

            // Does it produce a time-stamped value at all?
            if (!type.GetMethods(members).Any(m => m.ReturnType == typeof(Coupon)))
                continue;

            // Can it be handed a clock? A type that cannot is reading one from somewhere,
            // and that somewhere is ambient - which is precisely what makes it untestable
            // at a boundary.
            var takesAClock = type.GetConstructors()
                .Any(c => c.GetParameters().Any(p => p.ParameterType == typeof(IClock)));

            if (!takesAClock)
                offenders.Add(type.Name);
        }

        offenders.Sort(StringComparer.Ordinal);
        return offenders;
    }
}
