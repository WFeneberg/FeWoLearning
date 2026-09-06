// Exercise 064 - A custom MarkupExtension, and the XAML question decided the way row 058 decided
// it (intermediate). REFERENCE SOLUTION.
// Goal:   A MarkupExtension is a normal class with one virtual method - ProvideValue(IServiceProvider) -
//         so calling it directly with a hand-built IServiceProvider/IProvideValueTarget is a
//         legitimate, complete drill with no XAML involved at all, the same substitution rows 025
//         and 058 already made for this XAML-free tier. Probed directly for this row, the way row
//         058 probed XamlReader.Parse for its own subject: a custom MarkupExtension CAN be resolved
//         by XamlReader.Parse from a runtime-compiled clr-namespace reference - but only by its
//         literal type name, "Extension" suffix included ({local:FooExtension ...}); the
//         suffix-stripping convention markup-compiled XAML normally allows ({local:Foo ...}) is a
//         XAML-COMPILER feature, not something XamlReader.Parse's runtime type resolution honors -
//         {local:UpperCase ...} throws XamlParseException ("unknown type"), {local:UpperCaseExtension
//         ...} succeeds. Since the row's own Concepts (ProvideValue, IProvideValueTarget) are both
//         already fully exercised without it, and the literal markup form is this finicky about a
//         detail unrelated to either concept, this row ships no XAML and no XamlReader.Parse test -
//         the measurement above is recorded here, not turned into a graded assertion.
// Drills: MarkupExtension.ProvideValue(IServiceProvider) and IProvideValueTarget.TargetProperty -
//         a markup extension that hands back whatever the TARGET DependencyProperty's own
//         registered default value is, instead of a value baked into the extension itself. That
//         only works by actually reading IProvideValueTarget off the service provider and reading
//         ITS TargetProperty - ignoring either one produces a constant that is right for at most
//         one kind of target property and silently wrong for every other.

using System.Windows;
using System.Windows.Markup;

namespace FeWoLearning.Wpf.Exercises.Intermediate;

/// <summary>
/// Resolves, at ProvideValue time, to whatever value the DEPENDENCY PROPERTY it is applied to
/// already declares as its own default - never a value this extension carries itself.
/// </summary>
public sealed class Ex064_PropertyDefaultExtension : MarkupExtension
{
    public override object? ProvideValue(IServiceProvider serviceProvider)
    {
        var target = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget))!;
        var targetProperty = (DependencyProperty)target.TargetProperty;
        var targetObject = (DependencyObject)target.TargetObject!;

        return targetProperty.GetMetadata(targetObject.GetType()).DefaultValue;
    }
}
