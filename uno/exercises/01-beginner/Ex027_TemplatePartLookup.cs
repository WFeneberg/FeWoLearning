// Exercise 027 - Template Part Lookup (beginner).
// Goal:   Reach into your own template for the parts you need, and survive their absence.
// Drills: OnApplyTemplate, GetTemplateChild, the PART_ naming contract, and keeping the
//         control working when a template does not provide a part.
// Passes: dotnet test --filter FullyQualifiedName~Ex027_
//
// A template is somebody else's markup: a designer can replace it and leave out anything.
// A control that dereferences a missing part crashes an app that only changed a style,
// which is why every part lookup is a soft one.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;

namespace FeWoLearning.Uno.Exercises.Beginner;

public partial class Ex027_TemplatePartLookup : Control
{
    /// <summary>Test fixture: a template that provides the label part.</summary>
    public static readonly ControlTemplate WithLabel = (ControlTemplate)XamlReader.Load(
        """
        <ControlTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                         xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                         TargetType="Control">
            <Border>
                <TextBlock x:Name="PART_Label" />
            </Border>
        </ControlTemplate>
        """);

    /// <summary>Test fixture: a template that leaves the label part out.</summary>
    public static readonly ControlTemplate WithoutLabel = (ControlTemplate)XamlReader.Load(
        """
        <ControlTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                         TargetType="Control">
            <Border />
        </ControlTemplate>
        """);

    public static readonly DependencyProperty CaptionProperty =
        DependencyProperty.Register(
            nameof(Caption),
            typeof(string),
            typeof(Ex027_TemplatePartLookup),
            new PropertyMetadata("", OnCaptionChanged));

    /// <summary>The text this control wants its template to show.</summary>
    public string Caption
    {
        get => (string)GetValue(CaptionProperty);
        set => SetValue(CaptionProperty, value);
    }

    // TODO: hold the part here once it has been found, and nothing else. Do not walk the
    // visual tree for it on every change.

    /// <summary>
    /// Called every time a template is applied. Look up the part named "PART_Label", keep
    /// it, and push the current <see cref="Caption"/> into it.
    /// </summary>
    protected override void OnApplyTemplate() =>
        // TODO: call the base implementation, find the part with GetTemplateChild, remember
        // it, and update it. A template without the part must not throw.
        throw new NotImplementedException("TODO: Ex027 - find and remember the label part");

    private static void OnCaptionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) =>
        // TODO: push the new caption into the part, if there is one. Caption can change
        // before any template has been applied.
        throw new NotImplementedException("TODO: Ex027 - update the label when the caption changes");
}
