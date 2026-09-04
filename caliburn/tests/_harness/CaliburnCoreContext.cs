using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises;

namespace FeWoLearning.Caliburn.Tests;

/// <summary>
/// Base class for exercises with no view. Caliburn is configured through process-global
/// statics; a real app sets them once in a Bootstrapper, but a test run has to
/// re-establish them for EVERY test, because the previous test left its own behind.
/// </summary>
public abstract class CaliburnCoreContext
{
    protected SimpleContainer Container { get; } = new();

    protected CaliburnCoreContext()
    {
        // Reset to the inline provider. A previous [WpfFact] may have installed the XAML
        // one, whose captured Dispatcher belongs to an STA thread that no longer pumps --
        // NotifyOfPropertyChange would then block until the call is cancelled, surfacing
        // as a TaskCanceledException from deep inside PropertyChangedBase.
        PlatformProvider.Current = new DefaultPlatformProvider();

        // The ViewLocator searches these assemblies. TrackMarker names whichever content
        // assembly this run is built against; the test assembly carries the harness's own
        // probe view.
        AssemblySource.Instance.Clear();
        AssemblySource.Instance.Add(typeof(TrackMarker).Assembly);
        AssemblySource.Instance.Add(typeof(CaliburnCoreContext).Assembly);

        // Not optional even with no UI at all: Coroutine.BeginExecute calls IoC.BuildUp,
        // so an otherwise pure-core coroutine test throws "IoC is not initialized".
        IoC.GetInstance = (service, key) =>
            Container.GetInstance(service, key) ?? Activator.CreateInstance(service)!;
        IoC.GetAllInstances = service => Container.GetAllInstances(service, null);
        IoC.BuildUp = Container.BuildUp;
    }
}
