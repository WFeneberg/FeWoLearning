namespace FeWoLearning.Architecture.Exercises.Desktop.Ex020;

/// <summary>
/// Three outcomes, not two. "Cancel" is not "No" - it means the user does not want the
/// operation to happen at all, and a bool-shaped port cannot say it.
/// </summary>
public enum SaveChoice
{
    Save,
    Discard,
    Cancel,
}

/// <summary>
/// The port. Note what is absent: no window, no owner handle, no framework type. That
/// is what makes the caller testable, and it is the entire point of the abstraction.
/// </summary>
public interface IDialogService
{
    SaveChoice AskToSave(string documentName);
}

public sealed class DocumentStore
{
    public List<string> Saved { get; } = [];
    public List<string> Discarded { get; } = [];
}

// Exercise 020 — DialogServiceAbstraction (desktop).
// Goal:   Drive a real branching decision through a port, so the decision is testable
//         without ever opening a window.
// Drills: modal interaction as a port, three-way results, testability.
// Passes: Save    - the document is saved and TryClose returns true.
//         Discard - the document is discarded, NOT saved, and TryClose returns true.
//         Cancel  - nothing is saved, nothing is discarded, and TryClose returns FALSE.
//         always  - the dialog is asked exactly once, with the document's name.
//
// Cancel is why the port returns an enum. Modelled as a bool, "Discard" and "Cancel"
// collapse into the same false, and the window closes on a user who asked it not to -
// losing exactly the work they were trying to keep.
public sealed class DocumentCloser(IDialogService dialogs, DocumentStore store)
{
    /// <summary>Ask, act on the answer, and report whether the document may close.</summary>
    public bool TryClose(string documentName) =>
        throw new NotImplementedException(
            "TODO: Ex020 - ask the dialog service once and branch three ways: save and close, discard and close, or do nothing and stay open");
}
