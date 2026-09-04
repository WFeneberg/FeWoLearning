// Exercise 080 - Custom Automation Peer (advanced).
// Goal:   Make a custom control reachable by a screen reader and by a test.
// Drills: OnCreateAutomationPeer, FrameworkElementAutomationPeer, IInvokeProvider and
//         IToggleProvider, and the Core overrides that name a control to assistive tech.
// Passes: dotnet test --filter FullyQualifiedName~Ex080_
//
// A control with no peer is invisible to a screen reader, and its behaviour is unreachable
// by anything but synthetic input - which is also why this whole track can press a button
// at all. The peer is not extra work bolted on: it is the control's API for everything
// that is not a mouse.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Automation.Provider;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Exercises.Advanced;

/// <summary>
/// A switch that can be pressed and toggled, with no visual template at all - the peer is
/// the whole surface a test or a screen reader needs.
/// </summary>
public partial class Ex080_CustomAutomationPeer : Control
{
    public static readonly DependencyProperty IsOnProperty =
        DependencyProperty.Register(
            nameof(IsOn),
            typeof(bool),
            typeof(Ex080_CustomAutomationPeer),
            new PropertyMetadata(false));

    /// <summary>Whether the switch is on.</summary>
    public bool IsOn
    {
        get => (bool)GetValue(IsOnProperty);
        set => SetValue(IsOnProperty, value);
    }

    /// <summary>How many times the switch has been pressed.</summary>
    public int Presses { get; private set; }

    /// <summary>Presses the switch: counts it and flips <see cref="IsOn"/>.</summary>
    public void Press()
    {
        Presses++;
        IsOn = !IsOn;
    }

    protected override AutomationPeer OnCreateAutomationPeer() =>
        // TODO: return the peer below. Without this override the framework builds a generic
        // peer, and neither Invoke nor Toggle is reachable.
        throw new NotImplementedException("TODO: Ex080 - hand out the custom peer");
}

/// <summary>
/// The peer: names the control for assistive technology and exposes the two patterns.
/// </summary>
public sealed class Ex080_SwitchPeer : FrameworkElementAutomationPeer, IInvokeProvider, IToggleProvider
{
    public Ex080_SwitchPeer(Ex080_CustomAutomationPeer owner)
        : base(owner)
    {
    }

    private Ex080_CustomAutomationPeer Switch => (Ex080_CustomAutomationPeer)Owner;

    /// <summary>The toggle state the pattern reports.</summary>
    public ToggleState ToggleState =>
        throw new NotImplementedException("TODO: Ex080 - report the toggle state");

    /// <summary>The Invoke pattern: does what pressing the control does.</summary>
    public void Invoke() =>
        throw new NotImplementedException("TODO: Ex080 - invoke the control");

    /// <summary>The Toggle pattern: moves to the next state, which here is the same press.</summary>
    public void Toggle() =>
        throw new NotImplementedException("TODO: Ex080 - toggle the control");

    /// <summary>
    /// The type an assistive technology should treat this as. A switch reads as a button.
    /// </summary>
    protected override AutomationControlType GetAutomationControlTypeCore() =>
        throw new NotImplementedException("TODO: Ex080 - declare the control type");

    /// <summary>The class name a screen reader announces.</summary>
    protected override string GetClassNameCore() =>
        throw new NotImplementedException("TODO: Ex080 - declare the class name");
}
