namespace FeWoLearning.Architecture.Exercises.Desktop.Ex025;

public sealed class Document
{
    public string Text { get; set; } = "";
}

public interface IUndoableCommand
{
    string Name { get; }

    void Execute();

    void Undo();
}

/// <summary>
/// Captures the previous text at CONSTRUCTION time, not at Execute time, so re-executing
/// a redone command restores the right thing.
/// </summary>
public sealed class SetTextCommand(Document document, string newText) : IUndoableCommand
{
    private readonly string _previousText = document.Text;

    public string Name => $"set '{newText}'";

    public void Execute() => document.Text = newText;

    public void Undo() => document.Text = _previousText;
}

// Exercise 025 — UndoRedoCommandStack (desktop).
// Goal:   An undo/redo stack that behaves the way every editor a user has ever used
//         behaves - including the part people forget.
// Drills: command pattern, undo/redo invariants, redo invalidation.
// Passes: Execute()  - runs the command and makes it undoable.
//         Undo()     - reverses it and makes it redoable.
//         Redo()     - reapplies it.
//         unwinding  - several undos reverse in the opposite order to execution.
//         THE ONE     - executing a new command AFTER an undo throws the redo stack
//                       away: CanRedo goes false and Redo() throws.
//         empty      - Undo() or Redo() with nothing to do throws InvalidOperationException.
//
// The redo invalidation is the part people forget, and leaving it out produces a
// genuinely alarming bug: undo three edits, type something new, press redo, and the
// editor reapplies changes on top of a document that no longer has the text they were
// computed against. The stack was consistent; the document is now nonsense.
public sealed class UndoStack
{
    public bool CanUndo =>
        throw new NotImplementedException("TODO: Ex025 - is there anything to undo");

    public bool CanRedo =>
        throw new NotImplementedException("TODO: Ex025 - is there anything to redo");

    /// <summary>Names of the undoable commands, oldest first.</summary>
    public IReadOnlyList<string> UndoNames =>
        throw new NotImplementedException("TODO: Ex025 - the undoable commands, oldest first");

    public void Execute(IUndoableCommand command) =>
        throw new NotImplementedException(
            "TODO: Ex025 - run the command, make it undoable, and invalidate anything that was redoable");

    public void Undo() =>
        throw new NotImplementedException(
            "TODO: Ex025 - undo the most recent command and make it redoable");

    public void Redo() =>
        throw new NotImplementedException(
            "TODO: Ex025 - re-execute the most recently undone command and make it undoable again");
}
