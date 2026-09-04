// Exercise 037 - Template Part Subscriptions (intermediate).
// Goal:   Subscribe to a template part's events without leaking the part you replaced.
// Drills: OnApplyTemplate running more than once, unsubscribing from the previous part
//         before wiring the new one, and why a stale subscription is both a leak and a bug.
// Passes: dotnet test --filter FullyQualifiedName~Ex037_
//
// OnApplyTemplate is not a constructor. It runs again every time the Template property
// changes - a theme switch, a restyle, a control re-used in a different scope. A handler
// added there and never removed keeps the old element alive and keeps reacting to it.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;

namespace FeWoLearning.Uno.Exercises.Intermediate;

public partial class Ex037_TemplatePartSubscriptions : Control
{
    private static ControlTemplate ButtonTemplate() => (ControlTemplate)XamlReader.Load(
        """
        <ControlTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                         xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                         TargetType="Control">
            <Button x:Name="PART_Trigger" Content="go" />
        </ControlTemplate>
        """);

    /// <summary>Test fixture: two distinct templates, both providing the trigger part.</summary>
    public static readonly ControlTemplate FirstTemplate = ButtonTemplate();

    /// <summary>Test fixture: a second template, so re-templating produces a new part.</summary>
    public static readonly ControlTemplate SecondTemplate = ButtonTemplate();

    /// <summary>Test fixture: a template with no trigger at all.</summary>
    public static readonly ControlTemplate WithoutTrigger = (ControlTemplate)XamlReader.Load(
        """
        <ControlTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                         TargetType="Control">
            <Border />
        </ControlTemplate>
        """);

    // The element the handler is currently attached to. Not "the current part" - the part
    // the *subscription* belongs to, which is the only thing "-=" can be aimed at once the
    // template has moved on.
    private Button? _trigger;

    /// <summary>How many times the currently wired trigger has been pressed.</summary>
    public int Presses { get; private set; }

    /// <summary>
    /// Wires the "PART_Trigger" button so pressing it raises <see cref="Presses"/>.
    /// Any previously wired trigger is released first.
    /// </summary>
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        // Detach first, and unconditionally. Doing it after the lookup means a template
        // without the part never detaches at all.
        if (_trigger is not null)
        {
            _trigger.Click -= OnTriggerClick;
        }

        _trigger = GetTemplateChild("PART_Trigger") as Button;

        if (_trigger is not null)
        {
            _trigger.Click += OnTriggerClick;
        }
    }

    private void OnTriggerClick(object sender, RoutedEventArgs args) => Presses++;
}
