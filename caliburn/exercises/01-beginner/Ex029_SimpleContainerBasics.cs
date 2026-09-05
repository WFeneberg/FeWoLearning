// Exercise 029 - Simple Container Basics (beginner).
// Goal:   Learn Caliburn's own tiny IoC container: SimpleContainer, its two lifetime styles
//         (Singleton, PerRequest), and that asking it for something it has never heard of comes
//         back null rather than throwing.
// Drills: SimpleContainer.RegisterSingleton/RegisterPerRequest/GetInstance - the raw instance
//         methods (SimpleContainer also exposes friendlier Singleton<TService,TImpl>()/
//         PerRequest<TService,TImpl>() extension methods in Caliburn.Micro.ContainerExtensions;
//         this exercise uses the raw instance methods instead, to show what those extensions
//         actually call underneath); that constructor injection only kicks in for a type that is
//         ITSELF registered - GetInstance does not silently reflection-construct an arbitrary
//         unregistered concrete type just because its constructor happens to be satisfiable.
// Passes: dotnet test --filter FullyQualifiedName~Ex029_
//
// Measured on this machine (Caliburn.Micro 5.0.258): registering a service as
// RegisterSingleton(typeof(IThing), null, typeof(Thing)) and resolving it twice via
// GetInstance(typeof(IThing), null) returns the SAME instance both times; registering the same
// pair via RegisterPerRequest instead returns a DIFFERENT instance every call. Resolving a
// service type nothing was ever registered for returns null - GetInstance does not throw for an
// unknown service, so a typo'd registration fails silently at resolution, not loudly at startup.
//
// Constructor injection is real, but narrower than it looks: registering a consumer type whose
// constructor takes IThing via RegisterPerRequest (so the container itself builds it) resolves
// that consumer with the registered IThing wired into its constructor automatically. But asking
// GetInstance for that SAME consumer type WITHOUT ever registering the consumer itself measured
// null, exactly like any other unregistered service - SimpleContainer only builds a type through
// its constructor once that type is itself a registered service; it is not a general-purpose
// "construct anything reflectively" resolver.

using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex029_SimpleContainerBasics
{
    /// <summary>Registers TService as a SINGLETON backed by TImplementation - every resolution afterwards returns the exact same instance.</summary>
    public void RegisterSingleton<TService, TImplementation>(SimpleContainer container)
        where TImplementation : class, TService =>
        throw new NotImplementedException("TODO: Ex029 - register via container.RegisterSingleton");

    /// <summary>Registers TService as PER-REQUEST backed by TImplementation - every resolution afterwards returns a fresh instance.</summary>
    public void RegisterPerRequest<TService, TImplementation>(SimpleContainer container)
        where TImplementation : class, TService =>
        throw new NotImplementedException("TODO: Ex029 - register via container.RegisterPerRequest");

    /// <summary>Resolves TService from the container - null, not a thrown exception, if nothing is registered for it.</summary>
    public TService? Resolve<TService>(SimpleContainer container) where TService : class =>
        throw new NotImplementedException("TODO: Ex029 - resolve via container.GetInstance");
}

/// <summary>A service with an identity you can compare across resolutions.</summary>
public interface IEx029_Thing
{
    Guid Id { get; }
}

public class Ex029_Thing : IEx029_Thing
{
    public Guid Id { get; } = Guid.NewGuid();
}

/// <summary>A type whose constructor depends on IEx029_Thing - proves constructor injection once THIS type is itself registered.</summary>
public class Ex029_ThingConsumer
{
    public IEx029_Thing Thing { get; }

    public Ex029_ThingConsumer(IEx029_Thing thing) => Thing = thing;
}
