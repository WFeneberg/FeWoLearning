// Exercise 081 - Template Swapping (advanced).
// Goal:   Replace a control's template at runtime and come out in the right state.
// Drills: OnApplyTemplate running again, releasing what the old template owned, and
//         re-entering every state the control is logically in.
// Passes: dotnet test --filter FullyQualifiedName~Ex081_
//
// A theme switch, a restyle, a control re-used in another scope: the template changes and
// the control's own state does not. Three things have to happen in order - let go of the
// old parts, find the new ones, re-enter the states - and each of them is silently
// skippable.
//
// The part worth thinking about is the letting go: the old part is detached but still
// reachable from anything that captured it, and a control that keeps writing to it is both
// leaking the old tree and showing nothing.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;

namespace FeWoLearning.Uno.Exercises.Advanced;

public partial class Ex081_TemplateSwapping : Control
{
    private static ControlTemplate Template() => (ControlTemplate)XamlReader.Load(
        """
        <ControlTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                         xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                         TargetType="Control">
            <Border x:Name="PART_Fill" Width="20" Height="20" Opacity="1">
                <VisualStateManager.VisualStateGroups>
                    <VisualStateGroup x:Name="HighlightStates">
                        <VisualState x:Name="Normal" />
                        <VisualState x:Name="Highlighted">
                            <VisualState.Setters>
                                <Setter Target="PART_Fill.Width" Value="60" />
                            </VisualState.Setters>
                        </VisualState>
                    </VisualStateGroup>
                </VisualStateManager.VisualStateGroups>
            </Border>
        </ControlTemplate>
        """);

    /// <summary>Test fixture: two equivalent templates, so a swap produces new parts.</summary>
    public static readonly ControlTemplate FirstTemplate = Template();

    /// <summary>Test fixture: the second one.</summary>
    public static readonly ControlTemplate SecondTemplate = Template();

    public static readonly DependencyProperty IsHighlightedProperty =
        DependencyProperty.Register(
            nameof(IsHighlighted),
            typeof(bool),
            typeof(Ex081_TemplateSwapping),
            new PropertyMetadata(false, OnIsHighlightedChanged));

    public bool IsHighlighted
    {
        get => (bool)GetValue(IsHighlightedProperty);
        set => SetValue(IsHighlightedProperty, value);
    }

    /// <summary>The part this control is currently working with, or null when it has none.</summary>
    public Border? Fill { get; private set; }

    /// <summary>How many templates this control has been through.</summary>
    public int TemplatesApplied { get; private set; }

    /// <summary>How many parts it has released on the way.</summary>
    public int PartsReleased { get; private set; }

    /// <summary>
    /// Dims the current part, whatever it is. Used by the tests to see which part the
    /// control believes it owns.
    /// </summary>
    public void DimCurrentPart()
    {
        if (Fill is not null)
        {
            Fill.Opacity = 0.5;
        }
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        // Release first. After the lookup, `Fill` is the new part and this would reset the
        // wrong element - leaving the old one dimmed for ever and still referenced.
        if (Fill is not null)
        {
            Fill.Opacity = 1;
            PartsReleased++;
        }

        Fill = GetTemplateChild("PART_Fill") as Border;
        TemplatesApplied++;

        // The state groups live in the template and start empty, so the control has to say
        // what it is again. Skipping this makes a theme switch quietly reset every state.
        UpdateVisualState();
    }

    private static void OnIsHighlightedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) =>
        ((Ex081_TemplateSwapping)sender).UpdateVisualState();

    private void UpdateVisualState() =>
        VisualStateManager.GoToState(this, IsHighlighted ? "Highlighted" : "Normal", useTransitions: false);
}
