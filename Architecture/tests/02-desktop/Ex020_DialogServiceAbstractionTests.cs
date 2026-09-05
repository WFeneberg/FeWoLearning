using FeWoLearning.Architecture.Exercises.Desktop.Ex020;

namespace FeWoLearning.Architecture.Tests.Desktop;

public class Ex020_DialogServiceAbstractionTests
{
    /// <summary>Records what it was asked, so "asked once, with the right name" is checkable.</summary>
    private sealed class RecordingDialogs(SaveChoice answer) : IDialogService
    {
        public List<string> Asked { get; } = [];

        public SaveChoice AskToSave(string documentName)
        {
            Asked.Add(documentName);
            return answer;
        }
    }

    private static (DocumentCloser Closer, DocumentStore Store, RecordingDialogs Dialogs) Build(SaveChoice answer)
    {
        var store = new DocumentStore();
        var dialogs = new RecordingDialogs(answer);
        return (new DocumentCloser(dialogs, store), store, dialogs);
    }

    [Fact]
    public void Save_Writes_The_Document_And_Closes()
    {
        var (closer, store, _) = Build(SaveChoice.Save);

        Assert.True(closer.TryClose("notes.txt"));
        Assert.Equal(["notes.txt"], store.Saved);
        Assert.Empty(store.Discarded);
    }

    [Fact]
    public void Discard_Throws_The_Work_Away_And_Closes()
    {
        var (closer, store, _) = Build(SaveChoice.Discard);

        Assert.True(closer.TryClose("notes.txt"));
        Assert.Empty(store.Saved);
        Assert.Equal(["notes.txt"], store.Discarded);
    }

    [Fact]
    public void Mechanism_Cancel_Neither_Saves_Nor_Discards_And_Keeps_The_Document_Open()
    {
        // Why the port returns an enum. Modelled as a bool, Discard and Cancel collapse
        // into the same false, and the window closes on a user who asked it not to -
        // losing exactly the work they were trying to keep. Both facts above pass for
        // that design; this one does not.
        var (closer, store, _) = Build(SaveChoice.Cancel);

        Assert.False(closer.TryClose("notes.txt"));
        Assert.Empty(store.Saved);
        Assert.Empty(store.Discarded);
    }

    [Theory]
    [InlineData(SaveChoice.Save)]
    [InlineData(SaveChoice.Discard)]
    [InlineData(SaveChoice.Cancel)]
    public void The_User_Is_Asked_Exactly_Once_With_The_Document_Name(SaveChoice answer)
    {
        // Calling AskToSave inside each branch of a switch shows the same dialog two or
        // three times, and every behavioural assertion above still passes.
        //
        // A Theory across all three answers, not a Fact on one of them. Measured: this
        // was a [Fact] pinned to Save, and the wrong-mechanism probe - an implementation
        // that asks again inside the Discard branch - went completely undetected,
        // because the only branch the fact ever walked was the correct one. A
        // "called once" assertion has to visit every path that could call it twice.
        var (closer, _, dialogs) = Build(answer);

        closer.TryClose("notes.txt");

        Assert.Equal(["notes.txt"], dialogs.Asked);
    }
}
