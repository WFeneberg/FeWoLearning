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

    // Non-vacuity note: on its own, a hand-tracked bool flipped on the name field's
    // change event and cleared on reset would also pass this fact - the class check
    // and the #modified span read are two independent things unless something ties
    // them to the same mechanism. The tie-breaker is the pair with the next fact:
    // a hand-tracker can make #modified read False again after Reset, but it cannot
    // make the "modified" CSS class come OFF the input, because that class is driven
    // by the real EditContext's own FieldCssClassProvider machinery, not by anything
    // this component sets by hand. Only calling MarkAsUnmodified() on the actual
    // EditContext clears both #modified and the CSS class together.
    [Fact]
    public void Changing_The_Name_Adds_The_Modified_Css_Class_Via_EditContext()
    {
        var cut = Render<Ex040_EditContextFieldState>(p => p.Add(x => x.Model, new ContactModel()));

        cut.Find("#name").Change("Ada");

        cut.WaitForAssertion(() => Assert.Contains("modified", cut.Find("#name").ClassList));
    }

    [Fact]
    public void Resetting_After_A_Change_Removes_The_Modified_Css_Class_Too()
    {
        var cut = Render<Ex040_EditContextFieldState>(p => p.Add(x => x.Model, new ContactModel()));
        cut.Find("#name").Change("Ada");
        cut.WaitForAssertion(() => Assert.Contains("modified", cut.Find("#name").ClassList));

        cut.Find("#reset").Click();

        cut.WaitForAssertion(() => Assert.DoesNotContain("modified", cut.Find("#name").ClassList));
    }
}
