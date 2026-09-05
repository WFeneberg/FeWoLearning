using FeWoLearning.Architecture.Exercises.Desktop.Ex025;

namespace FeWoLearning.Architecture.Tests.Desktop;

public class Ex025_UndoRedoCommandStackTests
{
    [Fact]
    public void Executing_Applies_The_Change_And_Makes_It_Undoable()
    {
        var document = new Document { Text = "start" };
        var stack = new UndoStack();

        stack.Execute(new SetTextCommand(document, "first"));

        Assert.Equal("first", document.Text);
        Assert.True(stack.CanUndo);
        Assert.False(stack.CanRedo);
    }

    [Fact]
    public void Undoing_Reverses_The_Change_And_Makes_It_Redoable()
    {
        var document = new Document { Text = "start" };
        var stack = new UndoStack();
        stack.Execute(new SetTextCommand(document, "first"));

        stack.Undo();

        Assert.Equal("start", document.Text);
        Assert.False(stack.CanUndo);
        Assert.True(stack.CanRedo);
    }

    [Fact]
    public void Redoing_Reapplies_The_Change()
    {
        var document = new Document { Text = "start" };
        var stack = new UndoStack();
        stack.Execute(new SetTextCommand(document, "first"));
        stack.Undo();

        stack.Redo();

        Assert.Equal("first", document.Text);
        Assert.True(stack.CanUndo);
        Assert.False(stack.CanRedo);
    }

    [Fact]
    public void Several_Undos_Unwind_In_The_Opposite_Order()
    {
        var document = new Document { Text = "start" };
        var stack = new UndoStack();
        stack.Execute(new SetTextCommand(document, "one"));
        stack.Execute(new SetTextCommand(document, "two"));
        stack.Execute(new SetTextCommand(document, "three"));

        Assert.Equal(["set 'one'", "set 'two'", "set 'three'"], stack.UndoNames);

        stack.Undo();
        Assert.Equal("two", document.Text);
        stack.Undo();
        Assert.Equal("one", document.Text);
        stack.Undo();
        Assert.Equal("start", document.Text);
    }

    [Fact]
    public void Mechanism_A_New_Command_After_An_Undo_Throws_The_Redo_Stack_Away()
    {
        // The part people forget, and the bug it produces is genuinely alarming: undo
        // three edits, type something new, press redo, and the editor reapplies changes
        // computed against a document that no longer exists. The stack stayed
        // consistent; the document became nonsense.
        var document = new Document { Text = "start" };
        var stack = new UndoStack();
        stack.Execute(new SetTextCommand(document, "first"));
        stack.Undo();

        stack.Execute(new SetTextCommand(document, "different"));

        Assert.False(stack.CanRedo);
        Assert.Throws<InvalidOperationException>(stack.Redo);
        Assert.Equal("different", document.Text);
    }

    [Fact]
    public void Undoing_Or_Redoing_Nothing_Is_Refused()
    {
        var stack = new UndoStack();

        Assert.Throws<InvalidOperationException>(stack.Undo);
        Assert.Throws<InvalidOperationException>(stack.Redo);
    }
}
