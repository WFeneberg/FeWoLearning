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

// Exercise 025 — UndoRedoCommandStack (reference solution).
public sealed class UndoStack
{
    private readonly List<IUndoableCommand> _undo = [];
    private readonly List<IUndoableCommand> _redo = [];

    public bool CanUndo => _undo.Count > 0;

    public bool CanRedo => _redo.Count > 0;

    public IReadOnlyList<string> UndoNames => [.. _undo.Select(c => c.Name)];

    public void Execute(IUndoableCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        command.Execute();
        _undo.Add(command);

        // The line people forget. A redoable command was computed against a document
        // that no longer exists; reapplying it later produces nonsense from a stack that
        // was perfectly consistent the whole time.
        _redo.Clear();
    }

    public void Undo()
    {
        if (!CanUndo)
            throw new InvalidOperationException("There is nothing to undo.");

        var command = _undo[^1];
        _undo.RemoveAt(_undo.Count - 1);

        command.Undo();
        _redo.Add(command);
    }

    public void Redo()
    {
        if (!CanRedo)
            throw new InvalidOperationException("There is nothing to redo.");

        var command = _redo[^1];
        _redo.RemoveAt(_redo.Count - 1);

        command.Execute();
        _undo.Add(command);
    }
}
