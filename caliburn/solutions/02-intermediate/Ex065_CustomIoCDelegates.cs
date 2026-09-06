// Exercise 065 - Custom IoC Delegates (intermediate).
// Goal:   Caliburn has no built-in container of its own - IoC.GetInstance, IoC.GetAllInstances
//         and IoC.BuildUp are three plain, settable delegate fields, and IoC.Get<T>()/
//         GetAll<T>()/BuildUp(object) are just a generic facade calling straight through them.
//         Replacing SimpleContainer with any container of your own is exactly this: point those
//         three delegates at your container's own methods. The harness re-establishes all three
//         per test (it needs IoC initialized even for coroutine-only exercises), so this is
//         cleaned up automatically - no extra reset to write here.
// Drills: writing a small container's GetInstance/GetAllInstances/BuildUp, and wiring
//         IoC's three delegates to it - then proving both the facade (IoC.Get<T>/GetAll<T>) and
//         a fresh Install() genuinely REPLACE what was there before, rather than merely adding
//         to it.
// Passes: dotnet test --filter FullyQualifiedName~Ex065_
//
// A trap worth stating plainly: the harness's OWN IoC.GetInstance ends with
// "?? Activator.CreateInstance(service)", so under the harness's own delegates,
// IoC.Get<SomeConcreteType>() returns a NEW instance even for a type nobody ever registered.
// That fallback is the harness's, not Caliburn's - it does not apply here at all once THIS
// exercise's own delegates are installed, and proving "unregistered resolves to null" only
// works after they are.

using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Intermediate;

/// <summary>A minimal container, deliberately not SimpleContainer: register an instance against
/// a service type, resolve it back, or get nothing at all for a type nobody registered - no
/// Activator fallback anywhere in here.</summary>
public class Ex065_MiniContainer
{
    readonly Dictionary<Type, object> _instances = [];

    /// <summary>How many times BuildUp was asked to wire up an instance - proves IoC.BuildUp
    /// really reached THIS container, not the harness's SimpleContainer-backed one.</summary>
    public int BuildUpCallCount { get; private set; }

    public void Register(Type serviceType, object instance) => _instances[serviceType] = instance;

    /// <summary>Resolves whatever was registered for serviceType, or null if nothing was -
    /// unlike the harness's own container, there is no Activator.CreateInstance fallback here.</summary>
    public object? GetInstance(Type serviceType, string? key) =>
        _instances.TryGetValue(serviceType, out var instance) ? instance : null;

    /// <summary>Every registered instance whose own type IS ASSIGNABLE TO serviceType - a
    /// container has to answer this for interfaces and base types, not just exact matches.</summary>
    public IEnumerable<object> GetAllInstances(Type serviceType) =>
        _instances.Values.Where(instance => serviceType.IsInstanceOfType(instance));

    public void BuildUp(object instance) => BuildUpCallCount++;
}

public class Ex065_CustomIoCDelegates
{
    /// <summary>Points IoC's three delegates at container's own methods, replacing whatever was
    /// installed before entirely - not composing with it.</summary>
    public void Install(Ex065_MiniContainer container)
    {
        IoC.GetInstance = container.GetInstance;
        IoC.GetAllInstances = container.GetAllInstances;
        IoC.BuildUp = container.BuildUp;
    }
}

public interface Ex065_IGreeter
{
    string Greet();
}

public class Ex065_EnglishGreeter : Ex065_IGreeter
{
    public string Greet() => "Hello";
}

public class Ex065_FrenchGreeter : Ex065_IGreeter
{
    public string Greet() => "Bonjour";
}

/// <summary>A type deliberately never registered with anything in this exercise - used to prove
/// the "unregistered resolves to null" contrast with the harness's own Activator fallback.</summary>
public class Ex065_UnregisteredThing { }
