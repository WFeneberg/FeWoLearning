// Exercise 032 - Bootstrapper Configure (beginner).
// Goal:   Learn BootstrapperBase: the type a real app subclasses once to wire its container,
//         and see that nothing is wired until something explicitly runs that wiring.
// Drills: overriding Configure() to register a service; overriding GetInstance/GetAllInstances/
//         BuildUp so the container actually answers for the bootstrapper; and that Initialize()
//         - not construction - is what runs Configure() and installs those three overrides
//         behind the global IoC facade.
// Passes: dotnet test --filter FullyQualifiedName~Ex032_

using System.Collections.Generic;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex032_BootstrapperConfigure : BootstrapperBase
{
    public Ex032_BootstrapperConfigure() : base(useApplication: false)
    {
    }

    /// <summary>The container this bootstrapper wires - exposed so tests can inspect it directly.</summary>
    public SimpleContainer Container { get; } = new();

    /// <summary>How many times Configure() has actually run.</summary>
    public int ConfigureCallCount { get; private set; }

    protected override void Configure()
    {
        ConfigureCallCount++;
        Container.RegisterSingleton(typeof(IEx032_Greeter), null, typeof(Ex032_Greeter));
    }

    protected override object GetInstance(Type service, string key) =>
        Container.GetInstance(service, key);

    protected override IEnumerable<object> GetAllInstances(Type service) =>
        Container.GetAllInstances(service, null);

    protected override void BuildUp(object instance) =>
        Container.BuildUp(instance);
}

/// <summary>A trivial service Configure() must register - proves the wiring, not the greeting.</summary>
public interface IEx032_Greeter
{
    string Greet();
}

public class Ex032_Greeter : IEx032_Greeter
{
    public string Greet() => "hello";
}

/// <summary>A settable property whose type Configure() registers - proves BuildUp actually injects.</summary>
public class Ex032_GreeterConsumer
{
    public IEx032_Greeter? Greeter { get; set; }
}
