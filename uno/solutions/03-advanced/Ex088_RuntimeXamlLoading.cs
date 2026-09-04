// Exercise 088 - Runtime Xaml Loading (advanced).
// Goal:   Build UI from a string at runtime, and survive a string that is wrong.
// Drills: XamlReader.Load, the namespace declarations a fragment needs, and turning a
//         parse failure into a result instead of an exception.
// Passes: dotnet test --filter FullyQualifiedName~Ex088_
//
// Server-driven UI, a template editor, a plug-in that ships markup: all of them end up
// here. And all of them are handling *untrusted* markup, so the parse has to be a fallible
// operation rather than a call that throws through the caller.
//
// Two details cost an afternoon each: the default namespace has to be declared on the
// fragment's root (there is no ambient one), and x: has to be declared separately before
// x:Name will parse at all.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;

namespace FeWoLearning.Uno.Exercises.Advanced;

/// <summary>What a parse attempt produced.</summary>
/// <param name="Root">The loaded root element, or null when the parse failed.</param>
/// <param name="Error">The failure message, or null when it succeeded.</param>
public sealed record Ex088_ParseResult(UIElement? Root, string? Error)
{
    /// <summary>Whether the markup parsed.</summary>
    public bool Succeeded => Root is not null;
}

public static class Ex088_RuntimeXamlLoading
{
    /// <summary>The default XAML namespace a fragment needs on its root element.</summary>
    public const string PresentationNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";

    /// <summary>The x namespace, needed for x:Name and friends.</summary>
    public const string XamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";

    /// <summary>
    /// Loads <paramref name="markup"/>, reporting a failure rather than throwing. Markup
    /// that parses into something other than a UIElement counts as a failure too.
    /// </summary>
    public static Ex088_ParseResult TryLoad(string markup)
    {
        try
        {
            // Anything can come back: a brush, a style, a resource dictionary. A caller
            // that wanted something to put on screen gets a failure here rather than a
            // cast exception three frames later.
            return XamlReader.Load(markup) is UIElement element
                ? new Ex088_ParseResult(element, null)
                : new Ex088_ParseResult(null, "The markup did not produce a UIElement.");
        }
        catch (Exception error)
        {
            // Deliberately wide. Malformed XML, an undeclared prefix and an unknown
            // element all fail differently, and untrusted markup must not be able to throw
            // through this method at all.
            return new Ex088_ParseResult(null, error.Message);
        }
    }

    /// <summary>
    /// Wraps <paramref name="innerMarkup"/> in a Border whose root declares both
    /// namespaces, so a caller can pass a fragment without repeating them.
    /// </summary>
    public static string WrapFragment(string innerMarkup) =>
        // Both namespaces on the root: there is no ambient default for a runtime fragment,
        // and x: has to be declared before x:Name will even parse.
        $"""<Border xmlns="{PresentationNamespace}" xmlns:x="{XamlNamespace}">{innerMarkup}</Border>""";
}
