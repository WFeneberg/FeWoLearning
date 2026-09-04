// Exercise 038 - Orthogonal State Groups (intermediate).
// Goal:   Track two independent conditions in two state groups at the same time.
// Drills: one VisualState active *per group*, GoToState addressing a group by the state it
//         names, and updating both groups whenever either input changes.
// Passes: dotnet test --filter FullyQualifiedName~Ex038_
//
// Every WinUI control does this: CommonStates and CheckStates in a CheckBox, FocusStates
// and CommonStates in a Button. Squeezing both conditions into one group needs a state per
// combination - four for two booleans - and the count multiplies from there.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;

namespace FeWoLearning.Uno.Exercises.Intermediate;

public partial class Ex038_OrthogonalStateGroups : Control
{
    /// <summary>
    /// Test fixture: two groups over one part. "AvailabilityStates" dims the fill when
    /// disabled; "CheckStates" widens it when checked. Neither knows about the other.
    /// </summary>
    public static readonly ControlTemplate TwoGroupTemplate = (ControlTemplate)XamlReader.Load(
        """
        <ControlTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                         xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                         TargetType="Control">
            <Border x:Name="PART_Fill" Width="20" Height="20">
                <VisualStateManager.VisualStateGroups>
                    <VisualStateGroup x:Name="AvailabilityStates">
                        <VisualState x:Name="Available" />
                        <VisualState x:Name="Unavailable">
                            <VisualState.Setters>
                                <Setter Target="PART_Fill.Opacity" Value="0.4" />
                            </VisualState.Setters>
                        </VisualState>
                    </VisualStateGroup>
                    <VisualStateGroup x:Name="CheckStates">
                        <VisualState x:Name="Unchecked" />
                        <VisualState x:Name="Checked">
                            <VisualState.Setters>
                                <Setter Target="PART_Fill.Width" Value="60" />
                            </VisualState.Setters>
                        </VisualState>
                    </VisualStateGroup>
                </VisualStateManager.VisualStateGroups>
            </Border>
        </ControlTemplate>
        """);

    public static readonly DependencyProperty IsAvailableProperty =
        DependencyProperty.Register(
            nameof(IsAvailable),
            typeof(bool),
            typeof(Ex038_OrthogonalStateGroups),
            new PropertyMetadata(true, OnStateInputChanged));

    public static readonly DependencyProperty IsCheckedProperty =
        DependencyProperty.Register(
            nameof(IsChecked),
            typeof(bool),
            typeof(Ex038_OrthogonalStateGroups),
            new PropertyMetadata(false, OnStateInputChanged));

    public bool IsAvailable
    {
        get => (bool)GetValue(IsAvailableProperty);
        set => SetValue(IsAvailableProperty, value);
    }

    public bool IsChecked
    {
        get => (bool)GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }

    /// <summary>The state names last requested, in the order they were requested.</summary>
    public IReadOnlyList<string> LastRequestedStates => _requested;

    private readonly List<string> _requested = [];

    /// <summary>
    /// Enters "Available"/"Unavailable" and "Unchecked"/"Checked" to match the two
    /// properties, recording each name in <see cref="LastRequestedStates"/> - availability
    /// first, then the check state.
    /// </summary>
    public void UpdateVisualState() =>
        // TODO: request both states, every time. Requesting only the one whose property
        // changed works until the template is re-applied and the other group resets.
        throw new NotImplementedException("TODO: Ex038 - request the state of both groups");

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        UpdateVisualState();
    }

    private static void OnStateInputChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) =>
        ((Ex038_OrthogonalStateGroups)sender).UpdateVisualState();
}
