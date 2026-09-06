using ReactiveUI;
using Splat;

namespace FeWoLearning.Avalonia.Exercises.Expert;

/// Exercise 093 - DependencyInjectionWiring (expert).
/// Goal:   Put a container behind ReactiveUI's own resolver and register three
///         services with three different LIFETIMES - transient, lazy singleton and
///         constant - then see which of them hands back the same instance twice
///         and which does not.
/// Drills: Splat's IMutableDependencyResolver, ReactiveUI's
///         DependencyResolverRegistrar, Register versus RegisterLazySingleton
///         versus RegisterConstant, GetService for an unregistered type.
/// Passes: dotnet test --filter FullyQualifiedName~Ex093_
///
/// ReactiveUI 24's resolver is Splat's, reached through Splat.Locator - and
/// Locator.Current is PROCESS-GLOBAL. This exercise deliberately does not touch
/// it: it builds its own ModernDependencyResolver instead, which is public and has
/// a parameterless constructor, so nothing leaks into the next test in a serial
/// suite. Wiring that resolver into Locator via Locator.SetLocator is the one
/// extra line a real application adds, and it is left out here on purpose.
///
/// Measured lifetimes, which is what the test pins down:
///   Register              -> a NEW instance per resolve, factory called each time
///   RegisterLazySingleton -> the SAME instance, factory called exactly once
///   RegisterConstant      -> the same instance, built when you register it
///   an unregistered type  -> null, not an exception
public class Ex093_DependencyInjectionWiring
{
    /// <summary>Given. Do not change. Counts how often each factory ran.</summary>
    public int TransientBuilds { get; private set; }

    /// <summary>Given. Do not change.</summary>
    public int SingletonBuilds { get; private set; }

    /// <summary>Given. Do not change.</summary>
    public int ConstantBuilds { get; private set; }

    /// <summary>Given. Do not change. Call from the transient factory.</summary>
    protected Ex093_Clock BuildClock() => new(++TransientBuilds);

    /// <summary>Given. Do not change. Call from the lazy-singleton factory.</summary>
    protected Ex093_Cache BuildCache() => new(++SingletonBuilds);

    /// <summary>Given. Do not change. Call from the constant factory.</summary>
    protected Ex093_Settings BuildSettings() => new(++ConstantBuilds);

    /// <summary>
    /// The container. Built once by Wire and read by the test through
    /// Resolve, so the same resolver answers every question.
    /// </summary>
    public ModernDependencyResolver Resolver { get; } = new();

    /// <summary>
    /// Register the three services on Resolver through ReactiveUI's
    /// DependencyResolverRegistrar - not by calling Splat's Register directly,
    /// because the registrar is the seam ReactiveUI itself goes through:
    ///
    ///   Ex093_Clock     transient       (BuildClock)
    ///   Ex093_Cache     lazy singleton  (BuildCache)
    ///   Ex093_Settings  constant        (BuildSettings)
    ///
    /// Nothing else. Ex093_Missing must stay unregistered, because the test checks
    /// what an unregistered type resolves to.
    /// </summary>
    public void Wire() =>
        throw new NotImplementedException(
            "TODO: Ex093 - new DependencyResolverRegistrar(Resolver), then Register, " +
            "RegisterLazySingleton and RegisterConstant for the three services, each " +
            "using its Build... method as the factory");

    /// <summary>
    /// Whatever the container has for <typeparamref name="TService"/>, or null.
    ///
    /// Go through the resolver rather than keeping a dictionary of your own: the
    /// point of the exercise is that the container is the single source of truth.
    /// </summary>
    public TService? Resolve<TService>()
        where TService : class =>
        throw new NotImplementedException(
            "TODO: Ex093 - ask Resolver for a TService");
}

/// <summary>Given. Do not change.</summary>
public class Ex093_Clock(int build)
{
    public int Build { get; } = build;
}

/// <summary>Given. Do not change.</summary>
public class Ex093_Cache(int build)
{
    public int Build { get; } = build;
}

/// <summary>Given. Do not change.</summary>
public class Ex093_Settings(int build)
{
    public int Build { get; } = build;
}

/// <summary>Given. Do not change. Never registered, on purpose.</summary>
public class Ex093_Missing;
