using Bunit;
using FeWoLearning.Blazor.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Beginner;

public class Ex026_CheckboxGroupTests : BunitContext
{
    [Fact]
    public void Renders_One_Checkbox_Per_Feature_With_Selected_Initially_Empty()
    {
        var cut = Render<Ex026_CheckboxGroup>(p => p.Add(c => c.Features, new[] { "a", "b", "c" }));

        Assert.Equal(3, cut.FindAll("input[type=checkbox]").Count);
        Assert.Equal("", cut.Find("#selected").TextContent);

        // Each checkbox must be paired with its own <label for="feature-N">, not
        // just any three labels - the stub's TODO mandates this pairing.
        var checkboxes = cut.FindAll("input[type=checkbox]");
        var labels = cut.FindAll("label");
        Assert.Equal(new[] { "a", "b", "c" }, labels.Select(l => l.TextContent).ToArray());
        for (var i = 0; i < checkboxes.Count; i++)
        {
            Assert.Equal(checkboxes[i].GetAttribute("id"), labels[i].GetAttribute("for"));
        }
    }

    [Fact]
    public void An_Empty_Features_List_Renders_No_Checkboxes()
    {
        // Every other fact uses the same fixed Features array, so this is what
        // actually rules out a hard-coded set of checkboxes rather than a real
        // projection over Features - see ex025's identical empty-list fact.
        var cut = Render<Ex026_CheckboxGroup>(p => p.Add(c => c.Features, Array.Empty<string>()));

        Assert.Empty(cut.FindAll("input[type=checkbox]"));
        Assert.Equal("", cut.Find("#selected").TextContent);
    }

    [Fact]
    public void Checking_Out_Of_Order_Lists_Selected_In_Features_Order_Not_Click_Order()
    {
        var cut = Render<Ex026_CheckboxGroup>(p => p.Add(c => c.Features, new[] { "a", "b", "c" }));

        cut.Find("#feature-2").Change(true);
        cut.Find("#feature-0").Change(true);

        cut.WaitForAssertion(() => Assert.Equal("a, c", cut.Find("#selected").TextContent));
    }

    [Fact]
    public void Unchecking_A_Feature_Removes_It_From_Selected()
    {
        var cut = Render<Ex026_CheckboxGroup>(p => p.Add(c => c.Features, new[] { "a", "b", "c" }));

        cut.Find("#feature-2").Change(true);
        cut.Find("#feature-0").Change(true);
        cut.Find("#feature-0").Change(false);

        cut.WaitForAssertion(() => Assert.Equal("c", cut.Find("#selected").TextContent));
    }

    [Fact]
    public void SelectionChanged_Receives_The_Features_Ordered_Selection()
    {
        IReadOnlyList<string>? reported = null;
        var cut = Render<Ex026_CheckboxGroup>(p => p
            .Add(c => c.Features, new[] { "a", "b", "c" })
            .Add(c => c.SelectionChanged, (IReadOnlyList<string> s) => reported = s));

        cut.Find("#feature-2").Change(true);
        cut.Find("#feature-0").Change(true);

        // A captured local, not markup - WaitForAssertion is reserved for markup
        // assertions where a stale render frame is possible; wrapping a local
        // here would only delay reporting a genuine failure.
        Assert.Equal(new[] { "a", "c" }, reported);
    }
}
