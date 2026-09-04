using FeWoLearning.Uno.Exercises.Advanced;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Automation.Provider;

namespace FeWoLearning.Uno.Tests.Advanced;

public class Ex080_CustomAutomationPeerTests : UnoTestContext
{
    private static (Ex080_CustomAutomationPeer Control, AutomationPeer Peer) Switch()
    {
        var control = new Ex080_CustomAutomationPeer();
        return (control, FrameworkElementAutomationPeer.CreatePeerForElement(control));
    }

    [Fact]
    public void The_Control_Hands_Out_Its_Own_Peer()
    {
        var (_, peer) = Switch();

        // Without the override the framework builds a generic peer, and everything below
        // is unreachable - by a test and by a screen reader alike.
        Assert.IsType<Ex080_SwitchPeer>(peer);
    }

    [Fact]
    public void The_Peer_Offers_The_Invoke_Pattern()
    {
        var (_, peer) = Switch();

        Assert.IsAssignableFrom<IInvokeProvider>(peer);
    }

    [Fact]
    public void Invoking_Presses_The_Control()
    {
        var (control, peer) = Switch();

        ((IInvokeProvider)peer).Invoke();

        Assert.Equal(1, control.Presses);
        Assert.True(control.IsOn);
    }

    [Fact]
    public void Toggling_Presses_The_Control_Too()
    {
        var (control, peer) = Switch();

        ((IToggleProvider)peer).Toggle();

        // Two patterns over one behaviour: a screen reader may use either, and both must
        // end up in the same place as a press.
        Assert.Equal(1, control.Presses);
        Assert.True(control.IsOn);
    }

    [Fact]
    public void The_Toggle_State_Follows_The_Control()
    {
        var (control, peer) = Switch();
        var toggle = (IToggleProvider)peer;

        Assert.Equal(ToggleState.Off, toggle.ToggleState);

        control.IsOn = true;

        // Read from the control, not cached in the peer: a peer with its own copy of the
        // state announces the wrong thing the moment anything else changes it.
        Assert.Equal(ToggleState.On, toggle.ToggleState);
    }

    [Fact]
    public void Repeated_Invocations_Flip_Back_And_Forth()
    {
        var (control, peer) = Switch();
        var invoke = (IInvokeProvider)peer;

        invoke.Invoke();
        invoke.Invoke();

        Assert.Equal(2, control.Presses);
        Assert.False(control.IsOn);
    }

    [Fact]
    public void The_Peer_Announces_A_Control_Type()
    {
        var (_, peer) = Switch();

        Assert.Equal(AutomationControlType.Button, peer.GetAutomationControlType());
    }

    [Fact]
    public void The_Peer_Announces_A_Class_Name()
    {
        var (_, peer) = Switch();

        // The default is empty, and an assistive technology then has nothing to say about
        // what this control is.
        Assert.Equal(nameof(Ex080_CustomAutomationPeer), peer.GetClassName());
    }

    [Fact]
    public void The_Peer_Points_At_Its_Owner()
    {
        var (control, peer) = Switch();

        ((IInvokeProvider)peer).Invoke();

        Assert.Equal(1, control.Presses);
        Assert.Equal(0, new Ex080_CustomAutomationPeer().Presses);
    }
}
