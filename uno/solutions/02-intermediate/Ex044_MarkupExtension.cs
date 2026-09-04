// Exercise 044 - Markup Extension (intermediate).
// Goal:   Teach XAML a new way to produce a value.
// Drills: MarkupExtension.ProvideValue, properties on the extension set from markup, and
//         [MarkupExtensionReturnType] so the XAML compiler knows what it will get.
// Passes: dotnet test --filter FullyQualifiedName~Ex044_
//
// {Binding}, {StaticResource} and {x:Bind} are all this mechanism. An extension runs once,
// while the tree is built, and hands back a plain value - which is exactly why the ones
// that need to keep updating ({Binding}) return a binding object rather than a number.

using Microsoft.UI.Xaml.Markup;

namespace FeWoLearning.Uno.Exercises.Intermediate;

/// <summary>
/// Produces a size from a step count: <c>Base * Multiplier</c>, so a layout can be
/// described in steps rather than in pixels.
/// </summary>
[MarkupExtensionReturnType(ReturnType = typeof(double))]
public sealed partial class Ex044_MarkupExtension : MarkupExtension
{
    /// <summary>The size of one step. 8 unless markup says otherwise.</summary>
    public double Base { get; set; } = 8;

    /// <summary>How many steps. 1 unless markup says otherwise.</summary>
    public double Multiplier { get; set; } = 1;

    // Runs once, while the tree is built, and hands back a plain value. An extension that
    // needs to keep updating - {Binding} - returns a binding object from here instead of a
    // number, precisely because this method is never called again.
    protected override object ProvideValue() => Base * Multiplier;
}
