using Bunit;
using FeWoLearning.Blazor.Exercises.Intermediate;
using FeWoLearning.Blazor.Support;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Intermediate;

public class Ex040_EditContextFieldStateTests : BunitContext
{
    [Fact]
    public void Initially_The_Model_Is_Not_Modified()
    {
        var cut = Render<Ex040_EditContextFieldState>(p => p.Add(x => x.Model, new ContactModel()));

        // Plain initial render, no event dispatch - no WaitForAssertion needed.
        Assert.Equal("False", cut.Find("#modified").TextContent);
    }

    [Fact]
    public void Changing_The_Name_Marks_The_Model_Modified()
    {
        var cut = Render<Ex040_EditContextFieldState>(p => p.Add(x => x.Model, new ContactModel()));

        cut.Find("#name").Change("Ada");

        cut.WaitForAssertion(() => Assert.Equal("True", cut.Find("#modified").TextContent));
    }

    [Fact]
    public void Resetting_After_A_Change_Marks_The_Model_Unmodified_Again()
    {
        var cut = Render<Ex040_EditContextFieldState>(p => p.Add(x => x.Model, new ContactModel()));
        cut.Find("#name").Change("Ada");
        cut.WaitForAssertion(() => Assert.Equal("True", cut.Find("#modified").TextContent));

        cut.Find("#reset").Click();

        cut.WaitForAssertion(() => Assert.Equal("False", cut.Find("#modified").TextContent));
    }

    // Non-vacuity note: a hand-tracked bool flipped on the name field's change event
    // would also pass the first three facts above. It would NOT pass this one - the
    // "modified" class comes from EditContext's own FieldCssClassProvider machinery
    // applied to InputText, not from anything this component could set by hand on a
    // plain HTML element bound with @bind. That is the fact that forces the real
    // mechanism (a real EditContext driving the InputText) rather than a parallel
    // bookkeeping flag.
    [Fact]
    public void Changing_The_Name_Adds_The_Modified_Css_Class_Via_EditContext()
    {
        var cut = Render<Ex040_EditContextFieldState>(p => p.Add(x => x.Model, new ContactModel()));

        cut.Find("#name").Change("Ada");

        cut.WaitForAssertion(() => Assert.Contains("modified", cut.Find("#name").ClassList));
    }
}
