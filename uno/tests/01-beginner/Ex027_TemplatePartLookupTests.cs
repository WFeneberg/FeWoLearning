using FeWoLearning.Uno.Exercises.Beginner;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Beginner;

public class Ex027_TemplatePartLookupTests : UnoTestContext
{
    private static Ex027_TemplatePartLookup Control(string caption = "ready", bool withLabel = true) =>
        new()
        {
            Caption = caption,
            Template = withLabel ? Ex027_TemplatePartLookup.WithLabel : Ex027_TemplatePartLookup.WithoutLabel,
        };

    [Fact]
    public void Pushes_The_Caption_Into_The_Part_When_The_Template_Arrives()
    {
        var control = Layout(Control());

        Assert.Equal("ready", FindDescendant<TextBlock>(control, "PART_Label").Text);
    }

    [Fact]
    public void A_Later_Caption_Change_Reaches_The_Part()
    {
        var control = Layout(Control());

        control.Caption = "changed";

        Assert.Equal("changed", FindDescendant<TextBlock>(control, "PART_Label").Text);
    }

    [Fact]
    public void A_Caption_Set_Before_The_Template_Is_Not_Lost()
    {
        var control = Control(caption: "");
        control.Caption = "set early";

        // The property changed while there was no part to push it into, so the value has
        // to be re-read when one shows up.
        Layout(control);

        Assert.Equal("set early", FindDescendant<TextBlock>(control, "PART_Label").Text);
    }

    [Fact]
    public void A_Template_Without_The_Part_Does_Not_Throw()
    {
        var control = Control(withLabel: false);

        Layout(control);

        Assert.Empty(Descendants(control).OfType<TextBlock>());
    }

    [Fact]
    public void A_Caption_Change_Without_The_Part_Does_Not_Throw()
    {
        var control = Layout(Control(withLabel: false));

        control.Caption = "nobody is listening";

        Assert.Equal("nobody is listening", control.Caption);
    }

    [Fact]
    public void Swapping_To_A_Template_Without_The_Part_Drops_The_Old_One()
    {
        var control = Layout(Control());
        var firstLabel = FindDescendant<TextBlock>(control, "PART_Label");

        control.Template = Ex027_TemplatePartLookup.WithoutLabel;
        Layout(control);
        control.Caption = "after the swap";

        // Holding on to a part from a template that is gone writes into a detached
        // element - invisible on screen, and a leak of the old tree.
        Assert.Equal("after the swap", control.Caption);
        Assert.NotEqual("after the swap", firstLabel.Text);
    }

    [Fact]
    public void Swapping_To_A_Template_With_The_Part_Finds_The_New_One()
    {
        var control = Layout(Control(withLabel: false));

        control.Template = Ex027_TemplatePartLookup.WithLabel;
        Layout(control);

        Assert.Equal("ready", FindDescendant<TextBlock>(control, "PART_Label").Text);
    }
}
