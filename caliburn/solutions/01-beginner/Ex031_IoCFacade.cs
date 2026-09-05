// Exercise 031 - IoC Facade (beginner).
// Goal:   Learn the static Caliburn.Micro.IoC facade: three static members that forward to
//         whatever GetInstance/GetAllInstances/BuildUp delegates are currently installed -
//         IoC itself holds no container, it is just the well-known front door to one.
// Drills: IoC.Get<T>() to resolve one instance, IoC.GetAll<T>() to resolve every current
//         registration (not just the first), and IoC.BuildUp(instance) to run property
//         injection over an object that was not built by the container at all.
// Passes: dotnet test --filter FullyQualifiedName~Ex031_

using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex031_IoCFacade
{
    /// <summary>Resolves TService through the currently installed IoC delegates.</summary>
    public TService Resolve<TService>() where TService : class =>
        IoC.Get<TService>();

    /// <summary>Resolves every current registration for TService through the currently installed IoC delegates.</summary>
    public IEnumerable<TService> ResolveAll<TService>() where TService : class =>
        IoC.GetAll<TService>().ToList();

    /// <summary>Runs property injection over an already-constructed instance through the currently installed IoC delegates.</summary>
    public void Inject(object instance) =>
        IoC.BuildUp(instance);
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

/// <summary>Two settable properties: one whose service type gets registered, one whose never does.</summary>
public class Ex031_PropertyConsumer
{
    public IEx031_Thing? Thing { get; set; }
    public IEx031_Other? Other { get; set; }
}
