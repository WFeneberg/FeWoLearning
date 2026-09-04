// Exercise 087 - Control Library Styles (advanced).
// Goal:   Ship a control and its look as a library a consumer can use in one line.
// Drills: a ResourceDictionary with x:Class as the library's public entry point, an
//         implicit style keyed by the control type, and what a consumer has to do (and not
//         do) to pick it up.
// Passes: dotnet test --filter FullyQualifiedName~Ex087_
//
// The textbook answer is Themes/Generic.xaml, which the framework finds by itself. In Uno
// that lookup depends on the app head registering the library's dictionaries, and it does
// not happen here at all (see uno/README.md) - which is exactly why Uno.Toolkit and
// Uno.Material both ship a dictionary the app merges in App.xaml instead. This exercise
// builds that shape: it is the one that works everywhere.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Exercises.Advanced;

/// <summary>The library's control: API only, no look.</summary>
public partial class Ex087_LibraryControl : Control
{
    public Ex087_LibraryControl() => DefaultStyleKey = typeof(Ex087_LibraryControl);

    /// <summary>Test hook: the protected DefaultStyleKey this instance declared.</summary>
    public object? DeclaredStyleKey => DefaultStyleKey;
}

/// <summary>The library's entry point for consumers.</summary>
public static class Ex087_ControlLibraryStyles
{
    /// <summary>
    /// Merges the library's styles into <paramref name="scope"/> - the code equivalent of
    /// a consumer merging the dictionary in App.xaml.
    /// </summary>
    public static void MergeInto(FrameworkElement scope) =>
        scope.Resources.MergedDictionaries.Add(new Ex087_LibraryStyles());

    /// <summary>
    /// A panel with the library's styles merged in, holding <paramref name="controls"/> -
    /// what a consumer's page looks like.
    /// </summary>
    public static StackPanel CreateConsumerScope(params Ex087_LibraryControl[] controls)
    {
        var scope = new StackPanel();
        MergeInto(scope);

        foreach (var control in controls)
        {
            scope.Children.Add(control);
        }

        return scope;
    }
}
