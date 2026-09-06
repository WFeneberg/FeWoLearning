// Exercise 048 - Dialog Result (intermediate).
// Goal:   TryCloseAsync(bool?) flowing back to whoever awaited ShowDialogAsync. A bool? SUGGESTS
//         three outcomes, but there are only two you can tell apart from the return value alone:
//         TryCloseAsync(true) resolves ShowDialogAsync to true, TryCloseAsync(false) resolves it
//         to false, and TryCloseAsync(null) ALSO resolves it to false - not null. WPF's own
//         Window.ShowDialog() returns false whenever DialogResult was never explicitly set to
//         true, and that is exactly what happens when a view model closes with null: there is no
//         way to tell "the user explicitly declined" from "the dialog was merely dismissed"
//         by the ShowDialogAsync return value alone - a real application needs its own state on
//         the view model for that distinction.
// Drills: three near-identical methods whose only difference is which bool? they hand
//         TryCloseAsync - the exercise IS that difference, not the plumbing around it.
// Passes: dotnet test --filter FullyQualifiedName~Ex048_

using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Intermediate;

/// <summary>A dialog view model offering three ways to close, so a caller awaiting
/// ShowDialogAsync can be shown what each one actually resolves to.</summary>
public class Ex048_ConfirmableDialogVm : Screen
{
    public Task ConfirmAsync() => TryCloseAsync(true);

    public Task DeclineAsync() => TryCloseAsync(false);

    public Task DismissAsync() => TryCloseAsync(null);
}
