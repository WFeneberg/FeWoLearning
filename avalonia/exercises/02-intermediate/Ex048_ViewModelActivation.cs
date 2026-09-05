using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

/// Exercise 048 - ViewModelActivation (intermediate).
/// Goal:   Hook into a view model's activation lifecycle - run setup exactly when
///         a view showing it becomes active, and genuinely dispose that setup
///         when the view goes away - via IActivatableViewModel + WhenActivated,
///         not a constructor-side counter.
/// Drills: IActivatableViewModel, ViewModelActivator, WhenActivated disposal.
///
/// Measured on this machine: a view showing this view model in a headless window
/// (Ex048_ViewModelActivationTests.HostView, in the test file - not part of this
/// exercise) drives ActivationCount from 0 to 1 on Show(), and flips
/// DisposableWasDisposed to true only once that view is later removed from the
/// visual tree. Both only happen through the real WhenActivated/Activator
/// machinery - there is no view-side wiring available to a cheat that fakes it
/// from outside this file, since only this class is graded.
///
/// ReactiveUI.Primitives.Disposables is a separate assembly with no compile-time
/// reference from this project, so this exercise supplies its own tiny IDisposable
/// rather than reach for CompositeDisposable - see the track design doc.
/// Passes: dotnet test --filter FullyQualifiedName~Ex048_
public class Ex048_ViewModelActivationViewModel : ReactiveObject, IActivatableViewModel
{
    public ViewModelActivator Activator { get; } = new();

    public int ActivationCount { get; private set; }

    public bool DisposableWasDisposed { get; private set; }

    /// <summary>
    /// TODO:
    ///   this.WhenActivated((Action&lt;IDisposable&gt; register) =>
    ///   {
    ///       ActivationCount++;
    ///       register(new SomeDisposable(() => DisposableWasDisposed = true));
    ///   });
    /// (The explicit Action&lt;IDisposable&gt; parameter type is required - WhenActivated
    /// on IActivatableViewModel has two ambiguous overloads otherwise, per the track
    /// design doc.)
    /// </summary>
    public Ex048_ViewModelActivationViewModel()
    {
        throw new NotImplementedException(
            "TODO: Ex048 - this.WhenActivated((Action<IDisposable> register) => { " +
            "ActivationCount++; register(a disposable that flips DisposableWasDisposed " +
            "to true when Dispose() runs); });");
    }
}
