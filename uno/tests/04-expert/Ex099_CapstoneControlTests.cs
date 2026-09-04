using FeWoLearning.Uno.Exercises.Expert;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Automation.Provider;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Expert;

public class Ex099_CapstoneControlTests : UnoTestContext
{
    /// <summary>A control inside a scope that has the shipped styles, laid out.</summary>
    private static Ex099_RatingControl Rating(int value = 0, int maximum = 5, bool readOnly = false)
    {
        var control = new Ex099_RatingControl { Maximum = maximum, Value = value, IsReadOnly = readOnly };
        var scope = new StackPanel();
        Ex099_RatingStyles.MergeInto(scope);
        scope.Children.Add(control);
        Layout(scope, width: 200, height: 200);
        return control;
    }

    private static void Press(Ex099_RatingControl control)
    {
        // Asserted rather than dereferenced: a control whose OnApplyTemplate has not picked
        // the part up yet should say so, not fail with a NullReferenceException.
        var part = control.IncrementPart;
        Assert.True(part is Button, "PART_Increment was never picked up from the template");

        new ButtonAutomationPeer((Button)part!).Invoke();
    }

    [Fact]
    public void The_Shipped_Style_Supplies_The_Template()
    {
        var control = Rating();

        Assert.True(control.Template is not null, "no template - is DefaultStyleKey set, and were the styles merged?");
        Assert.True(control.IncrementPart is not null, "PART_Increment was never picked up from the template");
    }

    [Fact]
    public void Pressing_The_Part_Raises_The_Rating()
    {
        var control = Rating();

        Press(control);

        Assert.Equal(1, control.Value);
    }

    [Fact]
    public void The_Value_Change_Is_Announced()
    {
        var control = Rating();
        var announced = new List<int>();
        control.ValueChanged += (_, value) => announced.Add(value);

        Press(control);

        Assert.Equal([1], announced);
    }

    [Fact]
    public void The_Value_Is_Clamped_To_The_Maximum()
    {
        var control = Rating(maximum: 2);

        control.Value = 7;

        // Corrected rather than rejected - and the correction re-enters the callback, so it
        // has to be a no-op the second time rather than a loop.
        Assert.Equal(2, control.Value);
    }

    [Fact]
    public void A_Negative_Value_Is_Clamped_To_Zero()
    {
        var control = Rating();

        control.Value = -3;

        Assert.Equal(0, control.Value);
    }

    [Fact]
    public void Lowering_The_Maximum_Corrects_The_Value()
    {
        var control = Rating(value: 5, maximum: 5);

        control.Maximum = 3;

        Assert.Equal(3, control.Value);
    }

    [Fact]
    public void Incrementing_Stops_At_The_Maximum()
    {
        var control = Rating(value: 2, maximum: 2);

        control.Increment();

        Assert.Equal(2, control.Value);
    }

    [Fact]
    public void A_Read_Only_Control_Refuses_To_Change_Itself()
    {
        var control = Rating(readOnly: true);

        control.Increment();

        Assert.Equal(0, control.Value);
    }

    [Fact]
    public void A_Read_Only_Control_Disables_Its_Part()
    {
        var control = Rating(readOnly: true);

        Assert.False(control.IncrementPart!.IsEnabled);
    }

    [Fact]
    public void The_Two_State_Groups_Are_Independent()
    {
        var control = Rating(value: 3, readOnly: true);

        // Read-only and rated at once: a control that squeezed both into one group would
        // need a state per combination.
        Assert.Equal(["ReadOnly", "Rated"], control.LastRequestedStates.TakeLast(2));
        Assert.Equal(60, FindDescendant<Border>(control, "PART_Fill").Width, 1);
    }

    [Fact]
    public void A_Rating_Set_Before_The_Template_Comes_Up_Rated()
    {
        var control = Rating(value: 1);

        Assert.Equal(60, FindDescendant<Border>(control, "PART_Fill").Width, 1);
    }

    [Fact]
    public void The_Peer_Reports_The_Range()
    {
        var control = Rating(value: 2, maximum: 4);
        var peer = (IRangeValueProvider)FrameworkElementAutomationPeer.CreatePeerForElement(control);

        Assert.Equal(0, peer.Minimum);
        Assert.Equal(4, peer.Maximum);
        Assert.Equal(2, peer.Value);
    }

    [Fact]
    public void The_Peer_Can_Set_The_Value()
    {
        var control = Rating();
        var peer = (IRangeValueProvider)FrameworkElementAutomationPeer.CreatePeerForElement(control);

        peer.SetValue(3);

        Assert.Equal(3, control.Value);
    }

    [Fact]
    public void The_Peer_Respects_Read_Only()
    {
        var control = Rating(readOnly: true);
        var peer = (IRangeValueProvider)FrameworkElementAutomationPeer.CreatePeerForElement(control);

        peer.SetValue(3);

        Assert.True(peer.IsReadOnly);
        Assert.Equal(0, control.Value);
    }
}
