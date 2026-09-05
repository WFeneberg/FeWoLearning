using Bunit;
using FeWoLearning.Blazor.Exercises.Expert;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Expert;

using RowSpec = Ex099_DiffAlgorithmKeyMismatch.RowSpec;

public class Ex099_DiffAlgorithmKeyMismatchTests : BunitContext
{
    private IRenderedComponent<Ex099_DiffAlgorithmKeyMismatch> RenderRows(params RowSpec[] rows)
        => Render<Ex099_DiffAlgorithmKeyMismatch>(p => p.Add(c => c.Rows, rows));

    private static Ex099_DiffAlgorithmKeyMismatch_Row RowFor(
        IRenderedComponent<Ex099_DiffAlgorithmKeyMismatch> cut, string text)
        => cut.FindComponents<Ex099_DiffAlgorithmKeyMismatch_Row>()
            .Single(row => row.Instance.Text == text)
            .Instance;

    [Fact]
    public void Renders_One_Row_Per_Spec_In_Order()
    {
        var cut = RenderRows(new RowSpec("a", "alpha"), new RowSpec("b", "beta"));

        var rows = cut.FindAll(".rows .row");
        Assert.Equal(2, rows.Count);
        Assert.Equal("alpha", rows[0].TextContent);
        Assert.Equal("beta", rows[1].TextContent);
    }

    [Fact]
    public void A_Row_Whose_Key_Is_Unchanged_Keeps_Its_Instance_When_It_Moves()
    {
        var cut = RenderRows(new RowSpec("a", "alpha"), new RowSpec("b", "beta"));
        var alpha = RowFor(cut, "alpha");
        cut.InvokeAsync(() => alpha.Tick());
        cut.WaitForAssertion(() => Assert.Equal(1, RowFor(cut, "alpha").Ticks));

        cut.Render(p => p.Add(
            c => c.Rows, new[] { new RowSpec("b", "beta"), new RowSpec("a", "alpha") }));

        Assert.Same(alpha, RowFor(cut, "alpha"));
        Assert.Equal(1, RowFor(cut, "alpha").Ticks);
        Assert.Equal(0, RowFor(cut, "beta").Ticks);
    }

    // Ruling: the mismatch. Nothing about this row changed except the key it was
    // given, and that alone makes it a different row to the diff - the old instance
    // is disposed, a new one is built, and the state only it held is gone. Its
    // neighbour, whose key did not change, is untouched.
    [Fact]
    public void A_Row_Whose_Key_Changed_Is_Rebuilt_Even_Though_Nothing_Else_Did()
    {
        var cut = RenderRows(new RowSpec("a", "alpha"), new RowSpec("b", "beta"));
        var alpha = RowFor(cut, "alpha");
        var beta = RowFor(cut, "beta");
        cut.InvokeAsync(() => alpha.Tick());
        cut.InvokeAsync(() => beta.Tick());
        cut.WaitForAssertion(() => Assert.Equal(1, RowFor(cut, "beta").Ticks));

        cut.Render(p => p.Add(
            c => c.Rows, new[] { new RowSpec("a-renamed", "alpha"), new RowSpec("b", "beta") }));

        Assert.NotSame(alpha, RowFor(cut, "alpha"));
        Assert.Equal(0, RowFor(cut, "alpha").Ticks);
        Assert.Same(beta, RowFor(cut, "beta"));
        Assert.Equal(1, RowFor(cut, "beta").Ticks);
    }

    // Non-vacuity for the rebuild above: a plain parameter change under the same key
    // must NOT rebuild anything, or "changed key rebuilds" would be indistinguishable
    // from "any change rebuilds".
    [Fact]
    public void A_Changed_Text_Under_The_Same_Key_Patches_Instead_Of_Rebuilding()
    {
        var cut = RenderRows(new RowSpec("a", "alpha"));
        var alpha = RowFor(cut, "alpha");
        cut.InvokeAsync(() => alpha.Tick());
        cut.WaitForAssertion(() => Assert.Equal(1, RowFor(cut, "alpha").Ticks));

        cut.Render(p => p.Add(c => c.Rows, new[] { new RowSpec("a", "renamed") }));

        Assert.Same(alpha, RowFor(cut, "renamed"));
        Assert.Equal(1, RowFor(cut, "renamed").Ticks);
    }

    [Fact]
    public void A_Removed_Key_Takes_Only_Its_Own_Row()
    {
        var cut = RenderRows(
            new RowSpec("a", "alpha"), new RowSpec("b", "beta"), new RowSpec("c", "gamma"));
        var gamma = RowFor(cut, "gamma");
        cut.InvokeAsync(() => gamma.Tick());
        cut.WaitForAssertion(() => Assert.Equal(1, RowFor(cut, "gamma").Ticks));

        cut.Render(p => p.Add(
            c => c.Rows, new[] { new RowSpec("b", "beta"), new RowSpec("c", "gamma") }));

        Assert.Equal(2, cut.FindAll(".rows .row").Count);
        Assert.Same(gamma, RowFor(cut, "gamma"));
        Assert.Equal(1, RowFor(cut, "gamma").Ticks);
    }
}
