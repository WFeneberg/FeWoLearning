using System.Reflection;

namespace FeWoLearning.Exercises.Advanced;

// Exercise 082 — Minimal DI container (reference solution).
// Registrations map an interface Type to an implementation Type plus a lifetime.
// Resolve(Type) walks the implementation's constructor (the one with the most
// parameters), recursively resolving each parameter from the container so that
// dependencies of dependencies are wired up automatically. Singletons are built
// once, lazily, on first resolve and then cached.
public sealed class SimpleDiContainer
{
    private sealed class Registration
    {
        public required Type ImplementationType { get; init; }
        public required bool Singleton { get; init; }
        public object? Instance { get; set; }
    }

    private readonly Dictionary<Type, Registration> _registrations = new();

    public void Register<TInterface, TImplementation>(bool singleton = false)
        where TInterface : class
        where TImplementation : class, TInterface
    {
        _registrations[typeof(TInterface)] = new Registration
        {
            ImplementationType = typeof(TImplementation),
            Singleton = singleton,
        };
    }

    public bool IsRegistered<TInterface>() where TInterface : class
        => _registrations.ContainsKey(typeof(TInterface));

    public TInterface Resolve<TInterface>() where TInterface : class
        => (TInterface)Resolve(typeof(TInterface));

    private object Resolve(Type interfaceType)
    {
        if (!_registrations.TryGetValue(interfaceType, out var registration))
            throw new InvalidOperationException($"No registration found for '{interfaceType.FullName}'.");

        if (registration.Singleton && registration.Instance is not null)
            return registration.Instance;

        var instance = CreateInstance(registration.ImplementationType);

        if (registration.Singleton)
            registration.Instance = instance;

        return instance;
    }

    private object CreateInstance(Type implementationType)
    {
        var constructor = implementationType
            .GetConstructors()
            .OrderByDescending(c => c.GetParameters().Length)
            .FirstOrDefault()
            ?? throw new InvalidOperationException($"Type '{implementationType.FullName}' has no public constructor.");

        var parameters = constructor.GetParameters();
        if (parameters.Length == 0)
            return Activator.CreateInstance(implementationType)!;

        var args = new object[parameters.Length];
        for (var i = 0; i < parameters.Length; i++)
        {
            var parameterType = parameters[i].ParameterType;
            args[i] = Resolve(parameterType);
        }

        return constructor.Invoke(args);
    }
}
