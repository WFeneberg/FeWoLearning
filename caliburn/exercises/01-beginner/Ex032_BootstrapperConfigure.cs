// Exercise 032 - Bootstrapper Configure (beginner).
// Goal:   Learn BootstrapperBase: the type a real app subclasses once to wire its container,
//         and see that nothing is wired until something explicitly runs that wiring.
// Drills: overriding Configure() to register a service; overriding GetInstance/GetAllInstances/
//         BuildUp so the container actually answers for the bootstrapper; and that Initialize()
//         - not construction - is what runs Configure() and installs those three overrides
//         behind the global IoC facade.
// Passes: dotnet test --filter FullyQualifiedName~Ex032_
//
// Measured on this machine (Caliburn.Micro 5.0.258): BootstrapperBase's only constructor is
// BootstrapperBase(bool useApplication = true); passing false (below) is what makes it usable
// with no WPF Application at all - the headless path this whole exercise runs on. Constructing
// a bootstrapper does NOT run Configure() - nothing is registered yet. Calling Initialize()
// does run it, and afterward IoC.Get/IoC.GetAll/IoC.BuildUp are answered by THIS bootstrapper's
// GetInstance/GetAllInstances/BuildUp overrides, not whatever was installed before - a process-
// global mutation, but a safe one here because CaliburnCoreContext re-installs its own IoC
// delegates at the start of every test. Calling Initialize() a second or third time does not
// run Configure() again - it is idempotent.

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

    protected override void Configure() =>
        throw new NotImplementedException("TODO: Ex032 - count the call, then register IEx032_Greeter as a singleton backed by Ex032_Greeter in Container");

    protected override object GetInstance(Type service, string key) =>
        throw new NotImplementedException("TODO: Ex032 - resolve through Container");

    protected override IEnumerable<object> GetAllInstances(Type service) =>
        throw new NotImplementedException("TODO: Ex032 - resolve every registration through Container");

    protected override void BuildUp(object instance) =>
        throw new NotImplementedException("TODO: Ex032 - delegate property injection to Container");
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
