namespace FeWoLearning.Exercises.Advanced;

// Exercise 082 — Minimal DI container (advanced).
// Goal:   Build a tiny dependency-injection container that maps an interface to a
//         concrete implementation, resolves instances (including constructor
//         injection of other registered dependencies), and supports both
//         "transient" (new instance per resolve) and "singleton" (one shared
//         instance) lifetimes.
// Drills: reflection, generics, constructor injection, lifetime management.
public sealed class SimpleDiContainer
{
    public void Register<TInterface, TImplementation>(bool singleton = false)
        where TInterface : class
        where TImplementation : class, TInterface
        => throw new NotImplementedException();

    public bool IsRegistered<TInterface>() where TInterface : class
        => throw new NotImplementedException();

    public TInterface Resolve<TInterface>() where TInterface : class
        => throw new NotImplementedException();
}
