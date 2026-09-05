using ReactiveUI;
using ReactiveUI.Primitives;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

/// Exercise 046 - InteractionDialog (intermediate).
/// Goal:   A view model must ask its view a yes/no question - "confirm this
///         deletion?" - and wait for the answer, without the view model ever
///         knowing a view exists. Interaction&lt;TInput, TOutput&gt; is exactly this:
///         Handle(input) raises the question and returns an observable of the
///         answer; RegisterHandler is how a view (or, here, a test standing in
///         for one) answers it.
/// Drills: Interaction&lt;TInput, TOutput&gt;, RegisterHandler, the unhandled case.
///
/// Measured on this machine: calling Handle(...) with NO handler registered
/// throws UnhandledInteractionException&lt;TInput, TOutput&gt; - not silently, not as
/// a null result. That is this exercise's best discriminator: a solution that
/// fakes the outcome by setting LastResult directly, without ever routing
/// through the Interaction, never throws that exception and is caught by the
/// first test below.
/// Passes: dotnet test --filter FullyQualifiedName~Ex046_
public class Ex046_InteractionDialogViewModel : ReactiveObject
{
    /// <summary>Given. Do not change.</summary>
    public Interaction<string, bool> ConfirmDeletion { get; } = new();

    private bool? _lastResult;

    /// <summary>Given. Do not change.</summary>
    public bool? LastResult
    {
        get => _lastResult;
        private set => this.RaiseAndSetIfChanged(ref _lastResult, value);
    }

    /// <summary>
    /// TODO:
    ///   var confirmed = await ConfirmDeletion.Handle("Delete this item?").ToTask();
    ///   LastResult = confirmed;
    /// (needs "using ReactiveUI.Primitives;" for ToTask() - see the standing facts
    /// in the track design doc.)
    /// </summary>
    public Task DeleteAsync()
    {
        throw new NotImplementedException(
            "TODO: Ex046 - await ConfirmDeletion.Handle(\"Delete this item?\").ToTask(); " +
            "then LastResult = the awaited answer. Route the question through " +
            "ConfirmDeletion - do not set LastResult directly.");
    }
}
