using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

/// Exercise 048 - ViewModelActivation (intermediate).
/// Goal:   Hook into a view model's activation lifecycle via IActivatableViewModel
///         + WhenActivated, and genuinely dispose setup on deactivation.
/// Drills: IActivatableViewModel, ViewModelActivator, WhenActivated disposal.
/// Passes: dotnet test --filter FullyQualifiedName~Ex048_
public class Ex048_ViewModelActivationViewModel : ReactiveObject, IActivatableViewModel
{
    public ViewModelActivator Activator { get; } = new();

    public int ActivationCount { get; private set; }

    public bool DisposableWasDisposed { get; private set; }

    public Ex048_ViewModelActivationViewModel()
    {
        this.WhenActivated((Action<IDisposable> register) =>
        {
            ActivationCount++;
            register(new ActivationScopeDisposable(() => DisposableWasDisposed = true));
        });
    }

    private sealed class ActivationScopeDisposable(Action onDispose) : IDisposable
    {
        public void Dispose() => onDispose();
    }
}
