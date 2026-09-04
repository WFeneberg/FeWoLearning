// Exercise 078 - Templated Control Inheritance (advanced).
// Goal:   Extend a templated control without breaking the base's contract.
// Drills: calling the base's OnApplyTemplate and state update, adding a state group the
//         base does not know about, and a DefaultStyleKey that follows the subclass.
// Passes: dotnet test --filter FullyQualifiedName~Ex078_
//
// The base owns its parts and its states; the subclass adds to both and must not replace
// either. Two mistakes are common and both compile: forgetting base.OnApplyTemplate, which
// leaves the base's parts unbound, and overriding the state update without calling the
// base, which silently drops every state the base was managing.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;

namespace FeWoLearning.Uno.Exercises.Advanced;

/// <summary>
/// The base: a badge with an availability state group and one part it owns.
/// </summary>
public partial class Ex078_BadgeBase : Control
{
    /// <summary>Test fixture: the template both classes share.</summary>
    public static readonly ControlTemplate SharedTemplate = (ControlTemplate)XamlReader.Load(
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
                    <VisualStateGroup x:Name="UrgencyStates">
                        <VisualState x:Name="Calm" />
                        <VisualState x:Name="Urgent">
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
            typeof(Ex078_BadgeBase),
            new PropertyMetadata(true, OnStateInputChanged));

    public Ex078_BadgeBase() => DefaultStyleKey = typeof(Ex078_BadgeBase);

    public bool IsAvailable
    {
        get => (bool)GetValue(IsAvailableProperty);
        set => SetValue(IsAvailableProperty, value);
    }

    /// <summary>The part the base owns, once a template has been applied.</summary>
    public Border? Fill { get; private set; }

    /// <summary>Test hook: the protected DefaultStyleKey this instance declared.</summary>
    public object? DeclaredStyleKey => DefaultStyleKey;

    /// <summary>
    /// Enters the states this class is responsible for. A subclass overrides this and calls
    /// it.
    /// </summary>
    protected virtual void UpdateVisualState() =>
        VisualStateManager.GoToState(this, IsAvailable ? "Available" : "Unavailable", useTransitions: false);

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        Fill = GetTemplateChild("PART_Fill") as Border;
        UpdateVisualState();
    }

    private static void OnStateInputChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) =>
        ((Ex078_BadgeBase)sender).UpdateVisualState();
}

/// <summary>
/// The subclass: adds urgency on top of availability.
/// </summary>
public partial class Ex078_TemplatedControlInheritance : Ex078_BadgeBase
{
    public static readonly DependencyProperty IsUrgentProperty =
        DependencyProperty.Register(
            nameof(IsUrgent),
            typeof(bool),
            typeof(Ex078_TemplatedControlInheritance),
            new PropertyMetadata(false, OnUrgencyChanged));

    public Ex078_TemplatedControlInheritance() =>
        // The base set its own key in its constructor; this overwrites it. A subclass that
        // forgets looks up the base's default style and never finds its own.
        DefaultStyleKey = typeof(Ex078_TemplatedControlInheritance);

    public bool IsUrgent
    {
        get => (bool)GetValue(IsUrgentProperty);
        set => SetValue(IsUrgentProperty, value);
    }

    /// <summary>How many times this subclass has updated its states.</summary>
    public int StateUpdates { get; private set; }

    protected override void UpdateVisualState()
    {
        // The base first: it owns the availability group, and skipping this compiles
        // perfectly while silently dropping every state the base was managing.
        base.UpdateVisualState();

        StateUpdates++;

        VisualStateManager.GoToState(this, IsUrgent ? "Urgent" : "Calm", useTransitions: false);
    }

    private static void OnUrgencyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) =>
        ((Ex078_TemplatedControlInheritance)sender).UpdateVisualState();
}
