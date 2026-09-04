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
    public static Ex088_ParseResult TryLoad(string markup) =>
        // TODO: parse it, catch what a bad fragment throws, and return the result.
        // XamlReader.Load throws XamlParseException for malformed markup and can throw
        // other things for markup that is well-formed XML but not valid XAML - so the
        // catch has to be wider than one type.
        throw new NotImplementedException("TODO: Ex088 - load the markup, fallibly");

    /// <summary>
    /// Wraps <paramref name="innerMarkup"/> in a Border whose root declares both
    /// namespaces, so a caller can pass a fragment without repeating them.
    /// </summary>
    public static string WrapFragment(string innerMarkup) =>
        throw new NotImplementedException("TODO: Ex088 - wrap the fragment with its namespaces");
}
