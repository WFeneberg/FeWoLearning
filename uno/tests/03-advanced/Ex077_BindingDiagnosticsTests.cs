using FeWoLearning.Uno.Exercises.Advanced;
using FeWoLearning.Uno.Support;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Advanced;

public class Ex077_BindingDiagnosticsTests : UnoTestContext
{
    private static (TextBlock Editor, CaptionSource Source) Editor(string caption = "start")
    {
        var source = new CaptionSource { Caption = caption };
        return (Ex077_BindingDiagnostics.CreateEditor(source), source);
    }

    [Fact]
    public void The_Editor_Starts_From_The_Source()
    {
        var (editor, _) = Editor();

        Assert.Equal("start", editor.Text);
    }

    [Fact]
    public void The_Binding_Is_Reachable()
    {
        var (editor, _) = Editor();

        // A null here means the binding was never attached, which is a different bug from
        // one that is attached and failing - and only this call tells them apart.
        Assert.NotNull(Ex077_BindingDiagnostics.ExpressionOf(editor));
    }

    [Fact]
    public void An_Edit_Does_Not_Reach_The_Source_On_Its_Own()
    {
        var (editor, source) = Editor();

        editor.Text = "typed";

        // The point of an explicit trigger: a form writes to its model when Save is
        // pressed, not on every keystroke.
        Assert.Equal("start", source.Caption);
    }

    [Fact]
    public void Committing_Pushes_The_Edit_To_The_Source()
    {
        var (editor, source) = Editor();
        editor.Text = "typed";

        Assert.True(Ex077_BindingDiagnostics.Commit(editor));
        Assert.Equal("typed", source.Caption);
    }

    [Fact]
    public void The_Binding_Survives_A_Local_Write()
    {
        var (editor, _) = Editor();

        editor.Text = "typed";

        // Writing the target property of a two-way binding does not detach it - which is
        // exactly why the edit can still be committed afterwards.
        Assert.NotNull(Ex077_BindingDiagnostics.ExpressionOf(editor));
    }

    [Fact]
    public void Committing_Twice_Is_Harmless()
    {
        var (editor, source) = Editor();
        editor.Text = "typed";

        Ex077_BindingDiagnostics.Commit(editor);
        Ex077_BindingDiagnostics.Commit(editor);

        Assert.Equal("typed", source.Caption);
    }

    [Fact]
    public void An_Unbound_Element_Reports_No_Expression()
    {
        var plain = new TextBlock { Text = "no binding here" };

        Assert.Null(Ex077_BindingDiagnostics.ExpressionOf(plain));
    }

    [Fact]
    public void Committing_An_Unbound_Element_Says_So()
    {
        var plain = new TextBlock { Text = "no binding here" };

        // A diagnostic, not a crash: the caller asked whether there was anything to commit.
        Assert.False(Ex077_BindingDiagnostics.Commit(plain));
    }

    [Fact]
    public void A_Source_Change_Still_Reaches_The_Editor()
    {
        var (editor, source) = Editor();

        source.Caption = "from elsewhere";

        // Explicit governs the *source* direction only; the target still follows.
        Assert.Equal("from elsewhere", editor.Text);
    }
}
