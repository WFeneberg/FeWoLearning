// Exercise 065 - Custom IoC Delegates (intermediate).
// Goal:   Caliburn's IoC is not itself a container - it is three plain, settable delegate fields
//         (GetInstance, GetAllInstances, BuildUp), with IoC.Get<T>()/GetAll<T>()/BuildUp(object)
//         as a thin generic facade over them. Caliburn does ship SimpleContainer (ex029/ex030),
//         but nothing binds IoC to it: replacing it with any container of your own is exactly
//         this - point those three delegates at your container's own methods. The harness
//         re-establishes all three per test (it needs IoC initialized even for coroutine-only
//         exercises), so this is cleaned up automatically - no extra reset to write here.
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
        throw new NotImplementedException("TODO: Ex065 - look serviceType up in the registry, or null if nothing is registered for it");

    /// <summary>Every registered instance whose own type IS ASSIGNABLE TO serviceType - a
    /// container has to answer this for interfaces and base types, not just exact matches.</summary>
    public IEnumerable<object> GetAllInstances(Type serviceType) =>
        throw new NotImplementedException("TODO: Ex065 - every registered instance assignable to serviceType");

    public void BuildUp(object instance) =>
        throw new NotImplementedException("TODO: Ex065 - increment BuildUpCallCount");
}

public class Ex065_CustomIoCDelegates
{
    /// <summary>Points IoC's three delegates at container's own methods, replacing whatever was
    /// installed before entirely - not composing with it.</summary>
    public void Install(Ex065_MiniContainer container) =>
        throw new NotImplementedException("TODO: Ex065 - point IoC's three delegates at this container's matching methods");
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

/// <summary>Never registered with anything in the "unregistered resolves to null" test, used
/// there to prove that contrast with the harness's own Activator fallback - but registered
/// under its own type elsewhere, as the assignability negative control for GetAllInstances
/// (structurally incapable of implementing Ex065_IGreeter, so it can never sneak into a
/// GetAll&lt;Ex065_IGreeter&gt;() result).</summary>
public class Ex065_UnregisteredThing { }
