// Exercise 058 - Retemplating a control, in code (intermediate).
// Goal:   A ControlTemplate replaces a control's ENTIRE visual tree while leaving its actual
//         DependencyProperty-backed API untouched - the whole point of the separation between a
//         control's behavior and its look. Building one in code is the same shape XAML's
//         <ControlTemplate> takes: a FrameworkElementFactory tree, with a named part a caller can
//         reach back into once the template is applied.
//
//         {TemplateBinding X} itself has NO code form - it is markup-only, and this tier has no
//         XAML (see Ex025 for the same honest substitution, made for the same reason). Its real
//         code equivalent - what TemplateBinding itself compiles down to - is a plain Binding
//         with RelativeSource = RelativeSource.TemplatedParent: a template part binding back to
//         a property on the control instance the template was applied to. Map this onto
//         {TemplateBinding HeaderText} once you reach XAML.
// Drills: building a ControlTemplate whose VisualTree is a FrameworkElementFactory naming a part
//         (factory.Name = "...", NOT SetValue(FrameworkElement.NameProperty, ...) - measured
//         directly: the latter never registers in the template's own name scope, so
//         GetTemplateChild/FindName never find it) bound via RelativeSource.TemplatedParent;
//         assigning that template to a control and forcing it to apply immediately via
//         Control.ApplyTemplate(); and reaching the applied template's named part back through
//         the otherwise-protected Control.GetTemplateChild, exposed here through a small public
//         wrapper.
// Passes: dotnet test --filter FullyQualifiedName~Ex058_

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace FeWoLearning.Wpf.Exercises.Intermediate;

/// <summary>
/// Ready to use - not the subject of this row. A minimal templated control: one dependency
/// property a template's part can bind to, and a public wrapper around the otherwise-protected
/// GetTemplateChild so callers outside a Control subclass can reach into whatever the applied
/// template actually built.
/// </summary>
public class Ex058_HeaderedControl : Control
{
    public static readonly DependencyProperty HeaderTextProperty = DependencyProperty.Register(
        nameof(HeaderText),
        typeof(string),
        typeof(Ex058_HeaderedControl),
        new PropertyMetadata(string.Empty));

    public string HeaderText
    {
        get => (string)GetValue(HeaderTextProperty);
        set => SetValue(HeaderTextProperty, value);
    }

    /// <summary>Reaches into whatever the currently-applied template built, by part name.</summary>
    public FrameworkElement? GetPart(string name) => GetTemplateChild(name) as FrameworkElement;
}

public static class Ex058_ControlTemplateAndTemplateBinding
{
    /// <summary>
    /// Builds a ControlTemplate(typeof(Ex058_HeaderedControl)) whose visual tree is a single
    /// TextBlock named "PART_Header", with its Text bound to the templated control's own
    /// HeaderText via RelativeSource.TemplatedParent - the code equivalent of
    /// {TemplateBinding HeaderText}.
    /// </summary>
    public static ControlTemplate BuildTemplate()
        => throw new NotImplementedException("TODO: Ex058 - build a ControlTemplate for Ex058_HeaderedControl whose VisualTree is a FrameworkElementFactory for a TextBlock, named \"PART_Header\" through the factory's own Name property (not SetValue(NameProperty, ...) - that never registers in the template's name scope), with its Text bound back to HeaderText through a TemplatedParent RelativeSource binding - the code equivalent of {TemplateBinding HeaderText}");

    /// <summary>
    /// Retemplates <paramref name="control"/> with <paramref name="template"/> and forces it to
    /// apply immediately, so a caller can reach the new template's parts right away instead of
    /// waiting for a layout pass to trigger it.
    /// </summary>
    public static void Retemplate(Ex058_HeaderedControl control, ControlTemplate template)
        => throw new NotImplementedException("TODO: Ex058 - assign template to control.Template, then call control.ApplyTemplate() so the new template's visual tree is built immediately rather than only whenever the next layout pass happens to trigger it");
}
