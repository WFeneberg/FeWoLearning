using ReactiveUI;
using Splat;

namespace FeWoLearning.Avalonia.Exercises.Expert;

// Passes: dotnet test --filter FullyQualifiedName~Ex093_
public class Ex093_DependencyInjectionWiring
{
    /// <summary>Given. Do not change.</summary>
    public int TransientBuilds { get; private set; }

    /// <summary>Given. Do not change.</summary>
    public int SingletonBuilds { get; private set; }

    /// <summary>Given. Do not change.</summary>
    public int ConstantBuilds { get; private set; }

    /// <summary>Given. Do not change.</summary>
    protected Ex093_Clock BuildClock() => new(++TransientBuilds);

    /// <summary>Given. Do not change.</summary>
    protected Ex093_Cache BuildCache() => new(++SingletonBuilds);

    /// <summary>Given. Do not change.</summary>
    protected Ex093_Settings BuildSettings() => new(++ConstantBuilds);

    public ModernDependencyResolver Resolver { get; } = new();

    public void Wire()
    {
        // The registrar is the seam ReactiveUI itself registers through, which is
        // why this does not call Splat's Register directly.
        var registrar = new DependencyResolverRegistrar(Resolver);

        registrar.Register(BuildClock);
        registrar.RegisterLazySingleton(BuildCache);
        registrar.RegisterConstant(BuildSettings);
    }

    public TService? Resolve<TService>()
        where TService : class =>
        Resolver.GetService<TService>();
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

/// <summary>Given. Do not change.</summary>
public class Ex093_Missing;
