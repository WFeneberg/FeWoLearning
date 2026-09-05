// Exercise 031 - IoC Facade (beginner).
// Goal:   Learn the static Caliburn.Micro.IoC facade: three static members that forward to
//         whatever GetInstance/GetAllInstances/BuildUp delegates are currently installed -
//         IoC itself holds no container, it is just the well-known front door to one.
// Drills: IoC.Get<T>() to resolve one instance, IoC.GetAll<T>() to resolve every current
//         registration (not just the first), and IoC.BuildUp(instance) to run property
//         injection over an object that was not built by the container at all.
// Passes: dotnet test --filter FullyQualifiedName~Ex031_
//
// Measured on this machine (Caliburn.Micro 5.0.258), through a SimpleContainer wired behind
// IoC (exactly how CaliburnCoreContext wires its own Container): IoC.Get<IService>() returned
// the registered singleton, reference-equal to what the container itself resolves; requesting
// it PerRequest instead returned a different instance on every call, proving IoC.Get truly asks
// the container afresh rather than caching its first answer. IoC.GetAll<IService>() came back
// empty before anything was registered, and counted every registration afterwards - 2 for 2
// separate registrations of the same service, not just the newest one.
//
// IoC.BuildUp(instance) delegates straight to the installed container's BuildUp, which walks an
// object's INTERFACE-typed properties - setter accessibility does not matter, a private setter is
// injected exactly like a public one - and injects whichever ones have a currently-registered
// type, measured to do this even with SimpleContainer.EnablePropertyInjection at its default of
// false, so that flag is not the gate here. A CONCRETE-typed property is never touched, even when
// that exact concrete type is itself registered, and neither are fields - only interface-typed
// PROPERTIES are candidates at all. A property whose interface type was never registered is left
// exactly as it was, not forced to some placeholder.
//
// A trap deliberately NOT exercised here: the test harness's own IoC.GetInstance falls back to
// Activator.CreateInstance for anything unregistered, so "resolve something unregistered" does
// not reliably come back null through IoC the way it does through SimpleContainer.GetInstance
// directly (that is ex029's lesson). GetAllInstances has no such fallback, which is why the
// "nothing registered" case below is asserted through GetAll, not Get.

using System.Collections.Generic;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex031_IoCFacade
{
    /// <summary>Resolves TService through the currently installed IoC delegates.</summary>
    public TService Resolve<TService>() where TService : class =>
        throw new NotImplementedException("TODO: Ex031 - resolve via IoC.Get");

    /// <summary>Resolves every current registration for TService through the currently installed IoC delegates.</summary>
    public IEnumerable<TService> ResolveAll<TService>() where TService : class =>
        throw new NotImplementedException("TODO: Ex031 - resolve every registration via IoC.GetAll");

    /// <summary>Runs property injection over an already-constructed instance through the currently installed IoC delegates.</summary>
    public void Inject(object instance) =>
        throw new NotImplementedException("TODO: Ex031 - run property injection via IoC.BuildUp");
}

/// <summary>A service with an identity you can compare across resolutions.</summary>
public interface IEx031_Thing
{
    Guid Id { get; }
}

public class Ex031_Thing : IEx031_Thing
{
    public Guid Id { get; } = Guid.NewGuid();
}

/// <summary>A second, unrelated service - proves resolution is not just returning the same thing regardless of type.</summary>
public interface IEx031_Other
{
    Guid Id { get; }
}

public class Ex031_Other : IEx031_Other
{
    public Guid Id { get; } = Guid.NewGuid();
}

/// <summary>
/// Three settable properties: an interface type that gets registered, an interface type that
/// never does, and a CONCRETE type registered under its own name - BuildUp only ever considers
/// interface-typed properties, so the concrete one stays null no matter what is registered for it.
/// </summary>
public class Ex031_PropertyConsumer
{
    public IEx031_Thing? Thing { get; set; }
    public IEx031_Other? Other { get; set; }
    public Ex031_Thing? ConcreteThing { get; set; }
}
