using ReactiveUI;
using ReactiveUI.Primitives;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

/// Exercise 046 - InteractionDialog (intermediate).
/// Goal:   A view model must ask its view a yes/no question - "confirm this
///         deletion?" - and wait for the answer, without the view model ever
///         knowing a view exists.
/// Drills: Interaction&lt;TInput, TOutput&gt;, RegisterHandler, the unhandled case.
/// Passes: dotnet test --filter FullyQualifiedName~Ex046_
public class Ex046_InteractionDialogViewModel : ReactiveObject
{
    public Interaction<string, bool> ConfirmDeletion { get; } = new();

    private bool? _lastResult;
    public bool? LastResult
    {
        get => _lastResult;
        private set => this.RaiseAndSetIfChanged(ref _lastResult, value);
    }

    public async Task DeleteAsync()
    {
        var confirmed = await ConfirmDeletion.Handle("Delete this item?").ToTask();
        LastResult = confirmed;
    }
}
