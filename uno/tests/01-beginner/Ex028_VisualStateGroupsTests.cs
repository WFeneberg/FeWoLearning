using FeWoLearning.Uno.Exercises.Beginner;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Beginner;

public class Ex028_VisualStateGroupsTests : UnoTestContext
{
    private static Ex028_VisualStateGroups Control(bool highlighted = false) =>
        new() { Template = Ex028_VisualStateGroups.HighlightTemplate, IsHighlighted = highlighted };

    private static double FillOpacity(Ex028_VisualStateGroups control) =>
        FindDescendant<Border>(control, "PART_Fill").Opacity;

    [Fact]
    public void Starts_In_The_Normal_State()
    {
        var control = Layout(Control());

        Assert.Equal("Normal", control.LastRequestedState);
        Assert.Equal(1, FillOpacity(control), 2);
    }

    [Fact]
    public void Highlighting_Enters_The_Highlighted_State()
    {
        var control = Layout(Control());

        control.IsHighlighted = true;

        Assert.Equal("Highlighted", control.LastRequestedState);
        Assert.Equal(0.25, FillOpacity(control), 2);
    }

    [Fact]
    public void Unhighlighting_Goes_Back()
    {
        var control = Layout(Control(highlighted: true));

        control.IsHighlighted = false;

        // Leaving a state undoes its setters - the control never restores anything itself,
        // which is what keeps the logic independent of what the state actually changes.
        Assert.Equal(1, FillOpacity(control), 2);
    }

    [Fact]
    public void A_Control_Highlighted_Before_Its_Template_Comes_Up_Highlighted()
    {
        var control = Control(highlighted: true);

        Layout(control);

        // The property changed while no template - and therefore no state group - existed.
        Assert.Equal(0.25, FillOpacity(control), 2);
    }

    [Fact]
    public void The_Requested_State_Is_Recorded_Even_Without_A_Template()
    {
        var control = new Ex028_VisualStateGroups();

        control.IsHighlighted = true;

        // GoToState returns false when no group declares the state. A control still asks:
        // whether a look exists for a state is the template's business, not the control's.
        Assert.Equal("Highlighted", control.LastRequestedState);
    }

    [Fact]
    public void Setting_The_Same_Value_Keeps_The_State()
    {
        var control = Layout(Control(highlighted: true));

        control.IsHighlighted = true;

        Assert.Equal("Highlighted", control.LastRequestedState);
        Assert.Equal(0.25, FillOpacity(control), 2);
    }

    [Fact]
    public void Re_Applying_The_Template_Re_Enters_The_State()
    {
        var control = Layout(Control(highlighted: true));

        control.Template = Ex028_VisualStateGroups.HighlightTemplate;
        Layout(control);

        Assert.Equal(0.25, FillOpacity(control), 2);
    }
}
