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

        cut.WaitForAssertion(() => Assert.Equal(new[] { "a", "c" }, reported));
    }
}
