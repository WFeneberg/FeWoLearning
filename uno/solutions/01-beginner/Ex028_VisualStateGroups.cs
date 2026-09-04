// Exercise 028 - Visual State Groups (beginner).
// Goal:   Give a control named looks and switch between them.
// Drills: VisualStateManager.GoToState, VisualState.Setters targeting a template part, and
//         driving the state from a property change rather than from the outside.
// Passes: dotnet test --filter FullyQualifiedName~Ex028_

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
    public void UpdateVisualState()
    {
        LastRequestedState = IsHighlighted ? "Highlighted" : "Normal";

        // A string, and a bool return nobody checks. GoToState says false when no group
        // declares the state, and that is not an error: whether a look exists for a state
        // is the template's decision, not this control's.
        VisualStateManager.GoToState(this, LastRequestedState, useTransitions: false);
    }

    /// <summary>A freshly templated control must already be in the right state.</summary>
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        // Without this, a control that was highlighted before its template arrived comes
        // up looking normal - the state groups are part of the template and start empty.
        UpdateVisualState();
    }

    private static void OnIsHighlightedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) =>
        ((Ex028_VisualStateGroups)sender).UpdateVisualState();
}
