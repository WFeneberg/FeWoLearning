// Exercise 028 - Visual State Groups (beginner).
// Goal:   Give a control named looks and switch between them.
// Drills: VisualStateManager.GoToState, VisualState.Setters targeting a template part, and
//         driving the state from a property change rather than from the outside.
// Passes: dotnet test --filter FullyQualifiedName~Ex028_
//
// The states live in the template, the decision lives in the control. That split is what
// lets a designer restyle a state without touching the logic that enters it - and it is
// why a control calls GoToState for a state a template may not even declare.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;

namespace FeWoLearning.Uno.Exercises.Beginner;

public partial class Ex028_VisualStateGroups : Control
{
    /// <summary>
    /// Test fixture: a template with a "HighlightStates" group declaring "Normal" (nothing
    /// set) and "Highlighted" (the fill's Opacity drops to 0.25).
    /// </summary>
    public static readonly ControlTemplate HighlightTemplate = (ControlTemplate)XamlReader.Load(
        """
        <ControlTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                         xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                         TargetType="Control">
            <Border x:Name="PART_Fill" Width="20" Height="20">
                <VisualStateManager.VisualStateGroups>
                    <VisualStateGroup x:Name="HighlightStates">
                        <VisualState x:Name="Normal" />
                        <VisualState x:Name="Highlighted">
                            <VisualState.Setters>
                                <Setter Target="PART_Fill.Opacity" Value="0.25" />
                            </VisualState.Setters>
                        </VisualState>
                    </VisualStateGroup>
                </VisualStateManager.VisualStateGroups>
            </Border>
        </ControlTemplate>
        """);

    public static readonly DependencyProperty IsHighlightedProperty =
        DependencyProperty.Register(
            nameof(IsHighlighted),
            typeof(bool),
            typeof(Ex028_VisualStateGroups),
            new PropertyMetadata(false, OnIsHighlightedChanged));

    /// <summary>Whether the control is currently highlighted.</summary>
    public bool IsHighlighted
    {
        get => (bool)GetValue(IsHighlightedProperty);
        set => SetValue(IsHighlightedProperty, value);
    }

    /// <summary>
    /// The name of the state this control last asked for - "Normal" or "Highlighted".
    /// Recorded so a test can see the decision even when no template declares the state.
    /// </summary>
    public string LastRequestedState { get; private set; } = "";

    /// <summary>
    /// Enters the state that matches <see cref="IsHighlighted"/> and records its name.
    /// </summary>
    public void UpdateVisualState() =>
        // TODO: pick the state name, remember it in LastRequestedState, and ask the
        // VisualStateManager to go there. No transitions.
        throw new NotImplementedException("TODO: Ex028 - go to the state that matches the property");

    /// <summary>A freshly templated control must already be in the right state.</summary>
    protected override void OnApplyTemplate() =>
        // TODO: call the base implementation, then update the state. Without this a control
        // that was highlighted before its template arrived comes back up looking normal.
        throw new NotImplementedException("TODO: Ex028 - enter the current state on template apply");

    private static void OnIsHighlightedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) =>
        throw new NotImplementedException("TODO: Ex028 - update the state when the property changes");
}
