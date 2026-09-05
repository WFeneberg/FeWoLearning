using Bunit;
using FeWoLearning.Blazor.Exercises.Advanced;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Advanced;

using TagModel = Ex075_CustomInputBaseText_Form.TagModel;

public class Ex075_CustomInputBaseTextTests : BunitContext
{
    private readonly TagModel _model = new() { Tags = ["alpha", "beta"] };

    private IRenderedComponent<Ex075_CustomInputBaseText_Form> RenderForm()
        => Render<Ex075_CustomInputBaseText_Form>(p => p.Add(c => c.Model, _model));

    [Fact]
    public void Shows_The_Current_Value_As_Text()
    {
        var cut = RenderForm();

        Assert.Equal("alpha, beta", cut.Find("#tags").GetAttribute("value"));
    }

    [Fact]
    public void A_Null_Value_Shows_As_An_Empty_Input()
    {
        var cut = Render<Ex075_CustomInputBaseText_Form>(p => p.Add(
            c => c.Model, new TagModel { Tags = null! }));

        Assert.Equal("", cut.Find("#tags").GetAttribute("value"));
    }

    [Fact]
    public void Typing_A_Valid_List_Parses_It_Into_The_Model()
    {
        var cut = RenderForm();

        cut.Find("#tags").Change("red, green ,blue");

        cut.WaitForAssertion(() => Assert.Equal(["red", "green", "blue"], _model.Tags));
        Assert.Empty(cut.FindAll(".validation-message"));
    }

    // The contract of returning false: the model is left alone and the message is
    // what the user sees. Measured, so that the claim is not louder than the test -
    // an implementation that accepts the bad input (returns true, no message) fails
    // this fact and the next one. What `result` was set to on the false path is NOT
    // graded here, and cannot be: InputBase ignores the out value entirely when the
    // method returns false.
    [Fact]
    public void Typing_An_Invalid_List_Leaves_The_Model_Alone_And_Reports_It()
    {
        var cut = RenderForm();

        cut.Find("#tags").Change("red, two words, blue");

        cut.WaitForAssertion(() => Assert.Equal(
            Ex075_CustomInputBaseText.ParseError,
            cut.Find(".validation-message").TextContent));
        Assert.Equal(["alpha", "beta"], _model.Tags);
    }

    [Fact]
    public void Correcting_The_Text_Clears_The_Error_And_Lands_The_Value()
    {
        var cut = RenderForm();
        cut.Find("#tags").Change("red, two words");
        cut.WaitForAssertion(() => Assert.NotEmpty(cut.FindAll(".validation-message")));

        cut.Find("#tags").Change("red, green");

        cut.WaitForAssertion(() => Assert.Equal(["red", "green"], _model.Tags));
        Assert.Empty(cut.FindAll(".validation-message"));
    }

    // Empty text is a legal value, not a parse error - the difference between "the
    // user cleared the field" and "the user typed nonsense".
    [Fact]
    public void Clearing_The_Input_Parses_To_An_Empty_List()
    {
        var cut = RenderForm();

        cut.Find("#tags").Change("");

        cut.WaitForAssertion(() => Assert.Empty(_model.Tags));
        Assert.Empty(cut.FindAll(".validation-message"));
    }
}
