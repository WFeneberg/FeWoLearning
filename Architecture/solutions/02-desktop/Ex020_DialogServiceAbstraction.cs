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

// Exercise 020 — DialogServiceAbstraction (reference solution).
public sealed class DocumentCloser(IDialogService dialogs, DocumentStore store)
{
    public bool TryClose(string documentName)
    {
        // Asked ONCE, into a local. Calling AskToSave inside each branch of a switch
        // shows the user the same dialog two or three times.
        var choice = dialogs.AskToSave(documentName);

        switch (choice)
        {
            case SaveChoice.Save:
                store.Saved.Add(documentName);
                return true;

            case SaveChoice.Discard:
                store.Discarded.Add(documentName);
                return true;

            case SaveChoice.Cancel:
                // Neither list is touched: the user asked for the operation not to
                // happen, which is different from asking to throw the work away.
                return false;

            default:
                throw new ArgumentOutOfRangeException(nameof(documentName), choice, "Unhandled choice.");
        }
    }
}
