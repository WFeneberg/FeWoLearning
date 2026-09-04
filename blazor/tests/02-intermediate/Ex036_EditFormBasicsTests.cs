using Bunit;
using FeWoLearning.Blazor.Exercises.Intermediate;
using FeWoLearning.Blazor.Support;
using Microsoft.AspNetCore.Components;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Intermediate;

public class Ex036_EditFormBasicsTests : BunitContext
{
    [Fact]
    public void Renders_A_Form_With_A_Name_Input_And_A_Submit_Button()
    {
        var cut = Render<Ex036_EditFormBasics>(p => p.Add(x => x.Model, new ContactModel()));

        // Plain initial render, no event dispatch, so there is no re-render to wait
        // for - this needs no WaitForAssertion.
        cut.Find("form");
        Assert.Equal("INPUT", cut.Find("#name").TagName);
        Assert.Equal("BUTTON", cut.Find("#submit").TagName);
    }

    [Fact]
    public void Submitting_Once_Increments_The_Count_To_One()
    {
        var cut = Render<Ex036_EditFormBasics>(p => p.Add(x => x.Model, new ContactModel()));

        cut.Find("form").Submit();

        cut.WaitForAssertion(() => Assert.Equal("1", cut.Find("#count").TextContent));
    }

    [Fact]
    public void Submitting_Twice_Increments_The_Count_To_Two()
    {
        var cut = Render<Ex036_EditFormBasics>(p => p.Add(x => x.Model, new ContactModel()));

        cut.Find("form").Submit();
        cut.Find("form").Submit();

        cut.WaitForAssertion(() => Assert.Equal("2", cut.Find("#count").TextContent));
    }

    // Non-vacuity note: this exercise's EditForm has no <DataAnnotationsValidator />,
    // so EditContext.Validate() always reports valid (nothing ever adds a message) and
    // OnValidSubmit fires on every submit exactly as a plain OnSubmit would. Wiring
    // OnSubmit here instead of OnValidSubmit would pass all four facts in this class -
    // that distinction only becomes observable once a validator is attached, which is
    // ex037's subject, not this one's.
    [Fact]
    public void OnAccepted_Receives_The_Same_Model_Instance()
    {
        var model = new ContactModel();
        ContactModel? received = null;
        var cut = Render<Ex036_EditFormBasics>(p => p
            .Add(x => x.Model, model)
            .Add(x => x.OnAccepted, EventCallback.Factory.Create<ContactModel>(this, m => received = m)));

        cut.Find("form").Submit();

        // A captured local, not markup - no render frame to go stale, so this needs
        // no WaitForAssertion.
        Assert.Same(model, received);
    }
}
