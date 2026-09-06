// Exercise 058 - Retemplating a control, in code (intermediate). REFERENCE SOLUTION.
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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace FeWoLearning.Wpf.Exercises.Intermediate;

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

    public FrameworkElement? GetPart(string name) => GetTemplateChild(name) as FrameworkElement;
}

public static class Ex058_ControlTemplateAndTemplateBinding
{
    public static ControlTemplate BuildTemplate()
    {
        var template = new ControlTemplate(typeof(Ex058_HeaderedControl));
        var factory = new FrameworkElementFactory(typeof(TextBlock));
        factory.Name = "PART_Header";
        factory.SetBinding(TextBlock.TextProperty, new Binding(nameof(Ex058_HeaderedControl.HeaderText))
        {
            RelativeSource = RelativeSource.TemplatedParent,
        });
        template.VisualTree = factory;
        return template;
    }

    public static void Retemplate(Ex058_HeaderedControl control, ControlTemplate template)
    {
        control.Template = template;
        control.ApplyTemplate();
    }
}
