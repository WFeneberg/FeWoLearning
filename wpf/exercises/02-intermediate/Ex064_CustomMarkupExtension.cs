// Exercise 064 - A custom MarkupExtension (intermediate).
// Goal:   A MarkupExtension is a normal class with one virtual method - ProvideValue(IServiceProvider) -
//         so calling it directly with a hand-built IServiceProvider/IProvideValueTarget is a
//         legitimate, complete drill with no XAML involved at all, the same substitution rows 025
//         and 058 already made for this XAML-free tier: that precedent is reason enough on its
//         own, and it is the actual reason this row ships no XAML. (An earlier draft of this row
//         additionally claimed XamlReader.Parse could not resolve a custom MarkupExtension by its
//         suffix-stripped name - that claim was backwards. Re-measured with a five-name matrix
//         plus a negative control: XamlReader.Parse DOES honour suffix stripping - a type named
//         FooExtension resolves from both {local:FooExtension ...} and {local:Foo ...}; only a
//         type reference that does not exist at all throws XamlParseException, whether that is a
//         genuinely unknown name or a real type's bare name with "Extension" wrongly appended. The
//         row's scope decision does not depend on this fact either way, so it is recorded here and
//         nowhere else - not turned into a graded assertion.)
// Drills: MarkupExtension.ProvideValue(IServiceProvider) and IProvideValueTarget.TargetProperty -
//         a markup extension that hands back whatever the TARGET DependencyProperty's own
//         registered default value is, instead of a value baked into the extension itself. That
//         only works by actually reading IProvideValueTarget off the service provider and reading
//         ITS TargetProperty - ignoring either one produces a constant that is right for at most
//         one kind of target property and silently wrong for every other.
// Passes: dotnet test --filter FullyQualifiedName~Ex064_

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
        => throw new NotImplementedException("TODO: Ex064 - obtain the IProvideValueTarget service from serviceProvider; from it, determine which DependencyProperty is being targeted and which DependencyObject it belongs to; resolve and return THAT property's own registered default value for the target's actual type - never a value hard-coded into this extension");
}
