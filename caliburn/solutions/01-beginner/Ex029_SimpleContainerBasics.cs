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

using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex029_SimpleContainerBasics
{
    public void RegisterSingleton<TService, TImplementation>(SimpleContainer container)
        where TImplementation : class, TService =>
        container.RegisterSingleton(typeof(TService), null, typeof(TImplementation));

    public void RegisterPerRequest<TService, TImplementation>(SimpleContainer container)
        where TImplementation : class, TService =>
        container.RegisterPerRequest(typeof(TService), null, typeof(TImplementation));

    public TService? Resolve<TService>(SimpleContainer container) where TService : class =>
        container.GetInstance(typeof(TService), null) as TService;
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
