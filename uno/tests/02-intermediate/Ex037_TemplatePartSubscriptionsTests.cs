using FeWoLearning.Uno.Exercises.Intermediate;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Intermediate;

public class Ex037_TemplatePartSubscriptionsTests : UnoTestContext
{
    private static Ex037_TemplatePartSubscriptions Control() =>
        Layout(new Ex037_TemplatePartSubscriptions { Template = Ex037_TemplatePartSubscriptions.FirstTemplate });

    private static Button Trigger(Ex037_TemplatePartSubscriptions control) =>
        FindDescendant<Button>(control, "PART_Trigger");

    private static void Press(Button button) => new ButtonAutomationPeer(button).Invoke();

    [Fact]
    public void The_Wired_Trigger_Counts_Presses()
    {
        var control = Control();

        Press(Trigger(control));
        Press(Trigger(control));

        Assert.Equal(2, control.Presses);
    }

    [Fact]
    public void Re_Templating_Wires_The_New_Trigger()
    {
        var control = Control();

        control.Template = Ex037_TemplatePartSubscriptions.SecondTemplate;
        Layout(control);
        Press(Trigger(control));

        Assert.Equal(1, control.Presses);
    }

    [Fact]
    public void Re_Templating_Releases_The_Old_Trigger()
    {
        var control = Control();
        var oldTrigger = Trigger(control);

        control.Template = Ex037_TemplatePartSubscriptions.SecondTemplate;
        Layout(control);
        Press(oldTrigger);

        // The old button is detached but still reachable - by the handler, by an animation,
        // by anything that captured it. A subscription left on it keeps reacting, and keeps
        // the whole old template tree alive.
        Assert.Equal(0, control.Presses);
    }

    [Fact]
    public void The_Old_And_New_Triggers_Are_Different_Elements()
    {
        var control = Control();
        var oldTrigger = Trigger(control);

        control.Template = Ex037_TemplatePartSubscriptions.SecondTemplate;
        Layout(control);

        Assert.NotSame(oldTrigger, Trigger(control));
    }

    [Fact]
    public void Presses_Survive_A_Re_Template()
    {
        var control = Control();
        Press(Trigger(control));

        control.Template = Ex037_TemplatePartSubscriptions.SecondTemplate;
        Layout(control);
        Press(Trigger(control));

        // The count belongs to the control, not to the template.
        Assert.Equal(2, control.Presses);
    }

    [Fact]
    public void A_Template_Without_The_Part_Leaves_Nothing_Wired()
    {
        var control = Control();
        var oldTrigger = Trigger(control);

        control.Template = Ex037_TemplatePartSubscriptions.WithoutTrigger;
        Layout(control);
        Press(oldTrigger);

        Assert.Equal(0, control.Presses);
        Assert.Empty(Descendants(control).OfType<Button>());
    }

    [Fact]
    public void Going_Back_To_A_Template_With_The_Part_Wires_It_Again()
    {
        var control = Control();
        control.Template = Ex037_TemplatePartSubscriptions.WithoutTrigger;
        Layout(control);

        control.Template = Ex037_TemplatePartSubscriptions.FirstTemplate;
        Layout(control);
        Press(Trigger(control));

        Assert.Equal(1, control.Presses);
    }
}
