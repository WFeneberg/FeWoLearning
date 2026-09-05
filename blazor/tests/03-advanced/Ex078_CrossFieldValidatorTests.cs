using Bunit;
using FeWoLearning.Blazor.Exercises.Advanced;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Advanced;

using RangeModel = Ex078_CrossFieldValidator_Form.RangeModel;

public class Ex078_CrossFieldValidatorTests : BunitContext
{
    private readonly RangeModel _model = new() { Start = 5, End = 3 };

    private IRenderedComponent<Ex078_CrossFieldValidator_Form> RenderForm()
        => Render<Ex078_CrossFieldValidator_Form>(p => p.Add(c => c.Model, _model));

    private static string Messages(IRenderedComponent<Ex078_CrossFieldValidator_Form> cut, string field)
        => cut.Find($"#{field}-messages").TextContent;

    // The verdict belongs to End, not to the model and not to Start: a message
    // attached to the wrong field identifier renders nowhere the user is looking.
    [Fact]
    public void An_Invalid_Range_Reports_On_The_End_Field()
    {
        var cut = RenderForm();

        cut.Find("#submit").Click();

        cut.WaitForAssertion(() => Assert.Equal(Ex078_CrossFieldValidator.RangeError, Messages(cut, "end")));
        Assert.Equal("", Messages(cut, "start"));
    }

    [Fact]
    public void Moving_End_Past_Start_Clears_It()
    {
        var cut = RenderForm();
        cut.Find("#submit").Click();
        cut.WaitForAssertion(() => Assert.NotEqual("", Messages(cut, "end")));

        cut.Find("#end").Change("9");

        cut.WaitForAssertion(() => Assert.Equal("", Messages(cut, "end")));
    }

    // Ruling: the cross-field trap. Start is the field that changed, End is the field
    // whose verdict is now stale. A handler that only re-checks e.FieldIdentifier
    // leaves the error sitting on End even though the range is now fine.
    [Fact]
    public void Moving_Start_Behind_End_Clears_The_Message_On_End()
    {
        var cut = RenderForm();
        cut.Find("#submit").Click();
        cut.WaitForAssertion(() => Assert.Equal(Ex078_CrossFieldValidator.RangeError, Messages(cut, "end")));

        cut.Find("#start").Change("1");

        cut.WaitForAssertion(() => Assert.Equal("", Messages(cut, "end")));
    }

    [Fact]
    public void Moving_Start_Past_End_Raises_The_Message_Again()
    {
        var cut = RenderForm();
        cut.Find("#end").Change("9");
        cut.WaitForAssertion(() => Assert.Equal("", Messages(cut, "end")));

        cut.Find("#start").Change("20");

        cut.WaitForAssertion(() => Assert.Equal(Ex078_CrossFieldValidator.RangeError, Messages(cut, "end")));
    }

    // Non-vacuity for clearing before re-adding: without it every pass appends
    // another copy, which a single-message assertion would not notice but this does.
    [Fact]
    public void Repeated_Passes_Do_Not_Pile_Up_Duplicate_Messages()
    {
        var cut = RenderForm();

        cut.Find("#submit").Click();
        cut.Find("#submit").Click();
        cut.Find("#submit").Click();

        cut.WaitForAssertion(() => Assert.Single(cut.FindAll("#end-messages .validation-message")));
    }
}
