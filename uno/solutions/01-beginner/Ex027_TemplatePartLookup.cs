// Exercise 027 - Template Part Lookup (beginner).
// Goal:   Reach into your own template for the parts you need, and survive their absence.
// Drills: OnApplyTemplate, GetTemplateChild, the PART_ naming contract, and keeping the
//         control working when a template does not provide a part.
// Passes: dotnet test --filter FullyQualifiedName~Ex027_

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

    // Nullable, and reassigned on every template apply. Holding a part from a template
    // that has been replaced means writing into a detached element: nothing appears, and
    // the old tree cannot be collected.
    private TextBlock? _label;

    /// <summary>The text this control wants its template to show.</summary>
    public string Caption
    {
        get => (string)GetValue(CaptionProperty);
        set => SetValue(CaptionProperty, value);
    }

    /// <summary>
    /// Called every time a template is applied. Look up the part named "PART_Label", keep
    /// it, and push the current <see cref="Caption"/> into it.
    /// </summary>
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        // `as` rather than a cast: the name may be taken by something that is not a
        // TextBlock in a template this control has never seen.
        _label = GetTemplateChild("PART_Label") as TextBlock;

        // Re-read the property, because it may have moved while there was no part.
        UpdateLabel(Caption);
    }

    private static void OnCaptionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) =>
        ((Ex027_TemplatePartLookup)sender).UpdateLabel((string)args.NewValue);

    private void UpdateLabel(string caption)
    {
        if (_label is null)
        {
            // No part, no problem: the property is still the source of truth, and the next
            // template that does provide the part picks the value up in OnApplyTemplate.
            return;
        }

        _label.Text = caption;
    }
}
